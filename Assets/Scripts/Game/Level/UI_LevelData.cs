using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LevelData
{
    public int LevelId;                        // 关卡 Id
    public string SpriteName;                   // 关卡图片名
    public bool IsNeedPet;                      // 是否需要宠物
    public string PetSpriteName;                // 宠物图片
    public int TotalWave;                      // 怪物波数
    public int MapId;                          // 关卡对应的地图 Id
    public List<int> TowerIdList;              // 可以使用的塔的 Id
}