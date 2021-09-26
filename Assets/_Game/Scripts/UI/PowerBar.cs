using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Axie
{
    public class PowerBar : MonoBehaviour
    {
        [SerializeField] private RectTransform attackerRect;
        [SerializeField] private RectTransform defenderRect;
        [SerializeField] private TMP_Text attackPower;
        [SerializeField] private TMP_Text defensePower;
        
        public void UpdateBar(List<AxieCharacter> attackers, List<AxieCharacter> defenders)
        {
            var attackValue = attackers.Sum(x => x.HP);
            var defenseValue = defenders.Sum(x => x.HP);
            attackPower.text = attackValue.ToString();
            defensePower.text = defenseValue.ToString();

            var attackRatio = (float) attackValue / (attackValue + defenseValue);
            attackerRect.DOAnchorMax(new Vector2(attackRatio, 1), 0.2f);
            defenderRect.DOAnchorMin(new Vector2(attackRatio, 0), 0.2f);
        }
    }
}