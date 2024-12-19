using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateDirectoryStructure
{
    [MenuItem("Assets/Create/Directory Structure")]
    public static void CreateDirectories()
    {
        string rootPath = "Assets/_Core";
        
        CreateDirectory(rootPath, "Scenes");
        CreateDirectory(rootPath, "Scripts");
        CreateDirectory(rootPath, "Materials");
        CreateDirectory(rootPath, "Textures");
        CreateDirectory(rootPath, "Models");
        CreateDirectory(rootPath, "Animations");
        CreateDirectory(rootPath, "Audio");
        CreateDirectory(rootPath, "Prefabs");
        CreateDirectory(rootPath, "Plugins");
        CreateDirectory(rootPath, "Editor");
        CreateDirectory(rootPath, "Resources");
        CreateDirectory(rootPath, "Fonts");
        CreateDirectory(rootPath, "ThirdParty");
        CreateDirectory(rootPath, "UI");
        CreateDirectory(rootPath, "Settings");

        AssetDatabase.Refresh();
    }

    private static void CreateDirectory(string rootPath, string name)
    {
        string dir = Path.Combine(rootPath, name);
        if (!AssetDatabase.IsValidFolder(dir))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(dir), Path.GetFileName(dir));
        }
    }
}
