using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEditor;
namespace BubbleConverter
{
    public class Converter
    {
        private string text = "";
        private Tokenizer tokenizer;
        private string initialState = null;
        private SymbolTable symbolTable;
        private string stateTemplateFilePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Packages\bubble_converter\Runtime\StateTemplate.cs");
        private string dataCenterTemplateFilePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Packages\bubble_converter\Runtime\DataCenterTemplate.cs");
        private string outputFolderPath;
        public Converter(string inputFilePath, string outputFolderPath)
        {
            this.outputFolderPath = outputFolderPath;
            try
            {
                //ファイルをオープンする
                using (StreamReader sr = new StreamReader(inputFilePath))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            tokenizer = new Tokenizer(text);
        }

        public List<string> CompileStateMachine(string folderName)
        {
            // symbolTableを初期化
            symbolTable = new SymbolTable();
            MakeSymbolTable(symbolTable);
            // ステートマシン本体を生成
            List<string> resultArray = CompileMain(folderName);
            // トリガーを生成
            resultArray.Add(CompileTriggerMethods(folderName));
            // ステートを生成
            foreach (string state in symbolTable.StateTable)
            {
                resultArray.Add(CompileState(state, MakePascalCase(folderName)));
            }
            // データセンターを生成
            resultArray.Add(CompileDataCenter(MakePascalCase(folderName)));
            SaveSymbolTable(folderName);
            return resultArray;
        }
        public List<string> RecompileStateMachine(string folderName)
        {
            // symbolTableを初期化
            symbolTable = new SymbolTable();
            MakeSymbolTable(symbolTable);
            // ステートマシン本体を生成
            List<string> resultArray = CompileMain(folderName);
            // 古いSymbolTableをロード
            SymbolTable oldSymbolTable = new SymbolTable();
            string json = File.ReadAllText(Path.Combine(outputFolderPath, $"{MakePascalCase(folderName)}.JSON"));
            EditorJsonUtility.FromJsonOverwrite(json, oldSymbolTable);
            SymbolTable cloneSymbolTable = symbolTable.CloneSymbolTable();
            //oldSymbolTableに含まれるトリガーとステートを除去
            cloneSymbolTable.TriggerTable.RemoveAll(oldSymbolTable.TriggerTable.Contains);
            cloneSymbolTable.StateTable.RemoveAll(oldSymbolTable.StateTable.Contains);
            // トリガーを更新
            resultArray.Add(RecompileTriggerMethods(cloneSymbolTable, symbolTable, folderName));
            // 追加のステートを生成
            foreach (string state in cloneSymbolTable.StateTable)
            {
                resultArray.Add(CompileState(state, MakePascalCase(folderName)));
            }
            SaveSymbolTable(folderName);
            return resultArray;
        }

        private List<string> CompileMain(string folderName)
        {
            List<string> resultArray = new List<string>();
            string code = "using System.Collections;\n";
            code += "using System.Collections.Generic;\n";
            code += "using UnityEngine;\n";
            code += "using System;\n";
            code += "using BubbleConverter;\n";
            code += $"namespace {MakePascalCase(folderName)}\n";
            code += "{\n";
            code += $"    public partial class StateMachine : MonoBehaviour\n";
            code += "    {\n";
            // 今後システムがより複雑化したときのために，indentを設定できるようにしておく
            string indent = "        ";
            code += CompileTable(indent);
            code += "        private StateMachineAssistant<StateType, TriggerType> _stateMachine;\n";
            code += "        private Dictionary<StateType,State> _stateDict;\n";
            code += CompileStart(indent);
            code += CompileUpdate(indent);
            code += "    }\n";
            code += "}\n";
            resultArray.Add(code);
            return resultArray;
        }

        private void SaveSymbolTable(string folderName)
        {
            //SymbolTableをJSON化して保存
            string symbolTableJson = EditorJsonUtility.ToJson(symbolTable, true);
            string fileName = Path.Combine(outputFolderPath, $"{MakePascalCase(folderName)}.JSON");
            File.WriteAllText(fileName, symbolTableJson);
        }

        private void MakeSymbolTable(SymbolTable symbolTable)
        {
            while (tokenizer.hasMoreTokens())
            {
                tokenizer.advance();
                // stateとtriggerを定義
                if (tokenizer.token() != "[*]")
                {
                    symbolTable.define(tokenizer.tokenType(), tokenizer.token());
                }
                // 遷移情報を登録
                if (tokenizer.tokenType() == Tokenizer.TokenType.STATE && tokenizer.nextToken(1) != null && tokenizer.nextTokenType(1) == Tokenizer.TokenType.ARROW)
                {
                    if (tokenizer.token() != "[*]")
                    {
                        if (tokenizer.nextToken(2) != "[*]")
                        {
                            if (tokenizer.nextToken(3) == ":")
                            {
                                symbolTable.registerTransition(tokenizer.token(), tokenizer.nextToken(2), tokenizer.nextToken(4));
                            }
                            else
                            {
                                // triggerがない場合，新たにtriggerを生成
                                string trigger = char.ToUpper(tokenizer.token()[0]) + tokenizer.token().Substring(1) + "2" + char.ToUpper(tokenizer.nextToken(2)[0]) + tokenizer.nextToken(2).Substring(1);
                                symbolTable.registerTransition(tokenizer.token(), tokenizer.nextToken(2), trigger);
                                symbolTable.define(Tokenizer.TokenType.TRIGGER, trigger);
                            }
                        }
                        else
                        {
                            throw new Exception("Exit status [*] cannot be used. Please replace it with another string.");
                        }
                    }
                    else
                    {
                        initialState = MakePascalCase(tokenizer.nextToken(2));
                    }
                }
            }
            if (initialState == null)
            {
                // initialStateの指定がない場合はsymbolTableの最初のStateを代入
                initialState = symbolTable.StateTable[0];
            }
        }


        private string CompileDataCenter(string stateMachineName)
        {
            //DataCenterのテンプレートファイルをもとに新しいDataCenterのクラスを作成

            // ファイルが存在するか確認
            if (!File.Exists(dataCenterTemplateFilePath))
            {
                throw new Exception("指定されたファイルが見つかりません: " + dataCenterTemplateFilePath);
            }

            // ファイルの内容を読み込む
            string fileContent = File.ReadAllText(dataCenterTemplateFilePath);

            // 正規表現を使って最初に出てくるnamespaceを抽出
            Match nameMatch = Regex.Match(fileContent, @"namespace\s+(\w+)\b");

            if (nameMatch.Success)
            {
                string currentNameSpace = nameMatch.Groups[1].Value;

                // namespaceを新しいnamespaceに置き換える
                fileContent = fileContent.Replace(currentNameSpace, stateMachineName);
                return fileContent;
            }
            else
            {
                throw new Exception("namespaceの定義が見つかりませんでした");
            }
        }
        private string CompileTransition(string indent = "")
        {
            string code = indent+"// 遷移情報を登録\n";
            foreach(SymbolTable.Transition transition in symbolTable.TransitionTable)
            {
                code += indent+$"_stateMachine.AddTransition(StateType.{transition.fromState}, StateType.{transition.toState}, TriggerType.{transition.trigger});\n";
            }
            return code;
        }
        private string CompileUpdate(string indent = "")
        {
            string code = indent+"private void Update()\n";
            code += indent+"{\n";
            code += indent+"    // ステートマシンを更新\n";
            code += indent+"    _stateMachine.Update(Time.deltaTime);\n";
            code += indent+"\n";
            code += indent+"    // トリガーの発火をチェック\n";
            foreach(string trigger in symbolTable.TriggerTable)
            {
                code += indent+$"    if (trigger{char.ToUpper(trigger[0])+trigger.Substring(1)}()) _stateMachine.ExecuteTrigger(TriggerType.{trigger});\n";
            }

            
            code += indent+"}\n";
            return code;
        }
        private string CompileTriggerMethods(string name)
        {
            string code = "using System.Collections;\n";
            code += "using System.Collections.Generic;\n";
            code += "using UnityEngine;\n";
            code += "using System;\n";
            code += "using BubbleConverter;\n";
            code += $"namespace {MakePascalCase(name)}\n";
            code += "{\n";
            code += $"    public partial class StateMachine\n";
            code += "    {\n";
            code +="        // トリガーの発火を制御する関数\n";
            foreach(string trigger in symbolTable.TriggerTable)
            {
                code += $"        private bool trigger{char.ToUpper(trigger[0])+trigger.Substring(1)}()\n";
                code += "        {\n";
                code += "            return true;\n";
                code += "        }\n";
            }
            code += "    }\n";
            code += "}\n";
            return code;
        }
        private string RecompileTriggerMethods(SymbolTable cloneSymbolTable, SymbolTable newSymbolTable, string name)
        {
            string oldCode = File.ReadAllText(Path.Combine(outputFolderPath, "StateMachineTriggerMethods.cs"));
            // 正規表現で旧StateMachineのusingディレクティブを抽出
            MatchCollection directives = Regex.Matches(oldCode,@"using.*?;");
            string code = "";
            foreach(Match item in directives)
            {
                code = code + item.Groups[0].Value+"\n";
            }
            code += $"namespace {MakePascalCase(name)}\n";
            code += "{\n";
            code += $"    public partial class StateMachine\n";
            code += "    {\n";
            code +="        // トリガーの発火を制御する関数\n";
            // 正規表現で旧StateMachineのトリガー関数を抽出
            List<string> methods = ExtractMethods(oldCode);
            Regex reg = new Regex(@"private\s+bool\s+(\w+)");
            foreach(string method in methods)
            {
                // 古いトリガーのうち，新しいシンボルテーブルに含まれるもののみを抽出
                if(newSymbolTable.TriggerTable.Contains(reg.Match(method).Groups[1].Value.Substring(7)))
                {
                    code += "        "+method;
                }
            }
            // 新規トリガーを追加
            foreach(string trigger in cloneSymbolTable.TriggerTable)
            {
                code += $"        private bool trigger{char.ToUpper(trigger[0])+trigger.Substring(1)}()\n";
                code += "        {\n";
                code += "            return true;\n";
                code += "        }\n";
            }
            code += "    }\n";
            code += "}\n";
            return code;
        }
        private string CompileTable(string indent = "")
        {
            string code = indent+"public enum StateType\n";
            code += indent + "    {\n";
            foreach(string state in symbolTable.StateTable)
            {
                code += indent+"    " + state + ",\n";
            }
            code += indent+"}\n";
            code += indent+"public enum TriggerType\n";
            code += indent+"{\n";
            foreach(string trigger in symbolTable.TriggerTable)
            {
                code += indent+"    " + trigger + ",\n";
            }
            code += indent+"}\n";
            return code;
        }
        private string CompileStart(string indent = "")
        {
            string code = indent+"private void Start ()\n";
            code += indent+"{\n";
            code += indent+"    // StateMachineを生成\n";
            code += indent+"    _stateMachine = new StateMachineAssistant<StateType, TriggerType>(this);\n";
            code += CompileTransition(indent+"    ");
            code += indent+"    // Stateを生成してふるまいを登録\n";
            code += indent+"    _stateDict = new Dictionary<StateType, State>();\n";
            code += indent+"    foreach (StateType state in Enum.GetValues(typeof(StateType)))\n";
            code += indent+"    {\n";
            code += indent+"        string stateName = Enum.GetName(typeof(StateType),state);\n";
            code += indent+"        State tmpState = (State)gameObject.GetComponent(Type.GetType(GetType().Namespace+ \".\"+stateName));\n";
            code += indent+"        if(tmpState==null)\n";
            code += indent+"        {\n";
            code += indent+"            Debug.LogError($\"{stateName}コンポーネントが{gameObject.name}にアタッチまたは有効化されていません．{stateName}コンポーネントを{gameObject.name}にアタッチまたは有効化してください．{stateName}コンポーネントが見つからない場合はCustom Tools > FileConverterWindow から再度mdファイルをコンパイルしてください．\");\n";
            code += indent+"        }\n";
            code += indent+"        _stateDict[state] = tmpState;\n";
            code += indent+"        _stateMachine.SetupState(state,tmpState.OnEnter,tmpState.EnterRoutine,tmpState.OnExit,tmpState.ExitRoutine,tmpState.OnUpdate);\n";
            code += indent+"    }\n";
            code += indent+$"    _stateMachine.Start(StateType.{initialState});\n";
            code += indent+"}\n";
            return code;
        }
        private string CompileState(string state, string stateMachineName)
        {
            //Stateのテンプレートファイルをもとに新しいStateのクラスを作成

            // ファイルが存在するか確認
            if (!File.Exists(stateTemplateFilePath))
            {
                throw new Exception("指定されたファイルが見つかりません: " + stateTemplateFilePath);
            }

            // ファイルの内容を読み込む
            string fileContent = File.ReadAllText(stateTemplateFilePath);

            // 正規表現を使って最初に出てくるnamespaceを抽出
            Match nameMatch = Regex.Match(fileContent, @"namespace\s+(\w+)\b");

            if (nameMatch.Success)
            {
                string currentNameSpace = nameMatch.Groups[1].Value;

                // namespaceを新しいnamespaceに置き換える
                fileContent = fileContent.Replace(currentNameSpace, stateMachineName);

                // 正規表現を使って最初に出てくるpublic classのクラス名を抽出
                Match match = Regex.Match(fileContent, @"public\s+class\s+(\w+)\b");

                if (match.Success)
                {
                    string currentClassName = match.Groups[1].Value;

                    // クラス名を新しいクラス名に置き換える
                    string replacedContent = fileContent.Replace(currentClassName, state);

                    // 変更後の内容を返す
                    return replacedContent;
                }
                else
                {
                    throw new Exception("publicなclass定義が見つかりませんでした");
                }
            }
            else
            {
                throw new Exception("namespaceの定義が見つかりませんでした");
            }
        }
        private string MakePascalCase(string text)
        {
            string[] extractedStringArray = text.Split(" ");
            //単語の先頭を大文字に変えて分かち書きを連結
            for(int i = 0;i<extractedStringArray.Length;i++)
            {
                extractedStringArray[i] = char.ToUpper(extractedStringArray[i][0])+extractedStringArray[i].Substring(1);
            }
            string joinedString = string.Join("",extractedStringArray);
            string cleanedString = Regex.Replace(joinedString, @"\s+", "");
            return cleanedString;
        }
        private List<string> ExtractMethods(string code)
        {
            List<string> resultCode = new List<string>();
            string tmpCode = "";
            bool countMode = false;
            int braCount = 0;
            int cketCount = 0;
            for(int i = 0;i<code.Length;i++)
            {
                tmpCode = tmpCode+code[i];
                if(countMode)
                {
                    if(code[i].ToString()=="{") braCount++;
                    else if(code[i].ToString()=="}") cketCount++;
                    if(braCount==cketCount)
                    {
                        countMode=false;
                        resultCode.Add(tmpCode+"\n");
                        tmpCode = "";
                    }
                }
                else if(Regex.IsMatch(tmpCode,@"private\s+bool\s+(\w+)\s*\([^)]*\)\s*{")) 
                {
                    tmpCode = Regex.Match(tmpCode,@"private\s+bool\s+(\w+)\s*\([^)]*\)\s*{").Groups[0].Value;
                    countMode = true;
                    braCount = 1;
                    cketCount = 0;
                }
            }
            return resultCode;
        }
    }
}