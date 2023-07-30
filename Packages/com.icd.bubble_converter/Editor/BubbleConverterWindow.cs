using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.Compilation;
namespace BubbleConverter
{
    public class BubbleConverterWindow : EditorWindow
    {
        private string inputFile;
        private string outputFolder;
        private string reconvertFolder;
        private string newFolderName;
        private List<string> compiledStateMachine;
        private string newFolderPath;
        private bool isCompilationRequested = false;
        private bool isReloading = false;
        private bool recompileMode = false;
        private string stateMachineName;

        [MenuItem("Bubble Converter/Bubble Converter Window")]
        private static void Init()
        {
            BubbleConverterWindow window = (BubbleConverterWindow)EditorWindow.GetWindow(typeof(BubbleConverterWindow));
            window.titleContent = new GUIContent("Bubble Converter");
            window.Show();
        }
        private void OnEnable()
        {
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }
        private void OnGUI()
        {
            GUILayout.Label("Bubble Converter", EditorStyles.boldLabel);
            // コンパイル
            if (GUILayout.Button("Select Input File"))
            {
                string initialPath = string.IsNullOrEmpty(inputFile) ? Application.dataPath : inputFile;
                inputFile = EditorUtility.OpenFilePanel("Select Input File", initialPath, "md");
            }
            if (!string.IsNullOrEmpty(inputFile))
            {
                EditorGUILayout.LabelField("Input File Path:", inputFile);
            }

            if  (GUILayout.Button("Select Output Folder"))
            {
                string initialPath = string.IsNullOrEmpty(outputFolder) ? Application.dataPath : outputFolder;
                outputFolder = EditorUtility.OpenFolderPanel("Select Output Folder", initialPath, "");

                // Ensure that the selected output folder is within the Unity project's Asset folder
                if (!outputFolder.StartsWith(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Invalid Output Folder", "Please select an output folder within the Unity project's Asset folder.", "OK");
                    outputFolder = string.Empty;
                }
                else
                {
                    // Convert the selected output folder path to a relative path within the Unity project's Asset folder
                    outputFolder = "Assets" + outputFolder.Substring(Application.dataPath.Length);
                }
            }
            if (!string.IsNullOrEmpty(outputFolder))
            {
                EditorGUILayout.LabelField("Output Folder Path:", outputFolder);
            }

            newFolderName = EditorGUILayout.TextField("State Machine Name:", newFolderName);

            if (!string.IsNullOrEmpty(inputFile) && !string.IsNullOrEmpty(outputFolder) && !string.IsNullOrEmpty(newFolderName))
            {
                if (GUILayout.Button("Convert File"))
                {
                    if (!File.Exists(inputFile))
                    {
                        EditorUtility.DisplayDialog("File Not Found", "The input file does not exist.", "OK");
                        return;
                    }
                    // Create a new folder in the specified output folder
                    stateMachineName = GetUniqueFolderName(outputFolder, newFolderName);
                    newFolderPath = Path.Combine(outputFolder, stateMachineName);
                    Directory.CreateDirectory(newFolderPath);
                    // Create an instance of the Converter class with the input file path
                    Converter conv = new Converter(inputFile, newFolderPath);
                    // Get the compiled state machine from the Converter
                    compiledStateMachine = conv.CompileStateMachine(stateMachineName);

                    // 生成したスクリプトを保存
                    SaveScript(compiledStateMachine,newFolderPath);
                    // スクリプトをコンパイル
                    CompilationPipeline.RequestScriptCompilation();
                    isCompilationRequested = true;
                }
            }
            // 再コンパイル
            if  (GUILayout.Button("Select Reconvert Folder"))
            {
                string initialPath = string.IsNullOrEmpty(reconvertFolder) ? Application.dataPath : reconvertFolder;
                reconvertFolder = EditorUtility.OpenFolderPanel("Select Reconvert Folder", initialPath, "");
                // Ensure that the selected output folder is within the Unity project's Asset folder
                if (!reconvertFolder.StartsWith(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Invalid Output Folder", "Please select an output folder within the Unity project's Asset folder.", "OK");
                    reconvertFolder = string.Empty;
                }
                else
                {
                    // Convert the selected output folder path to a relative path within the Unity project's Asset folder
                    reconvertFolder = "Assets" + reconvertFolder.Substring(Application.dataPath.Length);
                }
            }
            if (!string.IsNullOrEmpty(reconvertFolder))
            {
                EditorGUILayout.LabelField("Reconvert Folder Path:", reconvertFolder);
            }

            if (!string.IsNullOrEmpty(reconvertFolder))
            {
                if (GUILayout.Button("Reconvert File"))
                {

                    if (!File.Exists(inputFile))
                    {
                        EditorUtility.DisplayDialog("File Not Found", "The input file does not exist.", "OK");
                        return;
                    }
                    if (!Directory.Exists(reconvertFolder))
                    {
                        EditorUtility.DisplayDialog("Folder Not Found", "The Reconvert Folder does not exist.", "OK");
                        return;
                    }
                    stateMachineName = Path.GetFileName(reconvertFolder);
                    // Create an instance of the Converter class with the input file path
                    Converter conv = new Converter(inputFile, reconvertFolder);
                    // Get the compiled state machine from the Converter
                    compiledStateMachine = conv.RecompileStateMachine(stateMachineName);

                    // 生成したスクリプトを保存
                    SaveScript(compiledStateMachine, reconvertFolder);
                    // スクリプトをコンパイル
                    CompilationPipeline.RequestScriptCompilation();
                    isCompilationRequested = true;
                    recompileMode = true;
                }
            }
        }

        private void SaveScript(List<string> compiledStateMachine, string folderPath)
        {
            int tmpcount = 0;
            for (int i = 0; i < compiledStateMachine.Count; i++)
            {
                // Extract class name using regex
                string className = ExtractClassName(compiledStateMachine[i]);
                if (className == "StateMachine")
                {
                    tmpcount++;
                    if (tmpcount == 2)
                    {
                        className = "StateMachineTriggerMethods";
                    }
                }
                // Generate file name using the extracted class name
                string fileName = Path.Combine(folderPath, $"{className}.cs");
                File.WriteAllText(fileName, compiledStateMachine[i]);
                AssetDatabase.ImportAsset(fileName, ImportAssetOptions.ForceUpdate);
            }
        }

        // Helper method to get a unique folder name by appending "(n)" to the input name if necessary
        private string GetUniqueFolderName(string parentFolderPath, string baseName)
        {
            string folderName = baseName;
            int suffix = 1;

            while (Directory.Exists(Path.Combine(parentFolderPath, folderName)))
            {
                folderName = $"{baseName}_{suffix}";
                suffix++;
            }

            return folderName;
        }

        // Helper method to extract class name using regex
        private string ExtractClassName(string content)
        {
            string pattern = @"public\s+(?:partial\s+)?class\s+(\w+)\b";
            Match match = Regex.Match(content, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                Debug.LogError("No public class is defined.");
            }
            return "DefaultClassName";
        }
        private  void Update()
        {
            if (EditorApplication.isCompiling && !isReloading)
            {
                isReloading = true;
            }
            else if (!EditorApplication.isCompiling && isReloading)
            {
                isReloading = false;
                OnDomainReloaded();
            }
        }
        private void OnDomainReloaded()
        {
            if (isCompilationRequested)
            {
                isCompilationRequested = false;
                GameObject stateMachineGO;
                string tmpNewFolderPath;
                if(recompileMode)
                {
                    StateMachineData data = new StateMachineData();
                    string json = File.ReadAllText(Path.Combine(reconvertFolder,"StateMachineData.json"));
                    EditorJsonUtility.FromJsonOverwrite(json,data);
                    recompileMode = false;
                    stateMachineGO = GameObject.Find(data.name);
                    tmpNewFolderPath = reconvertFolder;
                }
                else
                {
                    stateMachineGO = new GameObject(stateMachineName+"State Machine");
                    tmpNewFolderPath = newFolderPath;
                }
                // コンポーネント化してアタッチ
                for (int i = 0; i < compiledStateMachine.Count; i++)
                {
                    // Extract class name using regex
                    string className = ExtractClassName(compiledStateMachine[i]);
                    // Generate file name using the extracted class name
                    string fileName = Path.Combine(tmpNewFolderPath, $"{className}.cs");
                    // Create a C# script asset from the output file
                    MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(fileName);
                    if (scriptAsset != null && !stateMachineGO.GetComponent(scriptAsset.GetClass()))
                    {
                        // Attach the script component to the "State Machine" GameObject
                        stateMachineGO.AddComponent(scriptAsset.GetClass());
                    }
                }
                string name = stateMachineGO.name;
                string pathFileName = Path.Combine(tmpNewFolderPath, "StateMachineData.json");
                string jsonData = EditorJsonUtility.ToJson(new StateMachineData(inputFile,name));
                File.WriteAllText(pathFileName,jsonData);
                EditorUtility.DisplayDialog("Conversion Complete", "File conversion completed successfully.", "OK");
                
            }
        }
        [System.Serializable]
        public class StateMachineData
        {
            public string inputPath;
            public string name;
            public StateMachineData(string inputPath="", string name="")
            {
                this.inputPath = inputPath;
                this.name = name;
            }
        }
    }
}