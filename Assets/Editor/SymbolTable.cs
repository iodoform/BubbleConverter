using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class SymbolTable
{
    private List<string> triggerTable;
    private List<string> stateTable;
    private List<Transition> transitionTable;
    public SymbolTable()
    {
        triggerTable = new List<string>();
        stateTable = new List<string>();
        transitionTable = new List<Transition>();
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

    public void registerTransition(string fromState, string toState, string trigger = null)
    {
        Transition transition = new Transition(fromState,toState,trigger);
        ParameterComparer comparer = new ParameterComparer();
        // 被りがないかチェックしてから登録
        if(!transitionTable.Contains(transition, comparer))
        {
            transitionTable.Add(transition);
        }
    }
    private class Transition
    {
        public string fromState;
        public string toState;
        public string trigger;
        public Transition(string fromState, string toState, string trigger = null)
        {
            this.fromState = fromState;
            this.toState = toState;
            this.trigger = trigger;
        }
    }
    // transitionの比較用のクラス
    private class ParameterComparer : IEqualityComparer<Transition>
    {

        public bool Equals(Transition x, Transition y)
        {
            if( x.fromState ==y.fromState &&
                x.toState == y.toState &&
                x.trigger == y.trigger )
            {
                return true;
            }
            return false;
        }
        public int GetHashCode(Transition obj)
        {
            return obj.fromState.GetHashCode()^obj.toState.GetHashCode()^obj.trigger.GetHashCode();
        }
    }
}
