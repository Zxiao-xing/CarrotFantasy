using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 屏幕大小缓存
public class ScreenCache
{
    public static int Width { get; private set; }
    public static int Height { get; private set; }

    static ScreenCache()
    {
        // 先缓存一遍
        Cache();
    }

    // 缓存的屏幕大小，返回值表示屏幕分辨率是否发生变化
    public static bool Cache()
    {
#if UNITY_EDITOR
        Vector2 viewSize = UnityEditor.Handles.GetMainGameViewSize();
        int width = (int)viewSize.x;
        int height = (int)viewSize.y;
#else
        int width = Screen.width;
        int height = Screen.height;
#endif

        if (width == Width && height == Height)
        {
            return false;
        }

        Width = width;
        Height = height;
        return true;
    }

}
