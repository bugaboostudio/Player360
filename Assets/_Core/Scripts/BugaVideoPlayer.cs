using System.IO;
using UnityEngine;
using UnityEngine.Video;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    public VideoType videoType = VideoType.Mono360;

    [Header("Materiais para Skybox")]
    public Material material360Mono;
    public Material material360Stereo;
    public Material material180Mono;
    public Material material180Stereo;

    public VideoPlayer videoPlayer; // Arraste seu VideoPlayer aqui.

    private RenderTexture renderTexture;
    private string videoFolderPath;

    void Start()
    {
        // Define o caminho para a pasta de vídeos.
        videoFolderPath = Path.Combine(Application.persistentDataPath, "360Videos");

        // Certifique-se de que a pasta existe.
        if (!Directory.Exists(videoFolderPath))
        {
            Directory.CreateDirectory(videoFolderPath);
            Debug.Log("Pasta criada em: " + videoFolderPath);
        }

        // Tente carregar o primeiro vídeo encontrado.
        LoadFirstVideoInFolder();
    }

    void LoadFirstVideoInFolder()
    {
        string[] videoFiles = Directory.GetFiles(videoFolderPath, "*.mp4");

        if (videoFiles.Length > 0)
        {
            string videoPath = videoFiles[0];
            Debug.Log("Carregando vídeo: " + videoPath);

            videoPlayer.url = videoPath;
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogWarning("Nenhum vídeo encontrado na pasta: " + videoFolderPath);
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        int videoWidth = (int)vp.texture.width;
        int videoHeight = (int)vp.texture.height;

        renderTexture = new RenderTexture(videoWidth, videoHeight, 0);
        renderTexture.Create();

        videoPlayer.targetTexture = renderTexture;

        Material selectedMaterial = GetSkyboxMaterial();
        if (selectedMaterial != null)
        {
            selectedMaterial.SetTexture("_MainTex", renderTexture);
            RenderSettings.skybox = selectedMaterial;
            Debug.Log("Skybox atualizado com o material: " + selectedMaterial.name);

#if UNITY_EDITOR
            // Força o Editor a atualizar o Skybox no Lighting
            DynamicGI.UpdateEnvironment();
            EditorUtility.SetDirty(RenderSettings.skybox);
#endif
        }

        videoPlayer.Play();
    }

    private Material GetSkyboxMaterial()
    {
        switch (videoType)
        {
            case VideoType.Mono360:
                return material360Mono;
            case VideoType.Stereo360:
                return material360Stereo;
            case VideoType.Mono180:
                return material180Mono;
            case VideoType.Stereo180:
                return material180Stereo;
            default:
                return null;
        }
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}
