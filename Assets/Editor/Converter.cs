using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Converter
{
    private string text = "";
    private Tokenizer tokenizer;
    private string initialState = null;
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
            if(tokenizer.token() != "[*]") 
            {
                symbolTable.define(tokenizer.tokenType(),tokenizer.token());
            }
            // 遷移情報を登録
            if(tokenizer.tokenType()==Tokenizer.TokenType.STATE && tokenizer.nextTokenType(1)==Tokenizer.TokenType.ARROW)
            {
                if(tokenizer.token() != "[*]")
                {
                    if(tokenizer.nextToken(2)!="[*]")
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
                    else
                    {
                        Debug.LogError("終了状態[*]は使用できません．別の文字列に置き換えてください．");
                    }
                }
                else
                {
                    initialState = tokenizer.nextToken(2);
                }
            }
        }
        if(initialState == null)
        {
            // initialStateの指定がない場合はsymbolTableの最初のStateを代入
            initialState = symbolTable.stateTable[0];
        }
        code += CompileTable();
        code += "private StateMachine<StateType, TriggerType> _stateMachine;\n";
        code += CompileStart();
        code += CompileUpdate();
        code += CompileTriggerMethods();
        resultArray.Add(code);
        foreach (string state in symbolTable.stateTable)
        {
            resultArray.Add(CompileState(state));
        }
        return resultArray;
    }
    public string CompileTransition()
    {

    }
    public string CompileUpdate()
    {

    }
    public string CompileTriggerMethods()
    {

    }
    public string CompileTable()
    {

    }
    public string CompileStart()
    {

    }
    public string CompileState(string state)
    {

    }
}
