using System;
using TMPro;
using UnityEngine;
using Utils;

namespace Axie
{
    public class UIController : Singleton<UIController>
    {
        [SerializeField] private PauseDialog pauseDialog;
        [SerializeField] private TMP_Text fpsLabel;
        [SerializeField] private AxieInfoDialog axieInfoDialog;

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
    }
}