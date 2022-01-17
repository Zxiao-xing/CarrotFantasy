﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NormalBigLevelPanel : BasePanel
{
    SlideBookByPos m_slideBookByPos;
    PlayerManager m_playerManager;
    Transform m_contentTrans;

    protected void Awake()
    {
        base.Start();
        m_playerManager = uIFacade.playerManager;
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
            GameObject levelGroupGo = GameManager._Ins.factoryManager.GetObject(ObjectFactoryType.UIFactory, "Level/Btn_LevelGroup");
            levelGroupGo.name = $"Btn_LevelGroup_{levelGroupInfoList[i].LevelGroupId}";
            // 设置关卡组
            Image levelGroupImage = levelGroupGo.transform.GetComponent<Image>();
            levelGroupImage.sprite = GameManager._Ins.factoryManager.GetSprite(StringManager.Sprite_LevelGroupFile + levelGroupInfoList[i].SpriteName);
            // 设置关卡组标题图片
            Image levelGroupTitleImage = levelGroupGo.transform.GetChild(0).GetComponent<Image>();
            levelGroupTitleImage.sprite = GameManager._Ins.factoryManager.GetSprite(StringManager.Sprite_LevelGroupFile + levelGroupInfoList[i].TitleSpriteName);
            // 设置按钮点击事件
            Button levelGroupBtn = levelGroupGo.transform.GetComponent<Button>();
            levelGroupBtn.onClick.RemoveAllListeners();
            // 必须得这样
            uint levelGtoupId = levelGroupInfoList[i].LevelGroupId;
            levelGroupBtn.onClick.AddListener(() =>
            {
                OnLevelGroupBtn(levelGtoupId);
            });

            // 刷新关卡状态相关 UI
            RefreshLevelStateUI(m_playerManager.GetPlayerLevelGroupInfo(levelGroupInfoList[i].LevelGroupId).IsLocked, m_playerManager.GetUnlockedLevelCountInLevelGroup(levelGroupInfoList[i].LevelGroupId), levelGroupInfoList[i].BelongLevelCount, levelGroupGo.transform);

            levelGroupBtn.transform.parent = m_contentTrans;
        }
    }

    private void RefreshLevelStateUI(bool isLocked, int unlockedLevelCount, uint levelAmount, Transform page)
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
    public void OnLevelGroupBtn(uint levelGourpId)
    {
        uIFacade.PlayButtonAudio();
        // 被锁住就无法进入
        if (m_playerManager.GetPlayerLevelGroupInfo(levelGourpId).IsLocked)
        {
            return;
        }
        LevelManager.GetInstance().LevelGroupId = levelGourpId;
        Exit();
        uIFacade.IntoSmallLevel();
        uIFacade.EnterPanel(StringManager.NormalLevelPanel);
    }

}
