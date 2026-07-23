using UnityEngine;

/// <summary>
/// Monitora colocar/tirar o headset. Ao recolocar depois de
/// restartSeconds sem uso, reinicia a experiência (reescaneia a pasta de
/// vídeos e volta ao início) e registra a duração de cada sessão no log.
/// </summary>
public class HMDController : MonoBehaviour
{
    public BugaVideoPlayer player;

    [Tooltip("Segundos sem o headset para reiniciar ao recolocar. O restartSeconds do config.json tem prioridade.")]
    public float restartThreshold = 3f;

    public bool isCounting;
    public float countingTime;

    float sessionStart = -1f;

    float Threshold => player != null && player.Config != null
        ? player.Config.restartSeconds
        : restartThreshold;

    void Awake()
    {
        if (player == null) player = GetComponent<BugaVideoPlayer>();
        if (player == null) player = FindFirstObjectByType<BugaVideoPlayer>();
        isCounting = false;
        countingTime = 0f;
    }

    void OnEnable()
    {
        OVRManager.HMDMounted += OnHMDMounted;
        OVRManager.HMDUnmounted += OnHMDUnmounted;
    }

    void OnDisable()
    {
        OVRManager.HMDMounted -= OnHMDMounted;
        OVRManager.HMDUnmounted -= OnHMDUnmounted;
    }

    void Update()
    {
        if (isCounting) countingTime += Time.deltaTime;
    }

    void OnHMDMounted()
    {
        UsageLogger.Log("session_start");
        sessionStart = Time.realtimeSinceStartup;

        if (countingTime >= Threshold && player != null)
        {
            player.RestartExperience();
        }

        isCounting = false;
        countingTime = 0f;
    }

    void OnHMDUnmounted()
    {
        isCounting = true;
        if (sessionStart >= 0f)
        {
            float duration = Time.realtimeSinceStartup - sessionStart;
            UsageLogger.Log("session_end", "duration=" + duration.ToString("F1") + "s");
            sessionStart = -1f;
        }
    }
}
