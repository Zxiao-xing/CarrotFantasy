using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SlideBookByPos : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //改变content的Left值来实现翻书效果
    //每次翻一页
    ScrollRect scrollRect;
    RectTransform content;
    RectTransform canvasRect;
    int currentIndex = 0, itemCount;
    float mouseBeginPosx,
        offsetLeft,
        cellWidth,
        contentWidth,
        spacing,//单元格之间的距离
        oneItemDis,//移动一个单元格需要鼠标滑动的距离
        oneItemPosX,//移动一个单元content的LocalPositionX的值需要+-的值
        contentPosX = 0,//滑动鼠标后,要到达的LocalPositionX值
        initPosX;//初始时x坐标

    float contentLen;//记录content的原始长度
    [SerializeField] bool canSendMessage;
    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        initPosX = content.localPosition.x;
        canvasRect = GameObject.Find("Canvas").transform as RectTransform;

        itemCount = content.childCount;
        GridLayoutGroup gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        offsetLeft = gridLayoutGroup.padding.left;
        cellWidth = gridLayoutGroup.cellSize.x;
        contentWidth = content.rect.width;
        spacing = gridLayoutGroup.spacing.x;
        oneItemDis = cellWidth / 2 + offsetLeft;
        oneItemPosX = cellWidth + spacing;
        scrollRect.inertia = false;//必须禁止这个
        contentLen = content.sizeDelta.x;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, new Vector2
            (Input.mousePosition.x, Input.mousePosition.y), null, out pos);
        mouseBeginPosx = pos.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, new Vector2
            (Input.mousePosition.x, Input.mousePosition.y), null, out pos);
        float offsetX = mouseBeginPosx - pos.x;
        if (offsetX > 0)//右滑
            PageChange(1);
        else
            PageChange(-1);
    }

    /// <param name="currentIndexOffset"> 页码偏移量(单滑,只有-1与1) 1:向右滑动  -1:向左滑动</param>
    public void PageChange(int currentIndexOffset)
    {
        currentIndex += currentIndexOffset;
        if (currentIndex > (itemCount - 1))
        {
            currentIndex--;
            return;
        }
        if (currentIndex < 0)
        {
            currentIndex++;
            return;
        }
        AudioManager.GetInstance().PlayEffAudio("Main/Paging");
        float offsetPos = -currentIndexOffset * oneItemPosX;
        contentPosX += offsetPos;
        content.DOLocalMoveX(contentPosX, 0.5f).SetEase(Ease.OutQuint);
        if (canSendMessage)
        {
            gameObject.SendMessageUpwards("RefreshUI", currentIndexOffset);
        }
    }

    public void UpdateContentLength(int itemNum)
    {
        content.sizeDelta = new Vector2(contentLen + (cellWidth + spacing) * (itemNum - 1), content.sizeDelta.y);
        itemCount = itemNum;
    }

    public void ResetPos()
    {
        currentIndex = 0;
        contentPosX = 0;
        content.localPosition = new Vector2(initPosX, content.localPosition.y);
    }
}
