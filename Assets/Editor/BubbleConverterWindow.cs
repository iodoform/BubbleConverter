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
        private string newFolderName;
        private List<string> compiledStateMachine;
        private string newFolderPath;
        private bool compiledFlag = false;

        [MenuItem("Custom Tools/Bubble Converter")]
        private static void Init()
        {
            BubbleConverterWindow window = (BubbleConverterWindow)EditorWindow.GetWindow(typeof(BubbleConverterWindow));
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Bubble Converter", EditorStyles.boldLabel);

            if (GUILayout.Button("Select Input File"))
            {
                string initialPath = string.IsNullOrEmpty(inputFile) ? Application.dataPath : inputFile;
                inputFile = EditorUtility.OpenFilePanel("Select Input File", initialPath, "md");
                compiledFlag = false;
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
                    compiledFlag = false;
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
                    string stateMachineName = GetUniqueFolderName(outputFolder, newFolderName);
                    newFolderPath = Path.Combine(outputFolder, stateMachineName);
                    Directory.CreateDirectory(newFolderPath);
                    // Create an instance of the Converter class with the input file path
                    Converter conv = new Converter(inputFile);

                    // Get the compiled state machine from the Converter
                    compiledStateMachine = conv.CompileStateMachine(stateMachineName);

                    // 生成したスクリプトを保存
                    for (int i = 0; i < compiledStateMachine.Count; i++)
                    {
                        // Extract class name using regex
                        string className = ExtractClassName(compiledStateMachine[i]);

                        // Generate file name using the extracted class name
                        string fileName = Path.Combine(newFolderPath, $"{className}.cs");
                        File.WriteAllText(fileName, compiledStateMachine[i]);
                        AssetDatabase.ImportAsset(fileName, ImportAssetOptions.ForceUpdate ); 
                    }
                    // スクリプトをコンパイル
                    CompilationPipeline.RequestScriptCompilation();
                    EditorUtility.DisplayDialog("Conversion Complete", "File conversion completed successfully.", "OK");
                    compiledFlag = true;
                }
                if(compiledFlag)
                {
                    if (GUILayout.Button("Generate State Machine"))
                    {
                        // Create a new empty GameObject named "State Machine" in the scene
                        GameObject stateMachineGO = new GameObject("State Machine");
                        // コンポーネント化してアタッチ
                        for (int i = 0; i < compiledStateMachine.Count; i++)
                        {
                            // Extract class name using regex
                            string className = ExtractClassName(compiledStateMachine[i]);

                            // Generate file name using the extracted class name
                            string fileName = Path.Combine(newFolderPath, $"{className}.cs");
                            // Create a C# script asset from the output file
                            MonoScript scriptAsset = AssetDatabase.LoadAssetAtPath<MonoScript>(fileName);
                            if (scriptAsset != null)
                            {
                                // Attach the script component to the "State Machine" GameObject
                                stateMachineGO.AddComponent(scriptAsset.GetClass());
                            }
                        }
                        EditorUtility.DisplayDialog("Generation complete", "State Machine generation completed successfully.", "OK");
                    }
                }
                
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
            string pattern = @"public\s+class\s+(\w+)\b";
            Match match = Regex.Match(content, pattern);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "DefaultClassName";
        }
    }
}