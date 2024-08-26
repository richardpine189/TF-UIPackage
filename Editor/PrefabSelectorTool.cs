using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PrefabSelectorTool : EditorWindow
{
    private const string prefabsFolderPath = "Assets/Resources/Prefabs/HUDElement"; 
    private const string scenesFolderPath = "Assets/Scenes";

    private GameObject[] prefabs;
    private string[] prefabNames;
    private int selectedPrefabIndex = 0;

    private string[] scenePaths;
    private string[] sceneNames;
    private int selectedSceneIndex = 0;

    [MenuItem("Tools/TF-UIPackageTools/Add HUD-Element Prefab Template/Prefab Selector Tool")]
    public static void ShowWindow()
    {
        GetWindow<PrefabSelectorTool>("Prefab Selector Tool");
    }

    private void OnEnable()
    {
        LoadPrefabs();
        LoadScenes();
    }

    private void LoadPrefabs()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsFolderPath });
        prefabs = new GameObject[guids.Length];
        prefabNames = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            prefabNames[i] = prefabs[i].name;
        }
    }

    private void LoadScenes()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { scenesFolderPath });
        scenePaths = new string[guids.Length];
        sceneNames = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            scenePaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenePaths[i]);
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Select a Prefab to Instantiate", EditorStyles.boldLabel);
        selectedPrefabIndex = EditorGUILayout.Popup("Prefab", selectedPrefabIndex, prefabNames);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Select a Scene", EditorStyles.boldLabel);
        selectedSceneIndex = EditorGUILayout.Popup("Scene", selectedSceneIndex, sceneNames);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Prefab in Scene"))
        {
            CreatePrefabInScene();
        }
    }

    private void CreatePrefabInScene()
    {
        if (prefabs == null || prefabs.Length == 0 || scenePaths == null || scenePaths.Length == 0)
        {
            Debug.LogWarning("No prefabs or scenes found in the specified folders.");
            return;
        }

        GameObject prefab = prefabs[selectedPrefabIndex];
        string scenePath = scenePaths[selectedSceneIndex];

        if (prefab != null && !string.IsNullOrEmpty(scenePath))
        {
            
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            SceneManager.MoveGameObjectToScene(instance, scene);
            instance.transform.position = Vector3.zero;
            instance.gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log($"{prefab.name} created in {scene.name}.");
        }
    }
}