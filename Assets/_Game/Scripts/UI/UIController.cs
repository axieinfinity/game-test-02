using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Axie
{
    public class UIController : Singleton<UIController>
    {
        [SerializeField] private CanvasGroup splash;
        [SerializeField] private PauseDialog pauseDialog;
        [SerializeField] private TMP_Text fpsLabel;
        [SerializeField] private AxieInfoDialog axieInfoDialog;
        [SerializeField] private EndGameUI endGameUI;

        private void Start()
        {
            splash.alpha = 1;
            splash.DOFade(0, 1f);
        }

        public void OnClickPauseButton()
        {
            pauseDialog.Show();
        }

        private void Update()
        {
            fpsLabel.text = $"FPS\n{FPSCounter.Instance.FPS:F2}";
        }

        public void ShowAxieInfo(AxieCharacter axie)
        {
            axieInfoDialog.Show(axie);
        }

        public void ShowEndGameUI(AxieType winTeam)
        {
            endGameUI.Show(winTeam);
        }
    }
}