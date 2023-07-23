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

    public List<string> FileConvert()
    {
        List<string> resultArray = new List<string>();
        string code = "";
        while(tokenizer.hasMoreTokens())
        {
            tokenizer.advance();
            code = code + "<" +tokenizer.tokenType().ToString()+ @">" + tokenizer.token()+ "</" +tokenizer.tokenType().ToString()+ ">" ;
        }
        resultArray.Add(code);
        return resultArray;
    }
}
