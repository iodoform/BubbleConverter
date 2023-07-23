using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateB : State
{
    public override void OnEnter()
    {
        Debug.Log("Enter State B");
    }
    public override void OnExit()
    {
        Debug.Log("Exit State B");
    }
}
