using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///管理所有的回合 处理回合数的改变和开启下一关
/// </summary>
public class RoundHelper
{
    public List<Round> roundList { get; private set; }//本关卡的所有回合数集合
    public int currentRound { get; private set; }
    public RoundHelper(List<Round.RoundInfo> roundInfos)
    {
        currentRound = 0;
        roundList = new List<Round>(roundInfos.Count);
        for (int i = 0; i < roundInfos.Count; ++i)
        {
            roundList.Add(new Round(roundInfos[i]));
        }
        HandelRound();
    }

    private void HandelRound()
    {
        if (currentRound == roundList.Count)//胜利
        {
            GameController.GetInstance().GameOver(true);
            return;
        }
        if (currentRound == roundList.Count - 1)//最后一关,换背景音乐
        {

        }
        roundList[currentRound].Handel();
    }

    /// <summary>
    /// 进入下一回合
    /// </summary>
    /// <returns>进入的是第几回合</returns>
    public int NextRound()
    {
        currentRound++;
        HandelRound();
        return currentRound;
    }
}
