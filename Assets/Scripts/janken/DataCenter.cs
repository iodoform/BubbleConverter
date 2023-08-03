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
        public static float waitTime = 1f;
    }
}

