using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager 
{
    AudioSource[] audioSource;//0播放BG   1播放特效
    bool isPlayBG = true;
    bool isPlayEffAudioSource = true;

    AudioClip btnClip;//按钮点击音效很常用,所以特意封装一下

    public AudioManager()
    {
        audioSource = GameManager._Ins.GetComponents<AudioSource>();
        btnClip = GameManager._Ins.factoryManager.GetAudioClip("Main/Button");
    }

    public void PlayBG(string clipPath)
    {
        AudioClip clip = GetAudioClip(clipPath);
        if(audioSource[0].isPlaying==false||audioSource[0].clip!=clip)
        {
            audioSource[0].clip = clip;
            audioSource[0].Play();
        }
    }

    public void PlayEffAudio(string clipPath)
    {
        AudioClip clip = GetAudioClip(clipPath);
        if (isPlayEffAudioSource)
            audioSource[1].PlayOneShot(clip);
    }

    public bool BGSwitch()
    {
        isPlayBG = !isPlayBG;
        if(isPlayBG)
            audioSource[0].Play();
        else
            audioSource[0].Pause();
        return isPlayBG;
    }

    public bool EffAudioSwitch()
    {
        isPlayEffAudioSource = !isPlayEffAudioSource;
        return isPlayEffAudioSource;
    }

    AudioClip GetAudioClip(string clipPath)
    {
        return GameManager._Ins.factoryManager.GetAudioClip(clipPath);
    }

    public void PlayButtonAudio()
    {
        if(isPlayEffAudioSource)
        audioSource[1].PlayOneShot(btnClip);
    }

}
