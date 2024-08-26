using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.IO;
using UnityEngine.Events;
using System.Reflection;
using UnityEditor.Events;
using UnityEditor.SceneManagement;

public class MenuSceneTool : EditorWindow
{
    private string sceneName = "NewScene";
    private Sprite backgroundSprite;
    private Sprite containerSprite;
    private int canvasWidth = 1080;
    private int canvasHeight = 1200;
    private GameObject container;
    private int buttonCount = 0;
    private Sprite buttonSprite;
    private string[] buttonTexts;
    private Font buttonFont;
    private bool useHorizontalLayout;
    private bool useVerticalLayout;
    private bool useGridLayout;

    private int containerWidth = 300;
    private int containerHeight = 300;

    private int buttonWidth = 160;
    private int buttonHeight = 30;

    private TextAnchor childAlignment = TextAnchor.MiddleCenter;
    private TextAnchor containerAlignment = TextAnchor.MiddleCenter;

    private string[] availableActions;
    private int[] selectedActionIndices;

    [MenuItem("Tools/TF-UIPackageTools/Scene Creators/Menu Scene Creator Tool")]
    public static void ShowWindow()
    {
        GetWindow<MenuSceneTool>("Menu Scene Tool");
    }

    private void OnEnable()
    {
        string scriptsPath = "Assets/Resources/ButtonSimpleScripts";
        availableActions = Directory.GetFiles(scriptsPath, "*.cs")
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();
        
        selectedActionIndices = new int[buttonCount];
    }

    private void OnGUI()
    {

        sceneName = EditorGUILayout.TextField("Menu Scene Name:", sceneName);

        backgroundSprite = (Sprite)EditorGUILayout.ObjectField("Background Sprite:", backgroundSprite, typeof(Sprite), false);


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Container Settings", EditorStyles.boldLabel);
        containerSprite = (Sprite)EditorGUILayout.ObjectField("Container Sprite:", containerSprite, typeof(Sprite), false);
        containerWidth = EditorGUILayout.IntField("Container Width:", containerWidth);
        containerHeight = EditorGUILayout.IntField("Container Height:", containerHeight);
        containerAlignment = (TextAnchor)EditorGUILayout.EnumPopup("Container Alignment", containerAlignment);


        EditorGUILayout.Space();
        buttonCount = EditorGUILayout.IntField("Number of Buttons:", buttonCount);

        // Ajuste para retener los textos de los botones al cambiar entre campos
        if (buttonTexts == null || buttonTexts.Length != buttonCount)
        {
            string[] newButtonTexts = new string[buttonCount];
            int[] newSelectedActionIndices = new int[buttonCount];
            for (int i = 0; i < buttonCount && i < buttonTexts.Length; i++)
            {
                newButtonTexts[i] = buttonTexts[i];
                newSelectedActionIndices[i] = selectedActionIndices[i];
            }
            buttonTexts = newButtonTexts;
            selectedActionIndices = newSelectedActionIndices;
        }

        for (int i = 0; i < buttonCount; i++)
        {
            buttonTexts[i] = EditorGUILayout.TextField($"Button {i + 1} Text:", buttonTexts[i]);
            
            selectedActionIndices[i] = EditorGUILayout.Popup("Button Action:", selectedActionIndices[i], availableActions);
        }

        buttonSprite = (Sprite)EditorGUILayout.ObjectField("Button Sprite:", buttonSprite, typeof(Sprite), false);
        buttonFont = (Font)EditorGUILayout.ObjectField("Button Font:", buttonFont, typeof(Font), false);
        buttonWidth = EditorGUILayout.IntField("Button Width:", buttonWidth);
        buttonHeight = EditorGUILayout.IntField("Button Height:", buttonHeight);

        //TODO: LLevarlo a metodo: Alineación del contenedor con respecto a sus hijos
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Container Child Alignment", EditorStyles.boldLabel);
        childAlignment = (TextAnchor)EditorGUILayout.EnumPopup("Child Alignment", childAlignment);

        useHorizontalLayout = EditorGUILayout.Toggle("Horizontal Layout", useHorizontalLayout);
        useVerticalLayout = EditorGUILayout.Toggle("Vertical Layout", useVerticalLayout);
        useGridLayout = EditorGUILayout.Toggle("Grid Layout", useGridLayout);

        //TODO: LLevarlo a metodo: Botón para generar la escena
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Scene"))
        {
            GenerateScene();
        }

        
        if (GUILayout.Button("Add Scene to Build Settings"))
        {
            AddSceneToBuildSettings();
        }
    }

