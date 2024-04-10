using Assets.Scripts.Helpers;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.Scripts.HUD
{
    public class RespawnHUD
    {
        private readonly Image Panel;
        private readonly BlinkHelper Blinker;

        public RespawnHUD(Image healthPanel, BlinkHelper blinker)
        {
            Panel = healthPanel;
            Blinker = blinker;
        }

        //public void UpdateHUD(int health)
        //{
        //    Blinker.TryEnd();
        //}

        public void TriggerStart()
        {
            Blinker.Start();
        }

        public void TriggerEnd()
        {
            Blinker.TryEnd();
        }
    }
}
