using TMPro;

namespace Assets.Scripts.HUD
{
    public class PointsHUD
    {
        private readonly TextMeshProUGUI HUDText;

        public PointsHUD(TextMeshProUGUI pointsHUD)
        {
            HUDText = pointsHUD;
        }

        public void UpdateHUD(int points)
        {
            var text = $"Points: {points}";

            if (HUDText.text != text)
            {
                HUDText.text = text;
            }
        }
    }
}
