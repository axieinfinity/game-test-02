using UnityEngine;

namespace Axie
{
    [CreateAssetMenu(fileName = "AxieProperty", menuName = "AxieProperty", order = 0)]
    public class AxieProperty : ScriptableObject
    {
        public AxieType AxieType;
        public int StartingHP;
    }
}