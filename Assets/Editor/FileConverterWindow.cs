using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class FileConverterWindow : EditorWindow
{
    private string inputFile;
    private string outputFolder;
    private GameObject attach;

    [MenuItem("Custom Tools/File Converter")]
    private static void Init()
    {
        FileConverterWindow window = (FileConverterWindow)EditorWindow.GetWindow(typeof(FileConverterWindow));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("File Converter", EditorStyles.boldLabel);

        inputFile = EditorGUILayout.TextField("Input File Path:", inputFile);
        outputFolder = EditorGUILayout.TextField("Output Folder Path:", outputFolder);
        attach = (GameObject)EditorGUILayout.ObjectField(attach,typeof(GameObject),true);
        if (GUILayout.Button("Convert File"))
        {
            if (!File.Exists(inputFile))
            {
                EditorUtility.DisplayDialog("File Not Found", "The input file does not exist.", "OK");
                return;
            }

            Converter conv = new Converter(inputFile);
            List<string> convertedContentArray = conv.CompileStateMachine();

            // Save each element in the convertedContentArray as a separate file in the specified output folder
            for(int i = 0;i<convertedContentArray.Count;i++)
            {
                if(i == 0)
                {
                    string outputFile = Path.Combine(outputFolder, $"StateMachine.cs");
                    File.WriteAllText(outputFile, convertedContentArray[i]);
                }
                else
                {
                    string outputFile = Path.Combine(outputFolder, $"StateMachine.cs");
                    File.WriteAllText(outputFile, convertedContentArray[i]);
                }
            }

            EditorUtility.DisplayDialog("Conversion Complete", "File conversion completed successfully.", "OK");
        }
    }
}
