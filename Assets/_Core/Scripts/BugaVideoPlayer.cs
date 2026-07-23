using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Player kiosk de vídeos imersivos 360/180 (mono e estéreo).
/// Lê os vídeos da pasta persistentDataPath/360Videos, detecta o formato
/// pelo nome do arquivo (sufixos 360M/360S/180M/180S), reproduz em playlist
/// com fade de entrada/saída e registra o uso em usage_log.txt.
/// </summary>
public class BugaVideoPlayer : MonoBehaviour
{
    public enum VideoType
    {
        Mono360,
        Stereo360,
        Mono180,
        Stereo180
    }

    [Header("Configurações de Vídeo")]
    [Tooltip("Formato usado quando o nome do arquivo não indica o tipo e o config.json não define videoType.")]
    public VideoType videoType = VideoType.Mono360;

    [Header("Materiais para Skybox (Skybox/Panoramic)")]
    public Material material360Mono;
    public Material material360Stereo;
    public Material material180Mono;
    public Material material180Stereo;

    public VideoPlayer videoPlayer;

    public PlayerConfig Config { get; private set; }
    public string VideoFolderPath { get; private set; }

    static readonly string[] SupportedExtensions = { ".mp4", ".m4v", ".webm", ".mkv" };
    const string LatLongKeyword = "_MAPPING_LATITUDE_LONGITUDE_LAYOUT";

    static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
    static readonly int MappingProp = Shader.PropertyToID("_Mapping");
    static readonly int ImageTypeProp = Shader.PropertyToID("_ImageType");
    static readonly int LayoutProp = Shader.PropertyToID("_Layout");
    static readonly int ExposureProp = Shader.PropertyToID("_Exposure");

    readonly List<string> playlist = new List<string>();
    int currentIndex;
    int errorStreak;
    RenderTexture renderTexture;
    Material runtimeSkybox;
    bool ownsRuntimeSkybox;
    FallbackMessage fallback;
    Coroutine fadeRoutine;

    void Start()
    {
        VideoFolderPath = Path.Combine(Application.persistentDataPath, "360Videos");
        if (!Directory.Exists(VideoFolderPath))
        {
            Directory.CreateDirectory(VideoFolderPath);
            Debug.Log("Pasta criada em: " + VideoFolderPath);
        }

        UsageLogger.Init(VideoFolderPath);
        UsageLogger.Log("app_start");

        fallback = FallbackMessage.Create();
        BuildRuntimeSkybox();

        if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;
        videoPlayer.loopPointReached += OnVideoEnded;

        RestartExperience();
    }

    /// <summary>
    /// Recarrega o config.json, reescaneia a pasta de vídeos e recomeça a
    /// reprodução do primeiro item. Chamado no boot e pelo HMDController
    /// quando o usuário recoloca o headset — permite trocar o vídeo pelo
    /// USB sem fechar o app.
    /// </summary>
    public void RestartExperience()
    {
        Config = PlayerConfig.LoadOrCreate(VideoFolderPath);
        ScanFolder();
        currentIndex = 0;
        errorStreak = 0;

        if (playlist.Count == 0)
        {
            videoPlayer.Stop();
            fallback.Show("Nenhum vídeo encontrado.\n\nCopie um arquivo .mp4 para:\n" + VideoFolderPath);
            UsageLogger.Log("no_video_found");
            return;
        }

        fallback.Hide();
        PlayCurrent();
    }

    void ScanFolder()
    {
        playlist.Clear();
        playlist.AddRange(
            Directory.GetFiles(VideoFolderPath)
                .Where(f => SupportedExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
                .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase));
        Debug.Log("BugaVideoPlayer: " + playlist.Count + " vídeo(s) em " + VideoFolderPath);
    }

    void PlayCurrent()
    {
        string path = playlist[currentIndex];
        VideoType resolved = ResolveVideoType(path);
        ApplyProjection(resolved);
        ApplyFade(0f);

        Debug.Log("Carregando vídeo (" + resolved + "): " + path);
        UsageLogger.Log("video_load", Path.GetFileName(path) + ";type=" + resolved);

        videoPlayer.Stop();
        videoPlayer.url = path;
        videoPlayer.isLooping = Config.loop && playlist.Count == 1;
        videoPlayer.Prepare();
    }

    VideoType ResolveVideoType(string path)
    {
        if (Config.TryGetVideoTypeOverride(out VideoType fromConfig)) return fromConfig;
        VideoType? fromName = DetectFromFileName(path);
        return fromName ?? videoType;
    }

    // Convenção de nomes: "meuvideo_360M.mp4", "tour_180S.mp4" etc.
    // 360S/180S = estéreo; 360M/180M (ou só 360/180) = mono.
    static VideoType? DetectFromFileName(string path)
    {
        string n = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
        if (n.Contains("360s")) return VideoType.Stereo360;
        if (n.Contains("180s")) return VideoType.Stereo180;
        if (n.Contains("360")) return VideoType.Mono360;
        if (n.Contains("180")) return VideoType.Mono180;
        return null;
    }

    // Um único material Skybox/Panoramic configurado em runtime substitui
    // os quatro materiais por formato; eles ficam como base/fallback.
    void BuildRuntimeSkybox()
    {
        Material baseMaterial = material360Mono != null ? material360Mono
            : material360Stereo != null ? material360Stereo
            : material180Mono != null ? material180Mono
            : material180Stereo;

        if (baseMaterial != null)
        {
            runtimeSkybox = new Material(baseMaterial);
        }
        else
        {
            Shader panoramic = Shader.Find("Skybox/Panoramic");
            if (panoramic != null) runtimeSkybox = new Material(panoramic);
        }

        if (runtimeSkybox != null)
        {
            runtimeSkybox.name = "BugaRuntimeSkybox";
            ownsRuntimeSkybox = true;
        }
    }

