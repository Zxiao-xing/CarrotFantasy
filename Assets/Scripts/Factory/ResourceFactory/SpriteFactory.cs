using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFactory : IResourceFactory<Sprite>
{
    Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();
    string path = "Pictures/";
    public Sprite GetResource(string resourcePath)
    {
        Sprite sprite;
        if (spriteDict.ContainsKey(resourcePath))
            sprite = spriteDict[resourcePath];
        else
            sprite = Resources.Load<Sprite>(path + resourcePath);
        if (sprite == null)
            Debug.LogWarning("资源路径出错: " + path + resourcePath);
        return sprite;
    }
}
