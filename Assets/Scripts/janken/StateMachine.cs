using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BubbleConverter;
namespace Janken
{
    public partial class StateMachine : MonoBehaviour
    {
        public enum StateType
            {
            Idle,
            Initialized,
            Showdown,
            Win,
            Lose,
            Draw,
            End,
        }
        public enum TriggerType
        {
            SaisyohaGu,
            HandSelected,
            YouWin,
            YouLose,
            ItIsADraw,
            LapseOfACertainTime,
        }
        private StateMachineAssistant<StateType, TriggerType> _stateMachine;
        private Dictionary<StateType,State> _stateDict;
        private void Start ()
        {
            // StateMachineを生成
            _stateMachine = new StateMachineAssistant<StateType, TriggerType>(this);
            // 遷移情報を登録
            _stateMachine.AddTransition(StateType.Idle, StateType.Initialized, TriggerType.SaisyohaGu);
            _stateMachine.AddTransition(StateType.Initialized, StateType.Showdown, TriggerType.HandSelected);
            _stateMachine.AddTransition(StateType.Showdown, StateType.Win, TriggerType.YouWin);
            _stateMachine.AddTransition(StateType.Showdown, StateType.Lose, TriggerType.YouLose);
            _stateMachine.AddTransition(StateType.Showdown, StateType.Draw, TriggerType.ItIsADraw);
            _stateMachine.AddTransition(StateType.Win, StateType.End, TriggerType.LapseOfACertainTime);
            _stateMachine.AddTransition(StateType.Lose, StateType.End, TriggerType.LapseOfACertainTime);
            _stateMachine.AddTransition(StateType.Draw, StateType.End, TriggerType.LapseOfACertainTime);
            // Stateを生成してふるまいを登録
            _stateDict = new Dictionary<StateType, State>();
            foreach (StateType state in Enum.GetValues(typeof(StateType)))
            {
                string stateName = Enum.GetName(typeof(StateType),state);
                State tmpState = (State)gameObject.GetComponent(Type.GetType(GetType().Namespace+ "."+stateName));
                if(tmpState==null)
                {
                    Debug.LogError($"{stateName}コンポーネントが{gameObject.name}にアタッチまたは有効化されていません．{stateName}コンポーネントを{gameObject.name}にアタッチまたは有効化してください．{stateName}コンポーネントが見つからない場合はCustom Tools > FileConverterWindow から再度mdファイルをコンパイルしてください．");
                }
                _stateDict[state] = tmpState;
                _stateMachine.SetupState(state,tmpState.OnEnter,tmpState.EnterRoutine,tmpState.OnExit,tmpState.ExitRoutine,tmpState.OnUpdate);
            }
            _stateMachine.Start(StateType.Idle);
        }
        private void Update()
        {
            // ステートマシンを更新
            _stateMachine.Update(Time.deltaTime);
        
            // トリガーの発火をチェック
            if (triggerSaisyohaGu()) _stateMachine.ExecuteTrigger(TriggerType.SaisyohaGu);
            if (triggerHandSelected()) _stateMachine.ExecuteTrigger(TriggerType.HandSelected);
            if (triggerYouWin()) _stateMachine.ExecuteTrigger(TriggerType.YouWin);
            if (triggerYouLose()) _stateMachine.ExecuteTrigger(TriggerType.YouLose);
            if (triggerItIsADraw()) _stateMachine.ExecuteTrigger(TriggerType.ItIsADraw);
            if (triggerLapseOfACertainTime()) _stateMachine.ExecuteTrigger(TriggerType.LapseOfACertainTime);
        }
    }
}
