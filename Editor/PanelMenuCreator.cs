using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class PanelMenuCreator : EditorWindow
{
    private Texture2D backgroundTexture;
    private Texture2D buttonTexture;
    private int panelWidth = 500;
    private int panelHeight = 300;
    private List<string> buttonTexts = new List<string>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Panel Menu Creator")]
    public static void ShowWindow()
    {
        GetWindow<PanelMenuCreator>("Panel Menu Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Crear Panel/Menu", EditorStyles.boldLabel);

        // Slot para la imagen de fondo
        backgroundTexture = (Texture2D)EditorGUILayout.ObjectField("Fondo del Panel", backgroundTexture, typeof(Texture2D), false);

        // Slot para la imagen del botón
        buttonTexture = (Texture2D)EditorGUILayout.ObjectField("Imagen del Botón", buttonTexture, typeof(Texture2D), false);

        // Campos para el ancho y alto del panel
        panelWidth = EditorGUILayout.IntField("Ancho del Panel", panelWidth);
        panelHeight = EditorGUILayout.IntField("Alto del Panel", panelHeight);

        // Lista de botones
        GUILayout.Label("Botones", EditorStyles.boldLabel);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < buttonTexts.Count; i++)
        {
            GUILayout.BeginHorizontal();
            buttonTexts[i] = EditorGUILayout.TextField($"Texto del Botón {i + 1}", buttonTexts[i]);

            if (GUILayout.Button("Eliminar"))
            {
                buttonTexts.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("Agregar Botón"))
        {
            buttonTexts.Add("Nuevo Botón");
        }

        // Botón para generar el Prefab
        if (GUILayout.Button("Generar Prefab"))
        {
            CreatePrefab();
        }
    }

    private void CreatePrefab()
    {
        // Crear un canvas si no existe
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Crear el GameObject principal del panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObject.transform);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);
        panel.AddComponent<CanvasRenderer>();
        Image panelImage = panel.AddComponent<Image>();
        if (backgroundTexture != null)
        {
            panelImage.sprite = Sprite.Create(backgroundTexture, new Rect(0, 0, backgroundTexture.width, backgroundTexture.height), new Vector2(0.5f, 0.5f));
        }

        // Crear los botones
        for (int i = 0; i < buttonTexts.Count; i++)
        {
            GameObject button = new GameObject($"Button_{i + 1}");
            button.transform.SetParent(panel.transform);
            RectTransform buttonRect = button.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(200, 50); // Tamaño ajustable
            buttonRect.anchoredPosition = new Vector2(0, -(i * 60)); // Posición ajustable

            button.AddComponent<CanvasRenderer>();
            Image buttonImage = button.AddComponent<Image>();
            if (buttonTexture != null)
            {
                buttonImage.sprite = Sprite.Create(buttonTexture, new Rect(0, 0, buttonTexture.width, buttonTexture.height), new Vector2(0.5f, 0.5f));
            }

            Button btnComponent = button.AddComponent<Button>();
            GameObject buttonTextObject = new GameObject("Text");
            buttonTextObject.transform.SetParent(button.transform);
            Text buttonText = buttonTextObject.AddComponent<Text>();
            buttonText.text = buttonTexts[i];
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.rectTransform.sizeDelta = buttonRect.sizeDelta;
            buttonText.color = Color.black; // Color de texto ajustable
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Fuente por defecto
        }

        // Crear la ruta si no existe
        string folderPath = "Assets/Prefabs";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Guardar el Prefab
        string path = $"{folderPath}/Panel.prefab";
        PrefabUtility.SaveAsPrefabAsset(canvasObject, path);
        DestroyImmediate(canvasObject);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}