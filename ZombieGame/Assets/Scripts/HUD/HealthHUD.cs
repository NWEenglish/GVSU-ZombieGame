using Assets.Scripts.Helpers;
using UnityEngine.UI;

namespace Assets.Scripts.HUD
{
    public class HealthHUD
    {
        private readonly Image Panel;
        private readonly BlinkHelper Blinker;
        private readonly int MaxHealth;

        public HealthHUD(Image healthPanel, BlinkHelper blinker, int maxHealth)
        {
            Panel = healthPanel;
            Blinker = blinker;
            MaxHealth = maxHealth;
        }

        public void UpdateHUD(int health)
        {
            UpdatePanel(health);
            Blinker.TryEnd();
        }

        public void DisplayHit()
        {
            Blinker.Start();
        }

        private void UpdatePanel(int health)
        {
            var alpha = 1 - ((float)health / MaxHealth);
            alpha = alpha > 0.8f ? 0.8f : alpha;

            Panel.color = new UnityEngine.Color(1, 0, 0, alpha);
        }
    }
}
