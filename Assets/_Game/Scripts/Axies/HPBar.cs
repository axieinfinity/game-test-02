using DG.Tweening;
using UnityEngine;

namespace Axie
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Transform scaler;
        [SerializeField] private SpriteRenderer hpRenderer;
        [SerializeField] private Color normalHPColor;
        [SerializeField] private Color lowHPColor;

        private int maxHP;
        private int currentHP;
        
        public void Setup(int maxHP)
        {
            this.maxHP = maxHP;
            scaler.localScale = Vector3.one;
        }

        public void UpdateHP(int currentHP)
        {
            var ratio = (float)currentHP / maxHP; 
            scaler.DOScaleX(ratio, 0.3f);
            hpRenderer.color = ratio > Constants.GameLogic.LOW_HP ? normalHPColor : lowHPColor;
        }
    }
}