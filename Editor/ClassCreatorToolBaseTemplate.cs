using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ClassCreatorToolBaseTemplate : EditorWindow
{
    private string className = "NewClass";
    private List<string> variableNames = new List<string> { "variable1" };
    private List<int> selectedTypeIndices = new List<int> { 0 };
    private string[] variableTypes = new string[] { "int", "float", "string", "bool" };

    private bool classExists = false;

    [MenuItem("Tools/TF-UIPackageTools/Create Class Base Template")]
    public static void ShowWindow()
    {
        GetWindow<ClassCreatorToolBaseTemplate>("Create Class Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Class Tool", EditorStyles.boldLabel);

        className = EditorGUILayout.TextField("Class Name", className);

        if (classExists)
        {
            EditorGUILayout.HelpBox("Class Name already in use", MessageType.Warning);
        }

        
        for (int i = 0; i < variableNames.Count; i++)
        {
            GUILayout.BeginVertical();

            variableNames[i] = EditorGUILayout.TextField("Variable Name", variableNames[i]);
            
            
            selectedTypeIndices[i] = EditorGUILayout.Popup("Variable Type", selectedTypeIndices[i], variableTypes);

            
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                variableNames.RemoveAt(i);
                selectedTypeIndices.RemoveAt(i);
            }

            GUILayout.EndVertical();
        }
        
        if (GUILayout.Button("Add Variable"))
        {
            variableNames.Add("newVariable");
            selectedTypeIndices.Add(0); // Por defecto a 'int'
        }
        
        if (GUILayout.Button("Create Class"))
        {
            if (!CheckIfClassExists(className))
            {
                CreateClassFile();
            }
            else
            {
                classExists = true;
            }
        }
    }

    private bool CheckIfClassExists(string className)
    {
        string folderPath = "Assets/Scripts";
        string filePath = Path.Combine(folderPath, className + ".cs");

        return File.Exists(filePath);
    }

    private void CreateClassFile()
    {
        string folderPath = "Assets/Scripts";
        string fileSenderPath = Path.Combine(folderPath, className + "DataSender.cs");
        string fileReceiverPath = Path.Combine(folderPath, className + "DataReceiver.cs");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        using (StreamWriter writer = new StreamWriter(fileSenderPath, false))
        {
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine("using System;");
            writer.WriteLine();
            writer.WriteLine("public class " + className + "DataSender : MonoBehaviour");
            writer.WriteLine("{");
            writer.Write("    public static Action<");
            for (int i = 0; i < variableNames.Count; i++)
            {
                writer.Write(variableTypes[selectedTypeIndices[i]]);
                if (i < variableNames.Count - 1)
                {
                    writer.Write(",");
                }
            }
            writer.WriteLine("> OnDataNotify;");
            writer.WriteLine();
            writer.Write("    public void NotifyData(");
            for (int i = 0; i < variableNames.Count; i++)
            {
                writer.Write(variableTypes[selectedTypeIndices[i]] + " " + variableNames[i]);
                if (i < variableNames.Count - 1)
                {
                    writer.Write(", ");
                }
            }
            writer.WriteLine(")");
            writer.WriteLine("    {");
            writer.Write("        OnDataNotify?.Invoke(");
            for (int i = 0; i < variableNames.Count; i++)
            {
                writer.Write(variableNames[i]);
                if (i < variableNames.Count - 1)
                {
                    writer.Write(", ");
                }
            }
            writer.WriteLine(");");
            writer.WriteLine("    }");
            writer.WriteLine("}");
        }

        using (StreamWriter writer = new StreamWriter(fileReceiverPath, false))
        {
            writer.WriteLine("using UnityEngine;");
            writer.WriteLine();
            writer.WriteLine("public class " + className + "DataReceiver : HUDElementReceiver");
            writer.WriteLine("{");
            writer.WriteLine("    void Awake()");
            writer.WriteLine("    {");
            writer.WriteLine("          " +className + "DataSender.OnDataNotify += UpdateHUD;");
            writer.WriteLine("    }");
            writer.WriteLine("    private void OnDestroy()");
            writer.WriteLine("    {");
            writer.WriteLine("          " +className + "DataSender.OnDataNotify -= UpdateHUD;");
            writer.WriteLine("    }");
            writer.Write($"    public void UpdateHUD(");
            for (int i = 0; i < variableNames.Count; i++)
            {
                writer.Write(variableTypes[selectedTypeIndices[i]] + " " + variableNames[i]);
                if (i < variableNames.Count - 1)
                {
                    writer.Write(", ");
                }
            }
            writer.WriteLine(")");
            writer.WriteLine("    {");
            writer.WriteLine("        // Implementación de UpdateHUD");
            writer.WriteLine("    }");
            writer.WriteLine("}");

        }
        

        AssetDatabase.Refresh();

        Debug.Log("Class created in: " + folderPath);

        classExists = false; // Restablecer la bandera después de crear la clase con éxito
    }
}