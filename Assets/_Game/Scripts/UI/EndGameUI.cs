using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Axie
{
    public class EndGameUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text resultLabel;
        [SerializeField] private ParticleSystem effect;

        private bool isShow;
        
        public void Show(AxieType winTeam)
        {
            Time.timeScale = 1f;
            if (isShow) return;
            isShow = true;
            
            gameObject.SetActive(true);
            
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1f, 0.3f);

            resultLabel.text = $"{winTeam.ToString()} TEAM WIN";
            
            effect.gameObject.SetActive(true);
            effect.Play();
        }
        
        public void OnClickRestart()
        {
            SceneManager.LoadSceneAsync(Constants.Scene.SPLASH);
        }
    }
}