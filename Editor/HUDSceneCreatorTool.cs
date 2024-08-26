using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDSceneCreatorTool : EditorWindow
{
    private string sceneName = "NewHUDScene";
    private bool addMainCamera = false;
    private bool addCanvas = false;
    private float canvasWidth = 1080;
    private float canvasHeight = 1920;

    [MenuItem("Tools/TF-UIPackageTools/Scene Creators/Scene Creator Tool")]
    public static void ShowWindow()
    {
        GetWindow<HUDSceneCreatorTool>("HUD Scene Creator Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("HUD Scene Creator Tool", EditorStyles.boldLabel);
        
        sceneName = EditorGUILayout.TextField("Scene Name", sceneName);
        addMainCamera = EditorGUILayout.Toggle("Add Main Camera", addMainCamera);
        addCanvas = EditorGUILayout.Toggle("Add Canvas UI", addCanvas);

        if (addCanvas)
        {
            canvasWidth = EditorGUILayout.FloatField("Canvas Width", canvasWidth);
            canvasHeight = EditorGUILayout.FloatField("Canvas Height", canvasHeight);
        }
        
        if (GUILayout.Button("Create Scene"))
        {
            CreateScene();
        }

        if (GUILayout.Button("Add Scene to Build Settings"))
        {
            AddSceneToBuildSettings();
        }
    }

    private void CreateScene()
    {
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        if (addMainCamera)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }

        if (addCanvas)
        {
            GameObject canvasObject = new GameObject("Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(canvasWidth, canvasHeight);
            canvasObject.AddComponent<GraphicRaycaster>();
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        string scenePath = $"Assets/Scenes/{sceneName}.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        AssetDatabase.Refresh();
    }

    private void AddSceneToBuildSettings()
    {
        string scenePath = $"Assets/Scenes/{sceneName}.unity";

        if (System.Array.Exists(EditorBuildSettings.scenes, scene => scene.path == scenePath))
        {
            Debug.LogWarning("The scene is already in the Build Settings.");
            return;
        }

        EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(scenePath, true);
        var tempScenes = EditorBuildSettings.scenes;
        var scenes = new EditorBuildSettingsScene[tempScenes.Length + 1];
        System.Array.Copy(tempScenes, scenes, tempScenes.Length);
        scenes[scenes.Length - 1] = newScene;

        EditorBuildSettings.scenes = scenes;
        Debug.Log("Scene added to Build Settings.");
    }
}

