using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 小关卡选择面板管理类
/// </summary>
public class NormalLevelPanel : BasePanel
{
    string fileName = "GameOption/Normal/Level/";
    const string levelPrefabName = "Level";
    List<GameObject> levels = new List<GameObject>(5);
    Transform emp_TowerUsable;
    List<GameObject> towers = new List<GameObject>(5);
    RectTransform contentTrans;
    SlideBookByPos slideBookByPos;
    Text wavesTxt;
    GameObject m_LockMaskUI;

    private int m_curLevelIndex = 0;            // 潜规则，和 id 对应
    private int m_curLevelGroupId;

    List<UI_LevelData> m_levelDataList;

    protected override void Start()
    {
        base.Start();
        m_curLevelGroupId = (int)LevelManager.GetInstance().LevelGroupId;
        m_levelDataList = LevelManager.GetInstance().GetLevelInfoByLevelGroupId(m_curLevelGroupId);
        contentTrans = GetComponentInChildren<ScrollRect>().content;
        slideBookByPos = GetComponentInChildren<SlideBookByPos>();
        wavesTxt = transform.Find("Img_TotalWave/Txt_Waves").GetComponent<Text>();
        m_LockMaskUI = transform.Find("Img_Lock").gameObject;
        slideBookByPos.UpdateContentLength(5);
        emp_TowerUsable = transform.Find("Emp_TowerUsable");

        RefreshCurLevelGroupUI();
        RefreshUI(0);
    }

    private void RefreshCurLevelGroupUI()
    {

        for (int i = 0; i < m_levelDataList.Count; i++)
        {
            GameObject level = uIFacade.GetObject(ObjectFactoryType.UIFactory, levelPrefabName);
            level.GetComponent<Image>().sprite = uIFacade.GetSprite(fileName + m_curLevelGroupId + "/" + LevelManager.GetInstance().GetLevelFileName(m_levelDataList[i].LevelId));
            level.transform.SetParent(contentTrans, false);

            LevelInfo levelInfo = PlayerManager.GetInstance().GetPlayerLevelInfo(m_curLevelGroupId, m_levelDataList[i].LevelId);
            level.transform.Find("Img_Lock").gameObject.SetActive(levelInfo.IsLocked);
            level.transform.Find("Img_AllClear").gameObject.SetActive(levelInfo.IsAllClear);

            // todo：还要加个是否拥有宠物的条件哇
            if (m_levelDataList[i].IsNeedPet && levelInfo.IsLocked)
            {
                GameObject mask = level.transform.Find("Img_Mask").gameObject;
                mask.SetActive(true);

                Image image = mask.transform.Find("Img_Monster").GetComponent<Image>();
                image.sprite = uIFacade.GetSprite("MonsterNest/Monster/Baby/" + m_levelDataList[i].PetSpriteName);
                image.SetNativeSize();
            }
            else
            {
                level.transform.Find("Img_Mask").gameObject.SetActive(false);
            }

            int carrotState = levelInfo.CarrotState;
            if (carrotState != 0)
            {
                GameObject carrot = level.transform.Find("Img_Carrot").gameObject;
                carrot.SetActive(true);
                carrot.GetComponent<Image>().sprite = uIFacade.GetSprite(fileName + "Carrot_" + carrotState);
            }
            else
            {
                level.transform.Find("Img_Carrot").gameObject.SetActive(false);
            }
        }
        transform.Find("Img_CloudLeft").GetComponent<Image>().sprite = uIFacade.GetSprite(fileName + m_curLevelGroupId + "/" + "BG_Left");
        transform.Find("Img_CloudRight").GetComponent<Image>().sprite = uIFacade.GetSprite(fileName + m_curLevelGroupId + "/" + "BG_Right");
    }

    public void RefreshUI(int levelOffset)
    {
        m_curLevelIndex += levelOffset;
        //更新关卡可以使用的塔: 先把所有的塔都放入对象池再构建新的塔
        for (int i = 0; i < towers.Count; ++i)
        {
            uIFacade.PushObject(ObjectFactoryType.UIFactory, "Tower", towers[i]);
        }
        towers.Clear();
        List<int> towerIdList = m_levelDataList[m_curLevelIndex].TowerIdList;
        for (int i = 0; i < towerIdList.Count; ++i)
        {
            towers.Add(uIFacade.GetObject(ObjectFactoryType.UIFactory, "Tower"));

            if (towers[i].transform.parent != emp_TowerUsable)
            {
                towers[i].transform.SetParent(emp_TowerUsable, false);
            }

            Sprite sprite = uIFacade.GetSprite(fileName + "Tower/" + "Tower_" + towerIdList[i]);
            towers[i].GetComponent<Image>().sprite = sprite;
        }
        // 更新 “已锁定” mask
        LevelInfo levelInfo = PlayerManager.GetInstance().GetPlayerLevelInfo(m_curLevelGroupId, m_curLevelIndex);
        m_LockMaskUI.SetActive(levelInfo.IsLocked);
        // 更新怪物波数
        wavesTxt.text = m_levelDataList[m_curLevelIndex].TotalWave.ToString();
    }

    public override void Exit()
    {
        //对象池时用栈实现的,倒着放进去后面使用的时候顺序才不会乱
        for (int i = levels.Count - 1; i >= 0; --i)
        {
            uIFacade.PushObject(ObjectFactoryType.UIFactory, levelPrefabName, levels[i]);
        }
        levels.Clear();
        gameObject.SetActive(false);
    }

    public override void Enter()
    {
        base.Enter();

        // todo：这里看看
        if(slideBookByPos != null)
        {
            m_curLevelGroupId = (int)LevelManager.GetInstance().LevelGroupId;
            slideBookByPos.ResetPos();
            m_curLevelIndex = 0;
            RefreshUI(0);
        }
    }

    public void StartGameBtnClick()
    {
        LevelManager.GetInstance().LevelId = m_curLevelIndex;
        uIFacade.ChangeScene(new GameNormalState(uIFacade));
    }

}
