using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///保存本回合怪物数目与ID 调用方法生成本回合所需怪物
/// </summary>
[System.Serializable]
public class Round
{
    //Round还需要实现其他功能,里面有构造函数
    //JsonMapper的方法对有构造函数的类会出错
    //所以用RoundInfo类来储存每一波的怪物ID
    [System.Serializable]
    public class RoundInfo
    {
        public List<int> mMonsterIDList = new List<int>();
    }

    public RoundInfo roundInfo;

    public Round(RoundInfo roundInfo)
    {
        this.roundInfo = roundInfo;
    }

    public void Handel()
    {
        GameController.GetInstance().CreatMonster(roundInfo);
    }

}
