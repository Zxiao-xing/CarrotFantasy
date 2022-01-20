using System.Collections.Generic;
using LitJson;

public class UI_LevelGroupData
{
    public int LevelGroupId;                // 关卡组 id
    public string SpriteName;               // 关卡组图片名
    public string TitleSpriteName;          // 关卡组标题图片名
    public int BelongLevelCount;            // 关卡组包含的关卡数量
    public List<int> LevelGroupIdList;      // 关卡组包含的子关卡的 id 链表
}
