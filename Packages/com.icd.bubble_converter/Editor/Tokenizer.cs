using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
namespace BubbleConverter
{
    public class Tokenizer
    {
        public enum TokenType
        {
            STATE,
            SYMBOL,
            TRIGGER,
            ARROW
        }
        private string[] text;
        private int currentTextPosition = -1;
        private string currentToken;
        private TokenType previousTokenType = TokenType.STATE;
        public Tokenizer(string text)
        {
            // コメント及び不要な部分を削除
            text = Regex.Match(text,@"```\s*mermaid\s*(?:stateDiagram-v2|stateDiagram)\s*(.*)```",RegexOptions.IgnoreCase|RegexOptions.Singleline).Groups[1].Value;
            text = Regex.Replace(text,@"%%.*\n|%%.*\r|\r|\r\n","\n");
            // トリガーを分かち書きから連結
            text = ExtractAndReplace(text,@":.*\n");
            text = Regex.Replace(text,@"[\n\s]+","\n");
            // トークンを分割
            Regex regex = new Regex(@"(-->|\n|:)");
            this.text = regex.Split(text);
            // 長さが0の文字列と改行記号を除外
            this.text = this.text.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            this.text = this.text.Where(s => !(s=="\n")).ToArray();
            //テキストが文法通りか確認
            if(!isGrammatical()) throw new Exception("Incorrect syntax in .md file.");
        }
        private bool isGrammatical()
        {
            for(int i = 0;i<text.Length;i++)
            {
                // 次に来るトークンが文法的に正しいか判定
                switch (tokenType(text[i],i))
                {
                    case TokenType.STATE:
                    {
                        TokenType next;
                        try
                        {
                            next = tokenType(text[i+1],i+1);
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            return true;
                        }
                        if(previousTokenType==TokenType.STATE || previousTokenType == TokenType.TRIGGER)
                        {
                            if(next!=TokenType.ARROW && next!=TokenType.STATE)
                            {
                                throw new Exception("Incorrect syntax in .md file.");
                            }
                        }
                        else if (previousTokenType == TokenType.ARROW)
                        {
                            if(!(next==TokenType.STATE||next==TokenType.SYMBOL))
                            {
                                throw new Exception("Incorrect syntax in .md file.");
                            }
                        }
                        break;
                    }
                    case TokenType.TRIGGER:
                    {
                        TokenType next;
                        try
                        {
                            next = tokenType(text[i+1],i+1);
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            return true;
                        }
                        if(next!=TokenType.STATE)
                        {
                            throw new Exception("Incorrect syntax in .md file.");
                        }
                        break;
                    }
                    case TokenType.SYMBOL:
                    {
                        TokenType next;
                        try
                        {
                            next = tokenType(text[i+1],i+1);
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            throw new Exception(("Incorrect syntax in .md file."));
                        }
                        if(next!=TokenType.TRIGGER)
                        {
                            throw new Exception(("Incorrect syntax in .md file."));
                        }
                        break;
                    }
                    case TokenType.ARROW:
                    {
                            TokenType next;
                        try
                        {
                            next = tokenType(text[i+1],i+1);
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            throw new Exception(("Incorrect syntax in .md file."));
                        }
                        if(next!=TokenType.STATE)
                        {
                            throw new Exception(("Incorrect syntax in .md file."));
                        }
                        break;
                    }
                }
                previousTokenType = tokenType(text[i],i);
            }
            return false;
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
            return tokenType(currentToken, currentTextPosition);
        }

        public TokenType tokenType(string currentToken, int currentTextPosition)
        {
            // SYMBOLか判定
            if(currentToken==":")
            {
                return TokenType.SYMBOL;
            }
            if(currentToken == "-->")
            {
                return TokenType.ARROW;
            }
            // TRIGGERかSTATEか判定
            if(Regex.IsMatch(currentToken,@"\A(\w*)\Z"))
            {
                try
                {
                    if(text[currentTextPosition - 1]==":")
                    {
                        return TokenType.TRIGGER;
                    }
                    else
                    {
                        return TokenType.STATE;
                    }
                }
                catch (System.IndexOutOfRangeException)
                {
                    return TokenType.STATE;
                }
            }
            else if(currentToken=="[*]")
            {
                return TokenType.STATE;
            }
            else
            {
                throw new Exception("Triggers and States must be written in single-byte alphanumeric characters and underscores.");
            }
        }

        public TokenType nextTokenType(int offset)
        {
            string _nextToken = nextToken(offset);
            if(_nextToken!=null)
            {
                return tokenType(_nextToken,currentTextPosition+offset);
            }
            else
            {
                throw new Exception("The position specified by offset has exceeded the index.");
            }
        }
        public string token()
        {
            return currentToken;
        }
        public string nextToken(int offset)
        {
            try
            {
                return text[currentTextPosition+offset];
            }
            catch (System.IndexOutOfRangeException)
            {
                return null;
            }
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
}