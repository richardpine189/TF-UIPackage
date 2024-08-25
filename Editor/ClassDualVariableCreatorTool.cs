using UnityEditor;
using UnityEngine;
using System.IO;

namespace com.ricardopino.uipackage
{

    public class ClassDualVariableCreatorTool : EditorWindow
    {
        private string _className = "NewClass";
        private string _variableNameNumericOne = "VariableName";
        private string _variableNameNumericTwo = "VariableName";
        private string _variableNameNoNumeric = "NoNumericVariableName";
        private string _selectedTypeNumericOne = "int";
        private string _selectedTypeNumericTwo = "int";
        private string _selectedTypeNoNumeric = "string";
        private string _selectedTemplateType = "ImageFiller";
        private string _folderPath;
        private string[] _scriptTemplateTypes = { "ImageFiller", "FaderUpAndDown"};
        
        private string[] _variableTypesNumeric = { "int", "float"};
        private string[] _variableTypesNoNumeric = {"string"};

        [MenuItem("Tools/TF-UIPackageTools/Add HUD-Element Script Template/Create Class Dual Variable")]
        public static void ShowWindow()
        {
            GetWindow<ClassDualVariableCreatorTool>("Create Class Tool");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create Class", EditorStyles.boldLabel);

            _className = EditorGUILayout.TextField("Class Name", _className);
            
            _selectedTemplateType = _scriptTemplateTypes[
                EditorGUILayout.Popup("Script Template", System.Array.IndexOf(_scriptTemplateTypes, _selectedTemplateType),
                    _scriptTemplateTypes)];
            
            _selectedTypeNumericOne =
                _variableTypesNumeric[
                    EditorGUILayout.Popup("Variable Type Numeric", System.Array.IndexOf(_variableTypesNumeric, _selectedTypeNumericOne),
                        _variableTypesNumeric)];
            _variableNameNumericOne = EditorGUILayout.TextField("Variable Name Numeric", _variableNameNumericOne);
            
            CheckTemplateForSecondVariable();
            
            if (GUILayout.Button("Create Class"))
            {
                CreateFolder();


                if (_selectedTemplateType == "ImageFiller")
                {
                    CreateSenderWithNumberClass();
                    CreateReceiverImageClass();
                }
                else if (_selectedTemplateType == "FaderUpAndDown")
                {
                    CreateSenderWithTextClass();
                    CreateReceiverTextClass();
                }
                
                CreateInstructiveFile();
                AssetDatabase.Refresh();
            }
        }

