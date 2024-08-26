using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class PackageInitializer
{
    static PackageInitializer()
    {
        // Esto se ejecuta cuando se importa el paquete
        EditorApplication.update += OnFirstLoad;
    }

    private static void OnFirstLoad()
    {
        EditorApplication.update -= OnFirstLoad;

        // Verifica si los archivos ya están importados
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
        // Define la ruta a los archivos que quieres copiar
        string packagePath = "Packages/com.ricardopino.uipackage/Assets";
        string targetPath = "Assets";

        // Copia los archivos
        FileUtil.CopyFileOrDirectory(packagePath, targetPath);

        // Refresca el proyecto para que Unity reconozca los nuevos archivos
        AssetDatabase.Refresh();

        Debug.Log("Archivos adicionales importados exitosamente.");
    }
}
