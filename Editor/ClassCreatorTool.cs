using UnityEditor;
using UnityEngine;
using System.IO;

namespace com.ricardopino.uipackage
{

    public class ClassCreatorTool : EditorWindow
    {
        private string className = "NewClass";
        private string variableName = "Variable Name";
        private string selectedType = "int";
        private string selectedTemplateType = "Text";
        private string folderPath;
        private string[] _scriptTemplateTypes = { "Text", "Image AddAndRemove"};

        private string[] variableTypes = { "int", "float", "string" };

        [MenuItem("Tools/TF-UIPackageTools/Add HUD-Element Script Template/Create Class Single Variable")]
        public static void ShowWindow()
        {
            GetWindow<ClassCreatorTool>("Create Class Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create Class", EditorStyles.boldLabel);

            className = EditorGUILayout.TextField("Class Name", className);

            selectedType =
                    variableTypes[
                        EditorGUILayout.Popup("Variable Type", System.Array.IndexOf(variableTypes, selectedType),
                            variableTypes)];
            
            variableName = EditorGUILayout.TextField("Variable Name", variableName);
            
            selectedTemplateType = _scriptTemplateTypes[
                EditorGUILayout.Popup("Script Template", System.Array.IndexOf(_scriptTemplateTypes, selectedTemplateType),
                    _scriptTemplateTypes)];

            

            if (GUILayout.Button("Create Class"))
            {
                CreateFolder();
                CreateSenderClass();
                
                if(selectedTemplateType == "Text")
                    CreateReceiverTextClass();
                else
                    CreateReceiverImageClass();
                
                CreateInstructiveFile();
                AssetDatabase.Refresh();
            }
        }

        private void CreateFolder()
        {
            folderPath = "Assets/Scripts/HUDComponents/" + className + "Component";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            Debug.Log("Folder Created");
        }

        

        private void CreateSenderClass()
        {
            string senderClassName = className + "DataSender";
            string filePath = Path.Combine(folderPath, senderClassName + ".cs");

            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("public class " + senderClassName + " : MonoBehaviour");
                writer.WriteLine("{");
                writer.WriteLine("    public static Action<" + selectedType + "> OnDataNotify;");
                writer.WriteLine();
                writer.WriteLine("    public void NotifyData(" + selectedType + " dataToSend)");
                writer.WriteLine("    {");
                writer.WriteLine("        OnDataNotify?.Invoke(dataToSend);");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Debug.Log("The Sender Class was created in: " + filePath);
        }
        private void CreateReceiverImageClass()
        {
            string receiverClassName = className + "ImageReceiver";
            string filePath = Path.Combine(folderPath, receiverClassName+ ".cs");
#region ReceiverImageTemplate
            string content = $@"

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class {receiverClassName} : HUDElementReceiver
{{
    [SerializeField] private GameObject _prefab;
    private List<GameObject> _gameObjectsToHandle = new();
    private bool _isSetUp;
    private int count;
    void Awake()
    {{
        {className}DataSender.OnDataNotify += UpdateHUD;
    }}
    private void OnDestroy()
    {{
        {className}DataSender.OnDataNotify -= UpdateHUD;
    }}
    private void UpdateHUD(int data)
    {{
        
        if (!_isSetUp)
        {{
            for (int i = 0; i < data; i++)
            {{
                _gameObjectsToHandle.Add(Instantiate(_prefab, gameObject.transform));
            }}
            count = data;
            _isSetUp = true;
        }}
        else
        {{
            if (count < data)
                _gameObjectsToHandle.Add(Instantiate(_prefab, gameObject.transform));
            else if (count > data)
            {{
                Destroy(_gameObjectsToHandle.Last());
                _gameObjectsToHandle.Remove(_gameObjectsToHandle.Last());
            }}
            count = data;
        }}
    }}

}}
            ";
#endregion
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.Write(content);
            }

            Debug.Log("The Receiver Class was created in: " + filePath);
        }
        private void CreateReceiverTextClass()
        {
            string filePath = Path.Combine(folderPath, className + "TextReceiver.cs");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using UnityEngine.UI;");
                writer.WriteLine("public class " + className + "TextReceiver : HUDElementReceiver");
                writer.WriteLine("{");
                writer.WriteLine("    [SerializeField] private Text " + className + "Text;");
                writer.WriteLine("");
                writer.WriteLine("    void Awake()");
                writer.WriteLine("    {");
                writer.WriteLine("          " +className + "DataSender.OnDataNotify += UpdateHUD;");
                writer.WriteLine("    }");
                writer.WriteLine("    private void OnDestroy()");
                writer.WriteLine("    {");
                writer.WriteLine("          " +className + "DataSender.OnDataNotify -= UpdateHUD;");
                writer.WriteLine("    }");
                writer.WriteLine("    public void UpdateHUD(" + selectedType + " data)");
                writer.WriteLine("    {");
                writer.WriteLine("        " + className + "Text.text = data.ToString();");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }
    
            Debug.Log("The Receiver class was created in: " + filePath);
            
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
                writer.WriteLine(
                    $"3) Agrega el componente {className}Receiver.cs en el formato seleccionado dentro del GameObject que representa el HUDElementReceiver y ajusta sus dependencias.");
                writer.WriteLine(
                    $"4) En el caso del template de Remove / Add image, ajuste la dependencia faltante con el Prefab Image dentro de subResources.");
            }

            Debug.Log("Clase creada en: " + filePath);
        }
    }
    
}
