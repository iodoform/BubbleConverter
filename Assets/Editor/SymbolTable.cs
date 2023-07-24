using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SymbolTable
{
    private List<string> triggerTable;
    private List<string> stateTable;
    public SymbolTable()
    {
        triggerTable = new List<string>();
        stateTable = new List<string>();
    }
    public void define(Tokenizer.TokenType tokenType, string token)
    {
        if (tokenType == Tokenizer.TokenType.TRIGGER && !triggerTable.Contains(token))
        {
            triggerTable.Add(token);
        }
        else if (tokenType == Tokenizer.TokenType.STATE && !stateTable.Contains(token))
        {
            triggerTable.Add(token);
        }
    }
}