    void ApplyProjection(VideoType type)
    {
        if (runtimeSkybox == null || !runtimeSkybox.HasProperty(ImageTypeProp))
        {
            runtimeSkybox = GetLegacyMaterial(type);
            ownsRuntimeSkybox = false;
            return;
        }

        runtimeSkybox.EnableKeyword(LatLongKeyword);
        runtimeSkybox.SetFloat(MappingProp, 1f); // Latitude Longitude
        bool is180 = type == VideoType.Mono180 || type == VideoType.Stereo180;
        runtimeSkybox.SetFloat(ImageTypeProp, is180 ? 1f : 0f);

        float layout = 0f; // mono
        if (type == VideoType.Stereo360) layout = 2f;      // over-under
        else if (type == VideoType.Stereo180) layout = 1f; // side-by-side
        runtimeSkybox.SetFloat(LayoutProp, layout);
    }

    Material GetLegacyMaterial(VideoType type)
    {
        switch (type)
        {
            case VideoType.Mono360: return material360Mono;
            case VideoType.Stereo360: return material360Stereo;
            case VideoType.Mono180: return material180Mono;
            case VideoType.Stereo180: return material180Stereo;
            default: return null;
        }
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        errorStreak = 0;

        int width = (int)vp.width;
        int height = (int)vp.height;
        if (renderTexture == null || renderTexture.width != width || renderTexture.height != height)
        {
            ReleaseRenderTexture();
            renderTexture = new RenderTexture(width, height, 0);
            renderTexture.Create();
        }

        vp.targetTexture = renderTexture;

        if (runtimeSkybox != null)
        {
            runtimeSkybox.SetTexture(MainTexProp, renderTexture);
            RenderSettings.skybox = runtimeSkybox;
#if UNITY_EDITOR
            DynamicGI.UpdateEnvironment();
#endif
        }

        vp.Play();
        UsageLogger.Log("video_play", Path.GetFileName(vp.url));
        StartFade(1f, null);
    }

    void OnVideoEnded(VideoPlayer vp)
    {
        if (vp.isLooping || playlist.Count == 0) return;

        int next = currentIndex + 1;
        if (next >= playlist.Count)
        {
            if (!Config.loop)
            {
                UsageLogger.Log("playlist_end");
                StartFade(0f, () =>
                {
                    vp.Stop();
                    fallback.Show("Fim da reprodução.");
                });
                return;
            }
            next = 0;
        }

        int target = next;
        StartFade(0f, () =>
        {
            currentIndex = target;
            PlayCurrent();
        });
    }

    void OnVideoError(VideoPlayer vp, string message)
    {
        string file = playlist.Count > 0
            ? Path.GetFileName(playlist[Mathf.Clamp(currentIndex, 0, playlist.Count - 1)])
            : "?";
        Debug.LogError("Erro ao reproduzir '" + file + "': " + message);
        UsageLogger.Log("video_error", file + ";" + message);

        errorStreak++;
        if (playlist.Count == 0 || errorStreak >= playlist.Count)
        {
            vp.Stop();
            fallback.Show("Não foi possível reproduzir os vídeos da pasta.\n\nVerifique o formato do arquivo\n(recomendado: .mp4 H.264/H.265).");
            return;
        }

        currentIndex = (currentIndex + 1) % playlist.Count;
        PlayCurrent();
    }

    void StartFade(float target, Action onDone)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(target, onDone));
    }

    // O fade usa o _Exposure do skybox (imagem) e o volume do áudio juntos.
    IEnumerator FadeRoutine(float target, Action onDone)
    {
        float duration = Config != null ? Mathf.Max(0f, Config.fadeSeconds) : 0.5f;
        float start = runtimeSkybox != null && runtimeSkybox.HasProperty(ExposureProp)
            ? runtimeSkybox.GetFloat(ExposureProp)
            : target;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float k = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
            ApplyFade(Mathf.Lerp(start, target, k));
            yield return null;
        }

        ApplyFade(target);
        fadeRoutine = null;
        onDone?.Invoke();
    }

    void ApplyFade(float value)
    {
        if (runtimeSkybox != null && runtimeSkybox.HasProperty(ExposureProp))
        {
            runtimeSkybox.SetFloat(ExposureProp, value);
        }
        float volume = Config != null ? Mathf.Clamp01(Config.volume) : 1f;
        SetVolume(value * volume);
    }

    void SetVolume(float value)
    {
        if (videoPlayer == null) return;
        for (ushort track = 0; track < videoPlayer.audioTrackCount; track++)
        {
            videoPlayer.SetDirectAudioVolume(track, Mathf.Clamp01(value));
        }
    }

    void ReleaseRenderTexture()
    {
        if (renderTexture == null) return;
        if (videoPlayer != null && videoPlayer.targetTexture == renderTexture)
        {
            videoPlayer.targetTexture = null;
        }
        renderTexture.Release();
        Destroy(renderTexture);
        renderTexture = null;
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.errorReceived -= OnVideoError;
            videoPlayer.loopPointReached -= OnVideoEnded;
        }
        ReleaseRenderTexture();
        if (runtimeSkybox != null && ownsRuntimeSkybox)
        {
            Destroy(runtimeSkybox);
        }
    }
}
