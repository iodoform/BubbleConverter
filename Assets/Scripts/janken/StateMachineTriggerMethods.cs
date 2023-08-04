using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BubbleConverter;
namespace Janken
{
    public partial class StateMachine
    {
        // トリガーの発火を制御する関数
        private bool triggerSaisyohaGu()
        {
            return Input.GetKeyDown(KeyCode.S);
        }
        private bool triggerHandSelected()
        {
            return Input.GetKeyDown(KeyCode.Q)||Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.E);
        }
        private bool triggerYouWin()
        {
            switch (DataCenter.userHand)
            {
                case DataCenter.HandType.PAPER:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.STONE;
                }
                case DataCenter.HandType.SCISSORS:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.PAPER;
                }
                case DataCenter.HandType.STONE:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.SCISSORS;
                }
                default: return false;
            }
        }
        private bool triggerYouLose()
        {
            switch (DataCenter.userHand)
            {
                case DataCenter.HandType.PAPER:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.SCISSORS;
                }
                case DataCenter.HandType.SCISSORS:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.STONE;
                }
                case DataCenter.HandType.STONE:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.PAPER;
                }
                default: return false;
            }
        }
        private bool triggerItIsADraw()
        {
            switch (DataCenter.userHand)
            {
                case DataCenter.HandType.PAPER:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.PAPER;
                }
                case DataCenter.HandType.SCISSORS:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.SCISSORS;
                }
                case DataCenter.HandType.STONE:
                {
                    return DataCenter.cpuHand==DataCenter.HandType.STONE;
                }
                default: return false;
            }
        }
        private bool triggerLapseOfACertainTime()
        {
            return ((Win)_stateDict[StateType.Win]).timeFlag||((Lose)_stateDict[StateType.Lose]).timeFlag||((Draw)_stateDict[StateType.Draw]).timeFlag;
        }
    }
}
