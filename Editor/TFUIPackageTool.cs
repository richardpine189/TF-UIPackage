using UnityEditor;
using UnityEngine;

public class TFUIPackageTool : EditorWindow
{
    [MenuItem("Tools/TF-UIPackageTool/Options")]
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
        if (GUILayout.Button("Add HUD Scene"))
        {
            //AddSceneLoaderGameObject();
        }
        if (GUILayout.Button("Add HUD Elements"))
        {
            //AddSceneLoaderGameObject();
        }
        if (GUILayout.Button("Create Architecture Connection"))
        {
            //AddSceneLoaderGameObject();
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