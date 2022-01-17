using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipFactory : IResourceFactory<AudioClip>
{
    Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();
    string path = "AudioClips/";
    public AudioClip GetResource(string resourcePath)
    {
        AudioClip clip;
        if (audioClipDict.ContainsKey(resourcePath))
            clip = audioClipDict[resourcePath];
        else
            clip = Resources.Load<AudioClip>(path + resourcePath);
        if (clip == null)
            Debug.LogWarning("资源路径出错: " + path + resourcePath);
        return clip;
    }
}
