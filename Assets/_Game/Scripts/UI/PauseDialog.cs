using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Axie
{
    public class PauseDialog : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image blackImage;
        [SerializeField] private Slider timeSlider;
        [SerializeField] private TMP_Text timeScaleValue;

        private float timeScale;
        private bool isShow;
        
        public void Show()
        {
            if (isShow) return;
            isShow = true;
            
            gameObject.SetActive(true);
            timeSlider.value = Time.timeScale;
            UpdateTimeScale(Time.timeScale);

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1f, 0.3f).OnComplete(() =>
            {
                Time.timeScale = 0;
            });
        }

        public void Hide()
        {
            Time.timeScale = timeScale;
            canvasGroup.DOFade(0, 0.5f).OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                isShow = false;
                gameObject.SetActive(false);
            });
        }

        public void OnSliderChanged(float value)
        {
            UpdateTimeScale(value);
        }

        private void UpdateTimeScale(float value)
        {
            timeScale = value;
            timeScaleValue.text = $"x{timeScale:F1}";
        }
        

        public void OnClickContinue()
        {
            Hide();
        }

        public void OnClickRestart()
        {
            Hide();
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync(Constants.Scene.SPLASH);
        }
    }
}