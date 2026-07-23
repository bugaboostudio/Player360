using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Configuração opcional lida do arquivo config.json dentro da pasta 360Videos.
/// Se o arquivo não existir, um modelo com os valores padrão é criado
/// automaticamente, para o operador editar direto pelo USB — sem rebuild do app.
/// </summary>
[Serializable]
public class PlayerConfig
{
    public const string FileName = "config.json";

    // Volume do áudio, de 0 a 1.
    public float volume = 1f;

    // Se true, a reprodução recomeça do início ao chegar no fim
    // (vídeo único em loop, ou playlist voltando ao primeiro item).
    public bool loop = true;

    // Segundos sem o headset na cabeça para reiniciar a experiência
    // quando o usuário o recoloca.
    public float restartSeconds = 3f;

    // Duração do fade de entrada/saída, em segundos.
    public float fadeSeconds = 0.5f;

    // Força o formato de projeção: Mono360, Stereo360, Mono180 ou Stereo180.
    // Vazio = detecção automática pelo nome do arquivo.
    public string videoType = "";

    public static PlayerConfig LoadOrCreate(string folder)
    {
        string path = Path.Combine(folder, FileName);
        try
        {
            if (File.Exists(path))
            {
                PlayerConfig config = JsonUtility.FromJson<PlayerConfig>(File.ReadAllText(path));
                if (config != null) return config;
            }
            else
            {
                File.WriteAllText(path, JsonUtility.ToJson(new PlayerConfig(), true));
                Debug.Log("PlayerConfig: modelo criado em " + path);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("PlayerConfig: usando valores padrão (" + e.Message + ")");
        }
        return new PlayerConfig();
    }

    public bool TryGetVideoTypeOverride(out BugaVideoPlayer.VideoType type)
    {
        type = default;
        return !string.IsNullOrWhiteSpace(videoType) && Enum.TryParse(videoType, true, out type);
    }
}
