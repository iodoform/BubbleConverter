using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Janken
{
    /// <summary>
    /// 各状態間で変数を共有するためのクラス．
    /// </summary>
    public static class DataCenter
    {
        public enum HandType
        {
            STONE,
            SCISSORS,
            PAPER
        }
        public static Dictionary<HandType,string> HandDict = new Dictionary<HandType, string>(){{HandType.STONE,"グー"},{HandType.SCISSORS,"チョキ"},{HandType.PAPER,"パー"}};
        public static float waitTime{get;set;} = 1f;
        public static HandType userHand{get;set;}
        public static HandType cpuHand{get;set;}
    }
}

