using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace Axie
{
    public class MessageUI : Singleton<MessageUI>
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransfrom;
        [SerializeField] private float showPosY, hidePosY;
        [SerializeField] private TMP_Text messageLabel;
        
        public void Show(string message, float hideDelay = 3f)
        {
            messageLabel.text = message;
            canvasGroup.DOFade(1f, 0.3f);
            rectTransfrom.DOAnchorPosY(showPosY, 0.3f);
            var sq = DOTween.Sequence();
            sq.AppendInterval(hideDelay);
            sq.AppendCallback(Hide);
        }

        public void Hide()
        {
            canvasGroup.DOFade(0f, 0.3f);
            rectTransfrom.DOAnchorPosY(hidePosY, 0.3f);
        }
    }
}