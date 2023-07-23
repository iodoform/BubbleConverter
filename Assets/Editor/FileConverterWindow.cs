using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class FileConverterWindow : EditorWindow
{
    private string inputFile;
    private string outputFolder;

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

        if (GUILayout.Button("Convert File"))
        {
            if (!File.Exists(inputFile))
            {
                EditorUtility.DisplayDialog("File Not Found", "The input file does not exist.", "OK");
                return;
            }

            Converter conv = new Converter(inputFile);
            List<string> convertedContentArray = conv.FileConvert();

            // Save each element in the convertedContentArray as a separate file in the specified output folder
            int i = 0;
            foreach(string code in convertedContentArray)
            {
                string outputFile = Path.Combine(outputFolder, $"output{i}.txt");
                File.WriteAllText(outputFile, code);
                i++;
            }

            EditorUtility.DisplayDialog("Conversion Complete", "File conversion completed successfully.", "OK");
        }
    }
}
