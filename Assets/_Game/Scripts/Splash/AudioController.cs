using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class AudioController : Singleton<AudioController>
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip run;
    [SerializeField] private AudioClip ground;
    [SerializeField] private AudioClip bossLaught;

    public bool AudioEnable { get; set; } = true;

    public enum SFX
    {
        Run,
        Ground,
        BossLOL
    }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetAudioEnable(bool value)
    {
        AudioEnable = value;
        if (AudioEnable)
            PlayBGM();
        else
            StopBGM();
    }

    public void PlayBGM()
    {
        if (!AudioEnable) return;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(SFX sfx)
    {
        if (!AudioEnable) return;
        switch (sfx)
        {
            case SFX.Ground:
                sfxSource.PlayOneShot(ground);
                break;
            case SFX.Run:
                sfxSource.PlayOneShot(run);
                break;
            case SFX.BossLOL:
                sfxSource.PlayOneShot(bossLaught);
                break;
        }
    }
}
