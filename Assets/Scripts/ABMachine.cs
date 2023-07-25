using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BubbleConverter;
public class ABMachine : MonoBehaviour
{

// CompileTable
    public enum StateType
    {
        StateA,
        StateB
    }

    public enum TriggerType
    {
        GoA,
        GoB
    }
// \CompileTable

    private StateMachineAssistant<StateType, TriggerType> _stateMachine;
// CompileStart
    private void Start () {
        // StateMachineを生成
        _stateMachine = new StateMachineAssistant<StateType, TriggerType>(this, StateType.StateA);
        // CompileTransition
        // 遷移情報を登録
        _stateMachine.AddTransition(StateType.StateA, StateType.StateB, TriggerType.GoB);
        _stateMachine.AddTransition(StateType.StateB, StateType.StateA, TriggerType.GoA);
        // \CompileTransition
        // Stateを生成してふるまいを登録
        foreach (StateType state in Enum.GetValues(typeof(StateType)))
        {
            string stateName = Enum.GetName(typeof(StateType),state);
            State tmpState = (State)gameObject.GetComponent(Type.GetType(stateName));
            if(tmpState==null)
            {
                Debug.LogError($"{stateName}コンポーネントが{gameObject.name}にアタッチまたは有効化されていません．{stateName}コンポーネントを{gameObject.name}にアタッチまたは有効化してください．{stateName}コンポーネントが見つからない場合はCustom Tools > FileConverterWindow から再度mdファイルをコンパイルしてください．");
            }
            _stateMachine.SetupState(state,tmpState.OnEnter,tmpState.EnterRoutine,tmpState.OnExit,tmpState.ExitRoutine,tmpState.OnUpdate);
        }
    }
// \CompileStart

// CompileUpdate
    private void Update()
    {
        // トリガーの発火をチェック
        if (triggerGoB()) _stateMachine.ExecuteTrigger(TriggerType.GoB);
        if (triggerGoA()) _stateMachine.ExecuteTrigger(TriggerType.GoA);

        // ステートマシンを更新
        _stateMachine.Update(Time.deltaTime);
    }
// \CompileUpdate

// CompileTriggerMethods
    // トリガーの発火を制御する関数
    private bool triggerGoA()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    private bool triggerGoB()
    {
        return Input.GetKeyDown(KeyCode.B);
    }
}
// \CompileTriggerMethods