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
            // stateとtriggerを定義
            symbolTable.define(tokenizer.tokenType(),tokenizer.token());
            // 遷移情報を登録
            if(tokenizer.tokenType()==Tokenizer.TokenType.STATE && tokenizer.nextTokenType(1)==Tokenizer.TokenType.ARROW)
            {
                if(tokenizer.nextToken(3)==":")
                {
                    symbolTable.registerTransition(tokenizer.token(),tokenizer.nextToken(2),tokenizer.nextToken(4));
                }
                else
                {
                    // triggerがない場合，新たにtriggerを生成
                    string trigger = char.ToUpper(tokenizer.token()[0])+tokenizer.token().Substring(1)+"2"+char.ToUpper(tokenizer.nextToken(2)[0])+tokenizer.nextToken(2).Substring(1);
                    symbolTable.registerTransition(tokenizer.token(),tokenizer.nextToken(2),trigger);
                    symbolTable.define(Tokenizer.TokenType.TRIGGER,trigger);
                }
            }
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
