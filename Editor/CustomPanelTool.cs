using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Reflection;

public class CustomPanelTool : EditorWindow
{
    private string panelName = "NewPanel";
    private Sprite panelBackground;
    private float panelWidth = 500f;
    private float panelHeight = 300f;
    private Sprite containerSprite;
    private TextAnchor containerAlignment = TextAnchor.MiddleCenter;
    private bool useHorizontalLayout = false;
    private bool useVerticalLayout = false;
    private bool useGridLayout = false;
    
    private Sprite buttonSprite;
    private Font buttonFont;
    private TextAnchor buttonTextAlignment = TextAnchor.MiddleCenter;
    private int buttonCount = 1;
    private string[] buttonTexts;
    private string[] buttonScripts;

    [MenuItem("Tools/Custom Panel Tool")]
    public static void ShowWindow()
    {
        GetWindow<CustomPanelTool>("Custom Panel Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("Panel Settings", EditorStyles.boldLabel);
        panelName = EditorGUILayout.TextField("Panel Name", panelName);
        panelBackground = (Sprite)EditorGUILayout.ObjectField("Panel Background", panelBackground, typeof(Sprite), false);
        panelWidth = EditorGUILayout.FloatField("Panel Width", panelWidth);
        panelHeight = EditorGUILayout.FloatField("Panel Height", panelHeight);

        GUILayout.Space(10);
        GUILayout.Label("Container Settings", EditorStyles.boldLabel);
        containerSprite = (Sprite)EditorGUILayout.ObjectField("Container Sprite", containerSprite, typeof(Sprite), false);
        containerAlignment = (TextAnchor)EditorGUILayout.EnumPopup("Container Alignment", containerAlignment);

        GUILayout.Space(10);
        GUILayout.Label("Container Layout Settings", EditorStyles.boldLabel);
        useHorizontalLayout = EditorGUILayout.Toggle("Use Horizontal Layout", useHorizontalLayout);
        useVerticalLayout = EditorGUILayout.Toggle("Use Vertical Layout", useVerticalLayout);
        useGridLayout = EditorGUILayout.Toggle("Use Grid Layout", useGridLayout);

        GUILayout.Space(10);
        GUILayout.Label("Button Settings", EditorStyles.boldLabel);
        buttonSprite = (Sprite)EditorGUILayout.ObjectField("Button Sprite", buttonSprite, typeof(Sprite), false);
        buttonFont = (Font)EditorGUILayout.ObjectField("Button Font", buttonFont, typeof(Font), false);
        buttonTextAlignment = (TextAnchor)EditorGUILayout.EnumPopup("Button Text Alignment", buttonTextAlignment);
        buttonCount = EditorGUILayout.IntField("Button Count", buttonCount);

        if (buttonTexts == null || buttonTexts.Length != buttonCount)
        {
            buttonTexts = new string[buttonCount];
            buttonScripts = new string[buttonCount];
        }

        for (int i = 0; i < buttonCount; i++)
        {
            GUILayout.Label($"Button {i + 1} Settings", EditorStyles.boldLabel);
            buttonTexts[i] = EditorGUILayout.TextField("Button Text", buttonTexts[i]);
            buttonScripts[i] = DrawScriptDropdown($"Button Script", buttonScripts[i]);
        }

        if (GUILayout.Button("Create Panel"))
        {
            CreatePanel();
        }
    }

    string DrawScriptDropdown(string label, string selectedScript)
    {
        string[] scripts = GetScriptsFromFolder("Assets/Resources/ButtonSimpleScripts");
        int selectedIndex = Array.IndexOf(scripts, selectedScript);
        selectedIndex = EditorGUILayout.Popup(label, selectedIndex, scripts);
        return selectedIndex >= 0 ? scripts[selectedIndex] : null;
    }

    void CreatePanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        
        GameObject panel = new GameObject(panelName);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.SetParent(canvas.transform, false);
        panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.sprite = panelBackground;
        panelImage.type = Image.Type.Sliced; //TODO: Set Image mode to Sliced, check on others tools 

        
        GameObject container = new GameObject("Container");
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.SetParent(panel.transform, false);
        containerRect.sizeDelta = new Vector2(panelWidth - 20, panelHeight - 20);
        containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.anchoredPosition = Vector2.zero;

        Image containerImage = container.AddComponent<Image>();
        containerImage.sprite = containerSprite;
        containerImage.type = Image.Type.Sliced; 

       
        if (useHorizontalLayout)
        {
            HorizontalLayoutGroup hlg = container.AddComponent<HorizontalLayoutGroup>();
            ApplyAlignment(hlg);
        }
        else if (useVerticalLayout)
        {
            VerticalLayoutGroup vlg = container.AddComponent<VerticalLayoutGroup>();
            ApplyAlignment(vlg);
        }
        else if (useGridLayout)
        {
            container.AddComponent<GridLayoutGroup>(); //TODO: Check on this Grid layout doesn't use alignment in the same way
        }

        // Create buttons
        for (int i = 0; i < buttonCount; i++)
        {
            CreateButton(container.transform, buttonTexts[i], buttonScripts[i]);
        }
    }

    void CreateButton(Transform parent, string buttonText, string buttonScript)
    {
        GameObject buttonGO = new GameObject("Button");
        RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.SetParent(parent, false);

        Button button = buttonGO.AddComponent<Button>();

        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.sprite = buttonSprite;

        GameObject textGO = new GameObject("Text");
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.SetParent(buttonGO.transform, false);

        Text text = textGO.AddComponent<Text>();
        text.text = buttonText;
        text.font = buttonFont;
        text.alignment = buttonTextAlignment;
        text.color = Color.black;

        
        if (!string.IsNullOrEmpty(buttonScript))
        {
            Type scriptType = Type.GetType(buttonScript);
            if (scriptType != null)
            {
                buttonGO.AddComponent(scriptType);
                MethodInfo executeMethod = scriptType.GetMethod("Execute");
                if (executeMethod != null)
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, () =>
                    {
                        executeMethod.Invoke(buttonGO.GetComponent(scriptType), null);
                    });
                }
            }
        }
    }

    void ApplyAlignment(HorizontalOrVerticalLayoutGroup layoutGroup)
    {
        layoutGroup.childAlignment = containerAlignment;
    }

    string[] GetScriptsFromFolder(string folderPath)
    {
        return AssetDatabase.FindAssets("t:Script", new[] { folderPath })
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
            .Where(script => script != null && script.GetClass() != null)
            .Select(script => script.GetClass().FullName)
            .ToArray();
    }
}

