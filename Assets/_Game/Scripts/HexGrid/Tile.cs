using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace Axie
{
	public class Tile : MonoBehaviour
	{
		public CubeIndex Index;
		public AxieCharacter Axie;
		public GameObject background;
		
		[SerializeField] private Transform axieHolder;

		public bool IsEmpty => Axie == null;
		public AxieType AxieType => Axie.AxieType;

		public void SetAxie(AxieType axieType)
		{
			if (Axie != null && Axie.AxieType == axieType)
			{
				return; // DO NOTHING
			}
			
			ReturnPool();
			Axie = AxiePool.Instance.Pop(axieType);
			Axie.transform.SetParent(axieHolder);
			Axie.transform.localPosition = Vector3.zero;
			// Axie.transform.position = axieHolder.position;
			if (axieType == AxieType.Attacker)
				Axie.SetFlip(transform.position.x < 0);
			else
				Axie.SetFlip(transform.position.x > 0);
			Axie.AppearAnim();
			Axie.CubeIndex = Index;
			Axie.OnDead += OnAxieDead;
		}

		public void SetAxie(AxieCharacter axie)
		{
			if (Axie != null) return;
			Axie = axie;
			Axie.transform.SetParent(axieHolder);
			Axie.transform.DOLocalMove(Vector3.zero, 0.5f);
			Axie.OnDead += OnAxieDead;
			Axie.CubeIndex = Index;
		}

		public void ClearAxie()
		{
			var sq = DOTween.Sequence();
			if (Axie != null)
			{
				sq.Append(Axie.transform.DOScale(0, 0.3f));
			}
			sq.AppendCallback(ReturnPool);
		}

		private void OnAxieDead()
		{
			ReturnPool();
		}

		public void ReturnPool()
		{
			if (Axie != null)
			{
				Axie.OnDead -= OnAxieDead;
				AxiePool.Instance.Push(Axie);
				Axie = null;
			}
		}

		public void MoveAxieTo(Tile other)
		{
			if (Axie != null && other.IsEmpty)
			{
				other.SetAxie(Axie);
				Axie.OnDead -= OnAxieDead;
				Axie = null;
			}
		}

		public void AttackAxie(Tile other)
		{
			if (Axie != null && other.AxieType == AxieType.Defender)
			{
				Axie.Attack(other.Axie);
				other.Axie.Attack(Axie);
			}
		}
	}
}