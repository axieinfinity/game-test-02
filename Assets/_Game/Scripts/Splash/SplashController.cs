using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Axie
{
    public class SplashController : MonoBehaviour
    {
        [SerializeField] private Transform mainCamTransform;
        [SerializeField] private Transform bigDefender;
        [SerializeField] private SkeletonAnimation[] attackers;
        [SerializeField] private CanvasGroup splash;

        private IEnumerator Start()
        {
            AudioController.Instance.StopBGM();
            AudioController.Instance.PlaySFX(AudioController.SFX.Run);
            yield return mainCamTransform.DOMoveX(0, 3f).WaitForCompletion();
            
            AudioController.Instance.PlaySFX(AudioController.SFX.Ground);
            yield return bigDefender.DOMoveY(-2, 0.5f).WaitForCompletion();
            
            yield return new WaitForSeconds(1f);
            AudioController.Instance.PlaySFX(AudioController.SFX.BossLOL);
            foreach (var attacker in attackers)
            {
                yield return null;
                attacker.Skeleton.FlipX = false;
                attacker.transform.DOMoveX(-15, 6f);
            }
            yield return new WaitForSeconds(3f);
            AudioController.Instance.PlayBGM();
            yield return splash.DOFade(1f, 1f).WaitForCompletion();
            yield return new WaitForSeconds(2f);
            LoadGameScene();
        }

        void LoadGameScene()
        {
            SceneManager.LoadSceneAsync(Constants.Scene.GAME);
        }
    }
    
}
