using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ABMachine : MonoBehaviour
{
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


    private StateMachine<StateType, TriggerType> _stateMachine;

    private void Start () {
        // StateMachineを生成
        _stateMachine = new StateMachine<StateType, TriggerType>(this, StateType.StateA);

        // 遷移情報を登録
        _stateMachine.AddTransition(StateType.StateA, StateType.StateB, TriggerType.GoB);
        _stateMachine.AddTransition(StateType.StateB, StateType.StateA, TriggerType.GoA);
        // Stateを生成してふるまいを登録
        foreach (StateType state in Enum.GetValues(typeof(StateType)))
        {
            string stateName = Enum.GetName(typeof(StateType),state);
            State tmpState = (State)Activator.CreateInstance(Type.GetType(stateName));
            _stateMachine.SetupState(state,tmpState.OnEnter,tmpState.EnterRoutine,tmpState.OnExit,tmpState.ExitRoutine,tmpState.OnUpdate);
        }
    }

    private void Update()
    {
        // トリガーの発火をチェック
        if (triggerGoB()) _stateMachine.ExecuteTrigger(TriggerType.GoB);
        if (triggerGoA()) _stateMachine.ExecuteTrigger(TriggerType.GoA);

        // ステートマシンを更新
        _stateMachine.Update(Time.deltaTime);
    }

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
