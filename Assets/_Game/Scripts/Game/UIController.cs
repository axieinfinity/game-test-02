using UnityEngine;
using Utils;

namespace Axie
{
    public class UIController : Singleton<UIController>
    {
        [SerializeField] private PauseDialog pauseDialog;

        public void OnClickPauseButton()
        {
            pauseDialog.Show();
        }
    }
}