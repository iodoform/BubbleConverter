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

        public List<string> TriggerTable { get => triggerTable; private set => triggerTable = value; }
        public List<string> StateTable { get => stateTable; private set => stateTable = value; }
        public List<Transition> TransitionTable { get => transitionTable; private set => transitionTable = value; }
        public SymbolTable()
        {
            TriggerTable = new List<string>();
            StateTable = new List<string>();
            TransitionTable = new List<Transition>();
        }
        public void define(Tokenizer.TokenType tokenType, string token)
        {
            if (tokenType == Tokenizer.TokenType.TRIGGER && !TriggerTable.Contains(token))
            {
                TriggerTable.Add(token);
            }
            else if (tokenType == Tokenizer.TokenType.STATE && !StateTable.Contains(token))
            {
                StateTable.Add(token);
            }
        }

        public void registerTransition(string fromState, string toState, string trigger = null)
        {
            Transition transition = new Transition(fromState,toState,trigger);
            ParameterComparer comparer = new ParameterComparer();
            // 被りがないかチェックしてから登録
            if(!TransitionTable.Contains(transition, comparer))
            {
                TransitionTable.Add(transition);
            }
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