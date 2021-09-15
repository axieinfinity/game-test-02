using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity.Playables;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Axie
{
    public enum AxieType
    {
        Attacker,
        Defender
    }
    
    public class AxieCharacter : MonoBehaviour, ITakeDamage
    {
        #region Fields
        [SerializeField] protected AxieProperty property;
        [SerializeField] private HPBar hpBar;
        [SerializeField] private SkeletonAnimationStateHandler animationHandler;
        [SerializeField] private Collider2D collider;
        #endregion

        public event Action OnDead;
        public int RandomNumber { get; set; }
        
        #region Properties
        public AxieType AxieType => property.AxieType;
        public int HP { get; set; }
        #endregion

        private void OnEnable()
        {
            HP = property.StartingHP;
            hpBar.Setup(HP);
            RandomNumber = Random.Range(0, 3);
        }

        public void SetSkeletonEnable(bool value)
        {
            animationHandler.skeletonAnimation.enabled = value;
        }

        public void SetFlip(bool isFacingRight)
        {
            animationHandler.SetFlip(isFacingRight);
        }

        public void TakeDame(int damage)
        {
            HP -= damage;
            HP = Mathf.Max(HP, 0);
            
            hpBar.UpdateHP(HP);
            if (HP <= 0)
            {
                // OnDead?.Invoke();
                Die();
            }
        }

        private int GetDamageGiven(AxieCharacter other)
        {
            var rand = (3 + RandomNumber - other.RandomNumber) % 3;
            if (rand == 0) return 4;
            if (rand == 1) return 5;
            if (rand == 2) return 3;
            return 0;
        }

        private void Die()
        {
            attackSeq?.Kill();
            animationHandler.PlayAnimationForState("die", 0, false);
            var sq = DOTween.Sequence();
            sq.AppendInterval(0.5f);
            sq.AppendCallback(() =>
            {
                OnDead?.Invoke();
            });
        }

        private Sequence attackSeq = null;
        public void Attack(AxieCharacter other)
        {
            attackSeq = DOTween.Sequence();
            if (AxieType == AxieType.Attacker)
            {
                var targetPos = other.transform.position;
                targetPos.x = (transform.position.x + other.transform.position.x) * 0.5f;
                attackSeq.Append(transform.DOMove(targetPos, 0.3f));
            }
            else
            {
                attackSeq.AppendInterval(0.3f);
            }

            attackSeq.AppendCallback(() =>
            {
                animationHandler.PlayAnimationForState($"attack{Random.Range(1, 4)}", 0, false);
            });
            
            attackSeq.AppendInterval(0.7f);
            
            attackSeq.AppendCallback(() =>
            {
                other.TakeDame(GetDamageGiven(other));
            });
            
            if (AxieType == AxieType.Attacker)
            {
                attackSeq.Append(transform.DOLocalMove(Vector3.zero, 0.3f));
            }
            
            attackSeq.AppendCallback(() =>
            {
                animationHandler.PlayAnimationForState("idle", 0, true);
            });
        }

        public void AppearAnim()
        {
            animationHandler.PlayAnimationForState("appear", 0, false);
            transform.localScale = Vector3.zero;
            var sq = DOTween.Sequence();
            sq.Append(transform.DOScale(1f, 0.35f));
            sq.AppendInterval(1f);
            sq.AppendCallback(() =>
            {
                animationHandler.PlayAnimationForState("idle", 0, true);
            });
        }

        Camera mainCam = null;
        bool IsVisible()
        {
            if (mainCam == null) mainCam = Camera.main;
            
            var plans = GeometryUtility.CalculateFrustumPlanes(mainCam);
            return GeometryUtility.TestPlanesAABB(plans, collider.bounds);
        }

        public void Update()
        {
            SetSkeletonEnable(IsVisible());
        }
    }
}