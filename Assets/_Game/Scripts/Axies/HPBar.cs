using DG.Tweening;
using UnityEngine;

namespace Axie
{
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Transform scaler;

        private int maxHP;
        private int currentHP;
        
        public void Setup(int maxHP)
        {
            this.maxHP = maxHP;
            scaler.localScale = Vector3.one;
        }

        public void UpdateHP(int currentHP)
        {
            scaler.DOScaleX((float)currentHP / maxHP, 0.3f);
        }
    }
}