        private void CheckTemplateForSecondVariable()
        {
            switch (_selectedTemplateType)
            {
                case "ImageFiller" : 
                        _selectedTypeNumericTwo =
                        _variableTypesNumeric[
                        EditorGUILayout.Popup("Variable Type Numeric", System.Array.IndexOf(_variableTypesNumeric, _selectedTypeNumericTwo), _variableTypesNumeric)];
                        _variableNameNumericTwo = EditorGUILayout.TextField("Variable Name Numeric", _variableNameNumericTwo);
                        break;
                case "FaderUpAndDown" : 
                    _selectedTypeNoNumeric =
                        _variableTypesNoNumeric[
                            EditorGUILayout.Popup("Variable Type No Numeric", System.Array.IndexOf(_variableTypesNoNumeric, _selectedTypeNoNumeric),
                                _variableTypesNoNumeric)];
                    _variableNameNoNumeric = EditorGUILayout.TextField("Variable Name No Numeric", _variableNameNoNumeric);
                    break;
                default:
                    Debug.LogWarning("Script Template dont founded.");
                    break;
            }
        }
        private void CreateFolder()
        {
            _folderPath = "Assets/Scripts/HUDComponents/" + _className + "Component";
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
            Debug.Log("Folder Created");
        }
        private void CreateSenderWithTextClass()
        {
            string senderClassName = _className + "TextDataSender";
            string filePath = Path.Combine(_folderPath, senderClassName + ".cs");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("public class " + senderClassName + " : MonoBehaviour");
                writer.WriteLine("{");
                writer.WriteLine("    //Recomended Use: first variable for current amount, second variable for full amount");
                writer.WriteLine("    public static Action<" + _selectedTypeNumericOne +","+_selectedTypeNoNumeric + "> OnDataNotify;");
                writer.WriteLine();
                writer.WriteLine("    public void NotifyData(" + _selectedTypeNumericOne + " " + _variableNameNumericOne +", " + _selectedTypeNoNumeric + " " + _variableNameNoNumeric+")");
                writer.WriteLine("    {");
                writer.WriteLine("        OnDataNotify?.Invoke("+ _variableNameNumericOne +","+ _variableNameNoNumeric +");");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Debug.Log("The Sender Class was created in: " + filePath);
        }
        private void CreateSenderWithNumberClass()
        {
            string senderClassName = _className + "DataSender";
            string filePath = Path.Combine(_folderPath, senderClassName + ".cs");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine("public class " + senderClassName + " : MonoBehaviour");
                writer.WriteLine("{");
                writer.WriteLine("    //Recomended Use: first variable for current amount, second variable for full amount");
                writer.WriteLine("    public static Action<" + _selectedTypeNumericOne +","+_selectedTypeNumericTwo + "> OnDataNotify;");
                writer.WriteLine();
                writer.WriteLine("    public void NotifyData(" + _selectedTypeNumericOne + " " + _variableNameNumericOne +", " + _selectedTypeNumericTwo + " " + _variableNameNumericTwo+")");
                writer.WriteLine("    {");
                writer.WriteLine("        OnDataNotify?.Invoke("+ _variableNameNumericOne +","+ _variableNameNumericTwo +");");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            Debug.Log("The Sender Class was created in: " + filePath);
        }
        private void CreateReceiverImageClass()
        {
            string receiverClassName = _className + "ImageReceiver";
            string filePath = Path.Combine(_folderPath, receiverClassName+ ".cs");
#region ReceiverImageTemplate

string content = $@"
using UnityEngine;
using UnityEngine.UI;

public class {receiverClassName} : HUDElementReceiver
{{
    [SerializeField] private Image fillImage;
    void Awake()
    {{
        {_className}DataSender.OnDataNotify += UpdateHUD;
    }}
    private void OnDestroy()
    {{
        {_className}DataSender.OnDataNotify -= UpdateHUD;
    }}
    private void UpdateHUD({_selectedTypeNumericOne} {_variableNameNumericOne}, {_selectedTypeNumericTwo} {_variableNameNumericTwo})
    {{
        float factor = {_variableNameNumericOne} / (float){_variableNameNumericTwo};
        fillImage.fillAmount = factor;
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
            string filePath = Path.Combine(_folderPath, _className + "TextDataReceiver.cs");
            string content = $@"
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class {_className}TextDataReceiver : MonoBehaviour
{{
    [SerializeField] private GameObject textGo;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Text _text;
    private float _duration;

    private void Awake()
    {{
        {_className}TextDataSender.OnDataNotify += UpdateHUD;
    }}
    private void OnDestroy()
    {{
        {_className}TextDataSender.OnDataNotify -= UpdateHUD;
    }}
    void UpdateHUD({_selectedTypeNumericOne} {_variableNameNumericOne},{_selectedTypeNoNumeric} {_variableNameNoNumeric})
    {{
        _duration = {_variableNameNumericOne};
        _text.text = {_variableNameNoNumeric};
        textGo.SetActive(false);
        ShowUpTheBanner();
    }}
    private void ShowUpTheBanner()
    {{
        textGo.SetActive(true);
        Sequence sq = DOTween.Sequence();
        sq.Append(_canvasGroup.DOFade(1f,_duration)).OnComplete(ShowOff);
    }}
    private void ShowOff()
    {{
        Sequence sq = DOTween.Sequence();
        sq.Append(_canvasGroup.DOFade(0f,_duration)).OnComplete(() => textGo.SetActive(false));
    }}
}}
";
                
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.Write(content);
            }
    
            Debug.Log("The Receiver class was created in: " + filePath);
            
        }
        private void CreateInstructiveFile()
        {
            string filePath = Path.Combine(_folderPath, "instructions.md");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(
                    $"1) Coloca el componente {_className}Sender.cs dentro del GameObject que controla tu dato a mostrar, luego invoca Notify cuando sea necesario.");
                writer.WriteLine(
                    $"2) Integra el Prefab de HUD Element en la escena que maneja tu HUD dentro del canvas y configura su posicion.");
                writer.WriteLine(
                    $"3) Agrega el componente {_className}Receiver.cs en el formato seleccionado dentro del GameObject que representa el HUDElementReceiver y ajusta sus dependencias.");
            }

            Debug.Log("Clase creada en: " + filePath);
        }
    }
    
}
