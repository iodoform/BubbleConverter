using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class Tokenizer
{
    public enum TokenType
    {
        STATE,
        SYMBOL,
        TRIGGER
    }
    private string[] text;
    private int currentTextPosition = -1;
    private string currentToken;
    public Tokenizer(string text)
    {
        // コメント及び不要な部分を削除
        text = Regex.Replace(text,@"%%.*\n|%%.*\r|stateDiagram-v2|```|\r|\r\n","\n");
        Regex reg = new Regex("mermaid", RegexOptions.IgnoreCase);
        text = reg.Replace(text,"");
        // トリガーを分かち書きから連結
        text = ExtractAndReplace(text,@":.*\n");
        text = Regex.Replace(text,@"[\n\s]+","\n");
        // トークンを分割
        Regex regex = new Regex(@"(-->|\n|:)");
        this.text = regex.Split(text);
        // 長さが0の文字列を除外
        this.text = this.text.Where(s => !string.IsNullOrEmpty(s)).ToArray();
    }

    public bool hasMoreTokens()
    {
        return currentTextPosition+1<this.text.Length;
    }
    public void advance()
    {
        currentTextPosition+=1;
        currentToken = text[currentTextPosition];
    }
    public TokenType tokenType()
    {
        // SYMBOLか判定
        if(currentToken=="\n"|currentToken=="-->"|currentToken==":")
        {
            return TokenType.SYMBOL;
        }
        // TRIGGERか判定
        for(int i = currentTextPosition-1;i>=0;i--)
        {
            if(text[i]!=" ")
            {
                if(text[i]==":")
                {
                    return TokenType.TRIGGER;
                }
                break;
            }
        }
        return TokenType.STATE;
    }
    public string token()
    {
        return currentToken;
    }

    static string ExtractAndReplace(string inputString, string regexPattern)
    {
        // 正規表現オブジェクトを作成
        Regex regex = new Regex(regexPattern);

        // inputStringから正規表現で指定されたregexPatternのマッチを取得
        MatchCollection matches = regex.Matches(inputString);

        // regexPatternのマッチの空白文字を削除し，分かち書きを連結させて置換
        foreach (Match match in matches)
        {
            string extractedString = match.Value;
            string[] extractedStringArray = extractedString.Split(" ");
            //単語の先頭を大文字に変えて分かち書きを連結
            for(int i = 0;i<extractedStringArray.Length;i++)
            {
                extractedStringArray[i] = char.ToUpper(extractedStringArray[i][0])+extractedStringArray[i].Substring(1);
            }
            string joinedString = string.Join("",extractedStringArray);

            string cleanedString = Regex.Replace(joinedString, @"\s+", "");
            inputString = inputString.Replace(extractedString, cleanedString);
        }

        return inputString;
    }
}
