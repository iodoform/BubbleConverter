using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Converter
{
    private string text = "";
    private Tokenizer tokenizer;
    public Converter(string filepath)
    {
        try
        {
            //ファイルをオープンする
            using (StreamReader sr = new StreamReader(filepath))
            {
                text = sr.ReadToEnd();
                tokenizer = new Tokenizer(text);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public List<string> CompileStateMachine()
    {
        List<string> resultArray = new List<string>();
        string code = @"using System.Collections;\n
                    using System.Collections.Generic;\n
                    using UnityEngine;\n
                    using System;\n
                    public class ABMachine : MonoBehaviour\n
                    {\n";
        // symbolTableを作成
        SymbolTable symbolTable = new SymbolTable();
        while(tokenizer.hasMoreTokens())
        {
            tokenizer.advance();
            symbolTable.define(tokenizer.tokenType(),tokenizer.token());
        }
        resultArray.Add(code);
        return resultArray;
    }
    public string CompileTransition()
    {

    }
    public string CompileState()
    {
        
    }
    public string CompileTrigger()
    {

    }
    public string CompilrUpdate()
    {

    }
    public string CompileTriggerMethod()
    {

    }
    public string CompileTable()
    {

    }
}
