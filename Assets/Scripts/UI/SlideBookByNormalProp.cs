using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SlideBookByNormalProp : MonoBehaviour,IBeginDragHandler,IEndDragHandler
{
    //改变Scrol View的horizontalNormalizedPosition来实现翻书的效果
    //一次可以翻多页
    RectTransform canvasRect;//用于屏幕左边转为世界坐标
        ScrollRect scrollRect;

    float contentWidth,//Content的总长度 
        offseLeft,//Scrol View的左偏移量
        cellWidth,//每一个Content包含的物体的单元的宽度
        spacing,//每一个单元的间隔
        oneItemNormalPos,//一个单元格所占的比例
        oneItemPos,//翻一个单元格鼠标需要移动的距离
        nextItemPos;//翻一个单元后再翻一个需要的移动距离

    int itemCount;//单元格个数    
    int currentIndex;//现在视口看见的是哪一个单元格(防止越界)

    float mouseBeginX;//鼠标开始拖拽的位置

    [SerializeField]
    Text pageTxt;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null, out pos);
        mouseBeginX = pos.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 pos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect,new Vector2(Input.mousePosition.x,Input.mousePosition.y),null,out pos);
        float mouseOffset = mouseBeginX - pos.x;
        if(Mathf.Abs(mouseOffset)>oneItemPos-200)
        {
            if(mouseOffset>0)//右滑动
            {
                if (currentIndex >= itemCount-1)
                    return;
                int count = (int)((mouseOffset - oneItemPos) / nextItemPos) + 1;
                currentIndex += count;
                if (currentIndex > itemCount - 1)
                    currentIndex = itemCount - 1;
                /*每次只能滑动一个单元*/
                //currentIndex++;
                //moveNormalPos += oneItemNormalPos;
            }
            else
            {
                if (currentIndex <= 0)
                    return;
                int count = (int)((mouseOffset + oneItemPos) / nextItemPos) - 1;
                currentIndex += count;
                if (currentIndex < 0)
                    currentIndex = 0;

                /*每次只能滑动一个单元*/
                //currentIndex--;
                //moveNormalPos -= oneItemNormalPos;
            }
        }
        DOTween.To(() => scrollRect.horizontalNormalizedPosition, lerpValue => 
        scrollRect.horizontalNormalizedPosition = lerpValue,
       currentIndex/(float)(itemCount-1), 0.5f).SetEase(Ease.OutQuint);
        GameManager._Ins.audioManager.PlayEffAudio("Main/Paging");
        if (pageTxt != null)
            pageTxt.text = (currentIndex + 1) + "/" + itemCount;
        //scrollRect.DOHorizontalNormalizedPos(currentIndex / (float)(itemCount - 1), 0.5f).SetEase(Ease.OutQuint);
    }

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        GridLayoutGroup gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        offseLeft = gridLayoutGroup.padding.left;
        cellWidth = gridLayoutGroup.cellSize.x;
        contentWidth = scrollRect.content.rect.width ;
        spacing = gridLayoutGroup.spacing.x;
        itemCount = transform.Find("Viewport/Content").childCount;
        canvasRect = GameObject.Find("Canvas").transform as RectTransform;
        oneItemPos = cellWidth / 2 + offseLeft;
        oneItemNormalPos = (cellWidth + spacing) / contentWidth;
        nextItemPos = cellWidth + spacing;
        scrollRect.horizontalNormalizedPosition = 0;
        currentIndex = 0;
        Init();
    }

    public void Init()
    {
        currentIndex = 0;
        scrollRect.horizontalNormalizedPosition = 0;
        if (pageTxt != null)
            pageTxt.text = (currentIndex + 1) + "/" + itemCount;
    }

}
