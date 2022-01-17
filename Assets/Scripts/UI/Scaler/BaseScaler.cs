using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseScaler : MonoBehaviour
{
    public bool m_isFullScreen;

    private Canvas m_canvas;
    private CanvasScaler m_canvasScaler;
    // CanvasScaler 的默认分辨率
    private Vector2 m_referenceResolution = new Vector2(1024, 768);


    protected RectTransform m_rect;
    private Vector2 m_screenToRectRatio;
    public Vector2 m_baseSize;

    // 适配后的屏幕分辨率和基准分辨率比率
    private float m_matchedFormToScreenRation = 1.0f;

    private void Awake()
    {
        m_rect = gameObject.GetComponent<RectTransform>();
        // 应该为 1280 x 760
        m_baseSize = new Vector2(1024, 768);

        //m_canvas = gameObject.AddComponent<Canvas>();
        //m_canvasScaler = gameObject.AddComponent<CanvasScaler>();
    }

    // 分辨率改变时
    public virtual void MatchScreen()
    {
        /*

        //if(m_rect == null)
        //{
        //    return;
        //}

        //Vector2 screenSize = new Vector2(ScreenCache.Width, ScreenCache.Height);

        //m_screenToRectRatio.x = screenSize.x / m_baseSize.x;
        //m_screenToRectRatio.y = screenSize.y / m_baseSize.y;

        //if (m_screenToRectRatio.x > m_screenToRectRatio.y)
        //{
        //    // 全屏 bg 用上面
        //    if (m_isFullScreen)
        //    {
        //        m_matchedFormToScreenRation = m_screenToRectRatio.x;
        //    }
        //    else
        //    {
        //        m_matchedFormToScreenRation = m_screenToRectRatio.y;
        //    }
        //}
        //else
        //{
        //    if (m_isFullScreen)
        //    {
        //        m_matchedFormToScreenRation = m_screenToRectRatio.y;
        //    }
        //    else
        //    {
        //        m_matchedFormToScreenRation = m_screenToRectRatio.x;
        //    }
        //}

        //m_rect.sizeDelta = new Vector2(m_baseSize.x * m_matchedFormToScreenRation, m_baseSize.y * m_matchedFormToScreenRation);

        if (m_canvasScaler == null)
        {
            return;
        }

        Vector2 screenSize = new Vector2(ScreenCache.Width, ScreenCache.Height);

        m_canvasScaler.referenceResolution = m_referenceResolution;

        Vector2 m_screenToCanvasRatio = new Vector2();

        m_screenToCanvasRatio.x = screenSize.x / m_referenceResolution.x;
        m_screenToCanvasRatio.y = screenSize.y / m_referenceResolution.y;

        m_canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

        // 通常按高高比和宽宽比中高的那个进行适配，可以防止出屏
        // 对于全屏背景，为保证缩放比例，需要裁剪一部分，所以就按大的那个进行
        if (m_screenToCanvasRatio.x > m_screenToCanvasRatio.y)
        {
            // 全屏 bg 用上面
            if (m_isFullScreen)
            {
                m_canvasScaler.matchWidthOrHeight = 0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.x;
            }
            else
            {
                m_canvasScaler.matchWidthOrHeight = 1.0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.y;
            }
        }
        else
        {
            if (m_isFullScreen)
            {
                m_canvasScaler.matchWidthOrHeight = 1.0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.y;
            }
            else
            {
                m_canvasScaler.matchWidthOrHeight = 0f;
                m_matchedFormToScreenRation = m_screenToCanvasRatio.x;
            }
        }

        // 刷新一下 m_canvaScaler;
        m_canvasScaler.enabled = false;
        m_canvasScaler.enabled = true;

        */
    }
}
