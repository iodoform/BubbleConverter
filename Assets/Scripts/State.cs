using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    ///<summary>
    ///前の状態から現在の状態に遷移してきたときの処理
    ///</summary>
    virtual public void OnEnter() 
    {

    }

    ///<summary>
    ///前の状態から現在の状態に遷移してきたときに開始されるコルーチン
    ///</summary>
    virtual public IEnumerator EnterRoutine() 
    {
        yield return null;
    }

    ///<summary>
    ///現在の状態から次の状態へ遷移するときの処理
    ///</summary>
    virtual public void OnExit()
    {

    }

    ///<summary>
    ///現在の状態から次の状態へ遷移するときに開始されるコルーチン
    ///</summary>
    virtual public IEnumerator ExitRoutine()
    {
        yield return null;
    }

    ///<summary>
    ///状態遷移中以外は毎フレーム呼ばれる
    ///MonoBehaviourのUpdate関数とほぼ同じ扱いでOK
    ///</summary>
    virtual public void OnUpdate(float deltaTime)
    {

    }
}
