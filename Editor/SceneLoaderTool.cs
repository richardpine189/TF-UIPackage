using UnityEditor;
using UnityEngine;

public class SceneLoaderTool : EditorWindow
{
    [MenuItem("Tools/TF-UIPackageTools/AddAditiveSceneLoader")]

    private static void CreateGameObjectInScene()
    {
        if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            AddSceneLoaderGameObject();
        }
        else
        {
            Debug.LogWarning("No se puede crear un GameObject en modo Play.");
        }
    }
    

    private static void AddSceneLoaderGameObject()
    {
        GameObject newGameObject = new GameObject("SceneLoaderGameObject");
        newGameObject.AddComponent<SceneLoader>();
        Selection.activeGameObject = newGameObject;
        EditorGUIUtility.PingObject(newGameObject);
    }
}