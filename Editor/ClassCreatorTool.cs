using UnityEditor;
using UnityEngine;
using System.IO;

namespace com.ricardopino.uipackage
{
    public class ClassCreatorTool : EditorWindow
    {
        private string className = "NewClass";
        private string variableName = "variable";
        private string selectedType = "int";
        private string folderPath;

        private string[] variableTypes = { "int", "float", "string" };

        [MenuItem("Tools/TF-UIPackageTool/Create Class")]
        public static void ShowWindow()
        {
            GetWindow<ClassCreatorTool>("Create Class Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create Class Tool", EditorStyles.boldLabel);

            className = EditorGUILayout.TextField("Class Name", className);

            selectedType =
                variableTypes[
                    EditorGUILayout.Popup("Variable Type", System.Array.IndexOf(variableTypes, selectedType),
                        variableTypes)];

            variableName = EditorGUILayout.TextField("Variable Name", variableName);

            if (GUILayout.Button("Create Class"))
            {
                CreateFolder();
                CreateSenderClass();
                CreateReceiverTextClass();
                CreateReceiverImageClass();
                CreateInstructiveFile();
                AssetDatabase.Refresh();
            }
        }

        private void CreateFolder()
        {
            folderPath = "Assets/Scripts/" + className + "Component";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            Debug.Log("Folder Created");
        }

        private void CreateInstructiveFile()
        {
            string filePath = Path.Combine(folderPath, "instructions.md");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(
                    $"1) Coloca el componente {className}Sender.cs dentro del GameObject que controla tu dato a mostrar, luego invoca Notify cuando sea necesario.");
                writer.WriteLine(
                    $"2) Integra el Prefab de HUD Element en la escena que maneja tu HUD dentro del canvas y configura su posicion.");
            }

            Debug.Log("Clase creada en: " + filePath);
        }

        

        private void CreateSenderClass()
        {
            string filePath = Path.Combine(folderPath, className + "Sender.cs");

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("public class " + className + "Sender : MonoBehaviour");
                writer.WriteLine("{");
                writer.WriteLine("    public Action<" + selectedType + "> OnDataNotify;");
                writer.WriteLine();
                writer.WriteLine("    public void NotifyData(" + selectedType + " data)");
                writer.WriteLine("    {");
                writer.WriteLine("        OnDataNotify?.Invoke(data);");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Debug.Log("Clase creada en: " + filePath);
        }
        private void CreateReceiverImageClass()
        {
            string filePath = Path.Combine(folderPath, className + "ImageReceiver.cs");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine();
                writer.WriteLine("public class " + className + "ImageReceiver : MonoBehaviour");
                writer.WriteLine("{");
                writer.WriteLine("    public " + selectedType + " " + variableName + ";");
                writer.WriteLine();
                writer.WriteLine("    public void UpdateHUD(" + selectedType + " obj)");
                writer.WriteLine("    {");
                writer.WriteLine("        // Implementación de UpdateHUD");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Debug.Log("Clase creada en: " + filePath);
        }
        private void CreateReceiverTextClass()
        {
            string filePath = Path.Combine(folderPath, className + "TextReceiver.cs");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using UnityEngine.UI;");
                writer.WriteLine("public class " + className + "TextReceiver : MonoBehaviour");
                writer.WriteLine("{");
                writer.WriteLine("    [SerializeField] private Text " + className + "Text;");
                writer.WriteLine("");
                writer.WriteLine("    void Awake()");
                writer.WriteLine("    {");
                writer.WriteLine("          LifeDataSender.OnDataNotify += UpdateHUD;");
                writer.WriteLine("    }");
                writer.WriteLine("    private void OnDestroy()");
                writer.WriteLine("    {");
                writer.WriteLine("          LifeDataSender.OnDataNotify -= UpdateHUD;");
                writer.WriteLine("    }");
                writer.WriteLine("    public void UpdateHUD(" + selectedType + " obj)");
                writer.WriteLine("    {");
                writer.WriteLine("        " + className + "Text.text = obj.ToString();");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Debug.Log("Clase creada en: " + filePath);
        }
    }
}
