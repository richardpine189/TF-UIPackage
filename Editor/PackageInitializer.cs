using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class PackageInitializer
{
    static PackageInitializer()
    {
        EditorApplication.update += OnFirstLoad;
    }

    private static void OnFirstLoad()
    {
        EditorApplication.update -= OnFirstLoad;

        string pathToCheck = "Assets/Resources";
        if (!Directory.Exists(pathToCheck))
        {
            if (EditorUtility.DisplayDialog("Importar Archivos Adicionales",
                    "¿Deseas importar archivos adicionales requeridos por este paquete?", "Sí", "No"))
            {
                ImportAdditionalFiles();
            }
        }
    }

    private static void ImportAdditionalFiles()
    {
        
        string packagePath = "Packages/com.ricardopino.uipackage/Assets";
        string targetPath = "Assets";
        
        FileUtil.CopyFileOrDirectory(packagePath, targetPath);
        
        AssetDatabase.Refresh();

        Debug.Log("Archivos adicionales importados exitosamente.");
    }
}
