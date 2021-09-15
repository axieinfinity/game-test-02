using System;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;

namespace Axie
{
    public class AxieInfoDialog : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransfrom;
        [SerializeField] private float showPosX, hidePosX;

        [SerializeField] private SkeletonGraphic skeletonGraphic;
        [SerializeField] private TMP_Text teamLabel;
        [SerializeField] private TMP_Text hpLabel;
        [SerializeField] private TMP_Text damageLabel;
        [SerializeField] private TMP_Text randomNumberLabel;

        private AxieType axieType = AxieType.Attacker;
        private AxieCharacter axieCharacter;

        public void Show(AxieCharacter axie)
        {
            if (axie.AxieType != axieType)
            {
                skeletonGraphic.skeletonDataAsset = axie.AxieProperty.SkeletonDataAsset;
                skeletonGraphic.Initialize(true);
                axieType = axie.AxieType;
            }

            if (axieCharacter != null)
            {
                axieCharacter.OnDead -= Hide;
                axieCharacter.SetCustomMaterialActive(false);
            }
            axieCharacter = axie;
            axieCharacter.SetCustomMaterialActive(true);
            
            rectTransfrom.DOAnchorPosX(showPosX, 0.3f);
            canvasGroup.DOFade(1f, 0.3f);

            UpdateInfo();

            axieCharacter.OnDead += Hide;
        }

        void UpdateInfo()
        {
            if (axieCharacter != null)
            {
                teamLabel.text = axieCharacter.AxieType.ToString();
                hpLabel.text = $"HP: {axieCharacter.HP}/{axieCharacter.AxieProperty.StartingHP}";
                damageLabel.text = $"Total Damage: {axieCharacter.TotalDamage}";    
                randomNumberLabel.text = $"Random Number: {axieCharacter.RandomNumber}";
            }
        }

        private void Update()
        {
            UpdateInfo();
        }

        public void Hide()
        {
            rectTransfrom.DOAnchorPosX(hidePosX, 0.3f);
            canvasGroup.DOFade(0f, 0.3f);
            if (axieCharacter != null)
            {
                axieCharacter.OnDead -= Hide;
                axieCharacter.SetCustomMaterialActive(false);
                axieCharacter = null;
            }
        }
    }
}