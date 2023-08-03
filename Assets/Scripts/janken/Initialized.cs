using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BubbleConverter;

namespace Janken
{
    public class Initialized : State
    {
        public DataCenter.HandType userHand;
        ///<summary>
        ///前の状態から現在の状態に遷移してきたときの処理
        ///</summary>
        public override void OnEnter() 
        {
            Debug.Log("最初はグー！！");
            Debug.Log("じゃんけんの手を選択\nグー（Q）チョキ（W）パー（E）");
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
            if(Input.GetKeyDown(KeyCode.Q)) userHand = DataCenter.HandType.STONE;
            if(Input.GetKeyDown(KeyCode.W)) userHand = DataCenter.HandType.SCISSORS;
            if(Input.GetKeyDown(KeyCode.E)) userHand = DataCenter.HandType.PAPER;
        }
    }
}