using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Android;

public class BugaVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Arraste seu VideoPlayer aqui.
    public Material skyboxMaterial; // Material do Skybox.

    private RenderTexture renderTexture;
    private string videoFolderPath;

    void Start()
    {
        // Verifica e solicita permissões necessárias.
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }

        // Permissão adicional para gerenciamento de armazenamento (Android 12+).
        if (!Permission.HasUserAuthorizedPermission("android.permission.MANAGE_EXTERNAL_STORAGE"))
        {
            Permission.RequestUserPermission("android.permission.MANAGE_EXTERNAL_STORAGE");
        }

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
        // Obtém todos os arquivos de vídeo na pasta.
        string[] videoFiles = Directory.GetFiles(videoFolderPath, "*.mp4"); // Filtra por .mp4

        if (videoFiles.Length > 0)
        {
            string videoPath = videoFiles[0]; // Seleciona o primeiro vídeo.
            Debug.Log("Carregando vídeo: " + videoPath);

            // Configura o VideoPlayer para reproduzir o vídeo.
            videoPlayer.url = videoPath;
            videoPlayer.prepareCompleted += OnVideoPrepared; // Evento disparado quando o vídeo é preparado.
            videoPlayer.Prepare(); // Prepara o vídeo.
        }
        else
        {
            Debug.LogWarning("Nenhum vídeo encontrado na pasta: " + videoFolderPath);
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        int videoWidth = (int)vp.texture.width;  // Obtém a largura do vídeo.
        int videoHeight = (int)vp.texture.height; // Obtém a altura do vídeo.

        // Cria uma nova RenderTexture com a resolução do vídeo.
        renderTexture = new RenderTexture(videoWidth, videoHeight, 0);
        renderTexture.Create();

        // Configura o VideoPlayer para renderizar na RenderTexture.
        videoPlayer.targetTexture = renderTexture;

        // Atribui a RenderTexture ao material do Skybox.
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetTexture("_MainTex", renderTexture);

            // Aplica o Skybox Material ao RenderSettings globalmente.
            RenderSettings.skybox = skyboxMaterial;
            Debug.Log("Skybox atualizado com o material.");
        }

        // Inicia o vídeo.
        videoPlayer.Play();
    }

    void OnDestroy()
    {
        // Libera a RenderTexture quando não for mais necessária.
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}
