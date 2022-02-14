using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NormalBigLevelPanel : BasePanel
{
    SlideBookByPos m_slideBookByPos;
    Transform m_contentTrans;

    protected void Awake()
    {
        base.Start();
        m_slideBookByPos = GetComponentInChildren<SlideBookByPos>();
        m_contentTrans = GetComponentInChildren<ScrollRect>().content;

        RefreshUI();
    }

    public void NextPage()
    {
        uIFacade.PlayButtonAudio();
        m_slideBookByPos.PageChange(1);
    }

    public void LastPage()
    {
        uIFacade.PlayButtonAudio();
        m_slideBookByPos.PageChange(-1);
    }

    public override void Exit()
    {
        gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        // 初始化 UI 数据
        List<UI_LevelGroupData> levelGroupInfoList = LevelManager.GetInstance().GetStartLevelGroupInfoList();
        for (int i = 0; i < levelGroupInfoList.Count; i++)
        {
            GameObject levelGroupGo = FactoryManager.GetInstance().GetObject(ObjectFactoryType.UIFactory, "Level/Btn_LevelGroup");
            levelGroupGo.name = $"Btn_LevelGroup_{levelGroupInfoList[i].LevelGroupId}";
            // 设置关卡组
            Image levelGroupImage = levelGroupGo.transform.GetComponent<Image>();
            levelGroupImage.sprite = FactoryManager.GetInstance().GetSprite(StringManager.Sprite_LevelGroupFile + levelGroupInfoList[i].SpriteName);
            // 设置关卡组标题图片
            Image levelGroupTitleImage = levelGroupGo.transform.GetChild(0).GetComponent<Image>();
            levelGroupTitleImage.sprite = FactoryManager.GetInstance().GetSprite(StringManager.Sprite_LevelGroupFile + levelGroupInfoList[i].TitleSpriteName);
            // 设置按钮点击事件
            Button levelGroupBtn = levelGroupGo.transform.GetComponent<Button>();
            levelGroupBtn.onClick.RemoveAllListeners();
            // 必须得这样
            int levelGtoupId = levelGroupInfoList[i].LevelGroupId;
            levelGroupBtn.onClick.AddListener(() =>
            {
                OnLevelGroupBtn(levelGtoupId);
            });

            // 刷新关卡状态相关 UI
            RefreshLevelStateUI(PlayerManager.GetInstance().GetPlayerLevelGroupInfo(levelGroupInfoList[i].LevelGroupId).IsLocked, PlayerManager.GetInstance().GetUnlockedLevelCountInLevelGroup(levelGroupInfoList[i].LevelGroupId), levelGroupInfoList[i].BelongLevelCount, levelGroupGo.transform);

            levelGroupBtn.transform.SetParent(m_contentTrans);
        }
    }

    private void RefreshLevelStateUI(bool isLocked, int unlockedLevelCount, int levelAmount, Transform page)
    {
        page.Find("Img_Lock").gameObject.SetActive(isLocked);
        page.Find("Img_Page").gameObject.SetActive(!isLocked);
        if (isLocked == false)
        {
            page.Find("Img_Page/Text").GetComponent<Text>().text = unlockedLevelCount + "/" + levelAmount;
        }
        else
        {
            page.GetComponent<Button>().interactable = false;
        }
    }

    //点击了大关卡面板后进入对应的小关卡选择界面
    public void OnLevelGroupBtn(int levelGourpId)
    {
        uIFacade.PlayButtonAudio();
        // 被锁住就无法进入
        if (PlayerManager.GetInstance().GetPlayerLevelGroupInfo(levelGourpId).IsLocked)
        {
            return;
        }
        LevelManager.GetInstance().LevelGroupId = levelGourpId;
        Exit();
        uIFacade.IntoSmallLevel();
        uIFacade.EnterPanel(StringManager.NormalLevelPanel);
    }

}
