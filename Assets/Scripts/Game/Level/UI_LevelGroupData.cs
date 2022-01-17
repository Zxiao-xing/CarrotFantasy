using System.Collections.Generic;
using LitJson;

public class UI_LevelGroupData
{
    public uint LevelGroupId;               // 关卡组 id
    public string SpriteName;               // 关卡组图片名
    public string TitleSpriteName;          // 关卡组标题图片名
    public uint BelongLevelCount;           // 关卡组包含的关卡数量
    public List<uint> LevelGroupIdList;     // 关卡组包含的子关卡的 id 链表
}
