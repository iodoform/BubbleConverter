using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateA : State
{
    public override void OnEnter()
    {
        Debug.Log("Enter State A");
    }
    public override void OnExit()
    {
        Debug.Log("Exit State A");
    }
}
