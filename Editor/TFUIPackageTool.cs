using UnityEditor;
using UnityEngine;

public class TFUIPackageTool : EditorWindow
{
    [MenuItem("Tools/TF-UIPackageTool")]
    public static void ShowWindow()
    {
        GetWindow<TFUIPackageTool>("TF-UIPackageTool");
    }

    private void OnGUI()
    {
        GUILayout.Label("TF-UIPackage Tool", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Add SceneLoader GameObject"))
        {
            AddSceneLoaderGameObject();
        }
    }

    private void AddSceneLoaderGameObject()
    {
        GameObject newGameObject = new GameObject("SceneLoaderGameObject");
        newGameObject.AddComponent<SceneLoader>();
        Selection.activeGameObject = newGameObject;
        EditorGUIUtility.PingObject(newGameObject);
    }
}