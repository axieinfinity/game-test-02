using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Sprite onSpr, offSpr;

    private void OnEnable()
    {
        UpdateView();
    }

    void UpdateView()
    {
        icon.sprite = AudioController.Instance.AudioEnable ? onSpr : offSpr;
    }

    public void OnClickButton()
    {
        if (AudioController.Instance == null)
            return;
        
        AudioController.Instance.SetAudioEnable(!AudioController.Instance.AudioEnable);
        UpdateView();
    }
}