    private void GenerateScene()
    {
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var scene = SceneManager.GetActiveScene();
        
        var cameraGO = new GameObject("Main Camera");
        var camera = cameraGO.AddComponent<Camera>();
        camera.tag = "MainCamera";
        
        var eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();
        
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();
        var canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(canvasWidth, canvasHeight); // TODO: Cambiar y modificar el Reference Resolution del Scaler

        //TODO: LLevarlo a metodo: Añadir Background
        if (backgroundSprite != null)
        {
            var backgroundGO = new GameObject("Background");
            var backgroundImage = backgroundGO.AddComponent<Image>();
            backgroundImage.sprite = backgroundSprite;
            backgroundGO.transform.SetParent(canvasGO.transform, false);
            var bgRect = backgroundGO.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(1, 1);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bgRect.pivot = new Vector2(0.5f, 0.5f);
        }

        //TODO: LLevarlo a metodo: Crear el Container
        container = new GameObject("Container");
        var containerRect = container.AddComponent<RectTransform>();
        container.transform.SetParent(canvasGO.transform, false);
        containerRect.sizeDelta = new Vector2(containerWidth, containerHeight);

        AlignRectTransform(containerRect, containerAlignment);

        if (containerSprite != null)
        {
            var containerImage = container.AddComponent<Image>();
            containerImage.sprite = containerSprite;
        }

        //TODO: LLevarlo a metodo: Asignar Layout al Container
        if (useHorizontalLayout)
        {
            var layoutGroup = container.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childAlignment = childAlignment;
        }
        else if (useVerticalLayout)
        {
            var layoutGroup = container.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childAlignment = childAlignment;
        }
        else if (useGridLayout)
        {
            var layoutGroup = container.AddComponent<GridLayoutGroup>();
            layoutGroup.childAlignment = childAlignment;
        }

        //TODO: LLevarlo a metodo: Crear los botones
        for (int i = 0; i < buttonCount; i++)
        {
            var buttonGO = new GameObject($"Button {i + 1}");
            var buttonRect = buttonGO.AddComponent<RectTransform>();
            buttonGO.transform.SetParent(container.transform, false);
            buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

            var button = buttonGO.AddComponent<Button>();
            if (buttonSprite != null)
            {
                var buttonImage = buttonGO.AddComponent<Image>();
                buttonImage.sprite = buttonSprite;
            }

            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            var text = textGO.AddComponent<Text>();
            text.text = buttonTexts[i];
            text.alignment = TextAnchor.MiddleCenter;
            text.font = buttonFont != null ? buttonFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            text.rectTransform.sizeDelta = buttonRect.sizeDelta;

            //TODO: LLevarlo a metodo: Agregar el script seleccionado al botón como componente
            string scriptName = availableActions[selectedActionIndices[i]];
            string fullScriptPath = $"Assets/Resources/ButtonSimpleScripts/{scriptName}.cs";

            var assembly = Assembly.Load("Assembly-CSharp");
            var scriptType = assembly.GetType(scriptName);
            if (scriptType != null)
            {
                var action = buttonGO.AddComponent(scriptType);

                //TODO: LLevarlo a metodo: Configurar el OnClick() del botón para que llame al método Execute del script
                var buttonComponent = buttonGO.GetComponent<Button>();
                MethodInfo methodInfo = scriptType.GetMethod("Execute");
                if (methodInfo != null)
                {
                    UnityAction actionDelegate = Delegate.CreateDelegate(typeof(UnityAction), action, methodInfo) as UnityAction;
                    if (actionDelegate != null)
                    {
                        UnityEventTools.AddPersistentListener(buttonComponent.onClick, actionDelegate);
                    }
                }
            }
        }

        EditorSceneManager.SaveScene(scene, $"Assets/Scenes/{sceneName}.unity");
    }

    private void AlignRectTransform(RectTransform rectTransform, TextAnchor alignment)
    {
        switch (alignment)
        {
            case TextAnchor.UpperLeft:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                break;
            case TextAnchor.UpperCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 1);
                rectTransform.anchorMax = new Vector2(0.5f, 1);
                rectTransform.pivot = new Vector2(0.5f, 1);
                break;
            case TextAnchor.UpperRight:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(1, 1);
                break;
            case TextAnchor.MiddleLeft:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(0, 0.5f);
                rectTransform.pivot = new Vector2(0, 0.5f);
                break;
            case TextAnchor.MiddleCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                break;
            case TextAnchor.MiddleRight:
                rectTransform.anchorMin = new Vector2(1, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.pivot = new Vector2(1, 0.5f);
                break;
            case TextAnchor.LowerLeft:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0);
                break;
            case TextAnchor.LowerCenter:
                rectTransform.anchorMin = new Vector2(0.5f, 0);
                rectTransform.anchorMax = new Vector2(0.5f, 0);
                rectTransform.pivot = new Vector2(0.5f, 0);
                break;
            case TextAnchor.LowerRight:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(1, 0);
                break;
        }

        rectTransform.anchoredPosition = Vector2.zero;
    }

    private void AddSceneToBuildSettings()
    {
        string scenePath = $"Assets/Scenes/{sceneName}.unity";
        EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
        EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[original.Length + 1];

        newSettings[0] = new EditorBuildSettingsScene(scenePath, true);

        for (int i = 0; i < original.Length; i++)
        {
            newSettings[i + 1] = original[i];
        }

        EditorBuildSettings.scenes = newSettings;
    }
}