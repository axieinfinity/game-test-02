using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using Spine.Unity.Modules;
using Spine.Unity.Playables;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [SerializeField] private BoneFollower boneFollower;
        [SerializeField] private Collider2D collider;
        [SerializeField] private SkeletonRendererCustomMaterials customMaterial;
        [SerializeField] private ParticleSystem explosiveEffect;
        public CubeIndex CubeIndex;
        #endregion

        public event Action OnDead;
        
        #region Properties
        public AxieType AxieType => property.AxieType;
        public int Level { get; set; } = 1;
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int TotalDamage { get; set; }
        public int RandomNumber { get; set; }
        public AxieProperty AxieProperty => property;
        #endregion

        private void OnEnable()
        {
            HP = property.StartingHP;
            MaxHP = HP;
            hpBar.Setup(HP);
            RandomNumber = Random.Range(0, 3);
        }

        public void SetSkeletonEnable(bool value)
        {
            animationHandler.skeletonAnimation.enabled = value;
            boneFollower.enabled = value;
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
                var damageGiven = GetDamageGiven(other);
                TotalDamage += damageGiven;
                other.TakeDame(damageGiven);
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
            if (mainCam.orthographicSize >= 25) return false;
            
            var plans = GeometryUtility.CalculateFrustumPlanes(mainCam);
            return GeometryUtility.TestPlanesAABB(plans, collider.bounds);
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
                UIController.Instance.ShowAxieInfo(this);
        }

        private void Update()
        {
            SetSkeletonEnable(IsVisible());
        }

        public void SetCustomMaterialActive(bool value)
        {
            customMaterial.enabled = value;
        }

        public void Upgrade()
        {
            Level += 1;
            HP = property.StartingHP * 10;
            MaxHP = HP;
            hpBar.Setup(HP);
            transform.DOScale(2.5f, 0.5f);
            RandomNumber = Random.Range(0, 3);
        }

        public void Explosive()
        {
            if (explosiveEffect != null)
            {
                var effect = Instantiate(explosiveEffect, transform.position, quaternion.identity);
                effect.transform.localScale *= Level;
            }
            TakeDame(HP);
        }

        public void PlayWinAnim()
        {
            animationHandler.PlayAnimationForState($"win{Random.Range(1,4)}", 0, true);
        }
    }
}