using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace BubbleConverter
{
    [System.Serializable]
    public class SymbolTable
    {
        [SerializeField]
        private List<string> triggerTable;
        [SerializeField]
        private List<string> stateTable;
        [SerializeField]
        private List<Transition> transitionTable;

        public List<string> TriggerTable { get => triggerTable; set => triggerTable = value; }
        public List<string> StateTable { get => stateTable; set => stateTable = value; }
        public List<Transition> TransitionTable { get => transitionTable; private set => transitionTable = value; }
        public SymbolTable()
        {
            TriggerTable = new List<string>();
            StateTable = new List<string>();
            TransitionTable = new List<Transition>();
        }
        public void define(Tokenizer.TokenType tokenType, string token)
        {
            token = char.ToUpper(token[0])+token.Substring(1);
            if (tokenType == Tokenizer.TokenType.TRIGGER && !TriggerTable.Contains(token))
            {
                TriggerTable.Add(token);
            }
            else if (tokenType == Tokenizer.TokenType.STATE && !StateTable.Contains(token))
            {
                StateTable.Add(token);
            }
        }

        public void registerTransition(string fromState, string toState, string trigger)
        {
            fromState = char.ToUpper(fromState[0])+fromState.Substring(1);
            toState = char.ToUpper(toState[0])+toState.Substring(1);
            trigger = char.ToUpper(trigger[0])+trigger.Substring(1);
            Transition transition = new Transition(fromState,toState,trigger);
            ParameterComparer comparer = new ParameterComparer();
            // 被りがないかチェックしてから登録
            if(!TransitionTable.Contains(transition, comparer))
            {
                TransitionTable.Add(transition);
            }
        }
        public SymbolTable CloneSymbolTable()
        {
            SymbolTable copy = (SymbolTable)MemberwiseClone();
            copy.stateTable = new List<string>(this.stateTable);
            copy.triggerTable = new List<string>(this.triggerTable);
            copy.transitionTable = new List<Transition>();
            foreach(Transition transition in this.transitionTable)
            {
                copy.transitionTable.Add(transition);
            }
            return copy;
        }
        [System.Serializable]
        public class Transition
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
            public Transition CloneTransition()
            {
                return (Transition)this.MemberwiseClone();
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
}