using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Registro simples de uso em usage_log.txt, dentro da própria pasta
/// 360Videos — o operador lê pelo mesmo USB usado para trocar o vídeo.
/// Formato de cada linha: data hora;evento;detalhes
/// </summary>
public static class UsageLogger
{
    static string logPath;

    public static void Init(string folder)
    {
        logPath = Path.Combine(folder, "usage_log.txt");
    }

    public static void Log(string eventName, string details = null)
    {
        if (string.IsNullOrEmpty(logPath)) return;
        try
        {
            string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ";" + eventName;
            if (!string.IsNullOrEmpty(details)) line += ";" + details;
            File.AppendAllText(logPath, line + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogWarning("UsageLogger: " + e.Message);
        }
    }
}
