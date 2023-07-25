using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BubbleConverter;
namespace testA
{
public class StateB:MonoBehaviour
{
    public  void OnEnter()
    {
        Debug.Log("Enter State B");
    }
    public  void OnExit()
    {
        Debug.Log("Exit State B");
    }
}
}