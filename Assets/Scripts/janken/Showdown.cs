using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BubbleConverter;

namespace Janken
{
    public class Showdown : State
    {
        public DataCenter.HandType cpuHand;
        ///<summary>
        ///前の状態から現在の状態に遷移してきたときの処理
        ///</summary>
        public override void OnEnter() 
        {
            //ランダムな手を選択
            int no = UnityEngine.Random.Range(0, 3);
            DataCenter.cpuHand = (DataCenter.HandType)System.Enum.ToObject(typeof(DataCenter.HandType),no);
            Debug.Log("じゃんけんポン！");
            Debug.Log($"君：{DataCenter.HandDict[DataCenter.userHand]}相手：{DataCenter.HandDict[DataCenter.cpuHand]}");
        }
        ///<summary>
        ///前の状態から現在の状態に遷移してきたときに開始されるコルーチン
        ///</summary>
        public override IEnumerator EnterRoutine() 
        {
            yield return null;
        }
        ///<summary>
        ///現在の状態から次の状態へ遷移するときの処理
        ///</summary>
        public override void OnExit()
        {
        }
        ///<summary>
        ///現在の状態から次の状態へ遷移するときに開始されるコルーチン
        ///</summary>
        public override IEnumerator ExitRoutine()
        {
            yield return null;
        }
        ///<summary>
        ///状態遷移中以外は毎フレーム呼ばれる
        ///MonoBehaviourのUpdate関数とほぼ同じ扱いでOK
        ///</summary>
        public override void OnUpdate(float deltaTime)
        {
        }
    }
}