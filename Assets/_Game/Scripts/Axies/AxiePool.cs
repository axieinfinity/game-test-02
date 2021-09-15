using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Axie
{
    public class AxiePool : Singleton<AxiePool>
    {
        [SerializeField] private AxieCharacter attackerPrefab;
        [SerializeField] private AxieCharacter defenderPrefab;
        [SerializeField] private int initCapacity = 10;

        private Stack<AxieCharacter> attackerPool = new Stack<AxieCharacter>();
        private Stack<AxieCharacter> defenderPool = new Stack<AxieCharacter>();
        
        private void Start()
        {   
            Populate(attackerPrefab, attackerPool, initCapacity);
            Populate(defenderPrefab, defenderPool, initCapacity);
        }

        public void Populate<T>(T prefab, Stack<T> stack, int count) where T : Component
        {
            for (int i = 0; i < count; i++)
            {
                var item = Instantiate(prefab, transform, false);
                item.gameObject.SetActive(false);
                stack.Push(item);
            }
        }

        public AxieCharacter Pop(AxieType axieType)
        {
            AxieCharacter result = null;
            Stack<AxieCharacter> stack = axieType == AxieType.Attacker ? attackerPool : defenderPool;

            if (stack.Count > 0)
            {
                result = stack.Pop();
            }

            if (result == null)
            {
                var prefab = axieType == AxieType.Attacker ? attackerPrefab : defenderPrefab;
                result = Instantiate(prefab, transform, false);
                // Populate(prefab, stack, initCapacity);
            }
            
            result.gameObject.SetActive(true);
            return result;
        }

        public void Push(AxieCharacter axieCharacter)
        {
            Stack<AxieCharacter> stack = axieCharacter.AxieType == AxieType.Attacker ? attackerPool : defenderPool;
            stack.Push(axieCharacter);
            axieCharacter.transform.SetParent(transform);
            axieCharacter.gameObject.SetActive(false);
        }
    }
}