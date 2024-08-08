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

        private string[] variableTypes = new string[] { "int", "float", "string" };

        [MenuItem("Tools/TF-UIPackageTool/Create Class")]
        public static void ShowWindow()
        {
            GetWindow<ClassCreatorTool>("Create Class Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create Class Tool", EditorStyles.boldLabel);

            className = EditorGUILayout.TextField("Class Name", className);

            selectedType = variableTypes[EditorGUILayout.Popup("Variable Type", System.Array.IndexOf(variableTypes, selectedType), variableTypes)];
            
            variableName = EditorGUILayout.TextField("Variable Name", variableName);

            if (GUILayout.Button("Create Class"))
            {
                CreateClassFile();
            }
        }

        private void CreateClassFile()
        {
            string folderPath = "Assets/Scripts";
            string filePath = Path.Combine(folderPath, className + ".cs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine();
                writer.WriteLine("public class " + className + " : MonoBehaviour, IHUDElements");
                writer.WriteLine("{");
                writer.WriteLine("    public " + selectedType + " " + variableName + ";");
                writer.WriteLine();
                writer.WriteLine("    public void UpdateHUD()");
                writer.WriteLine("    {");
                writer.WriteLine("        // Implementación de UpdateHUD");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();

            Debug.Log("Clase creada en: " + filePath);
        }
    }
}
