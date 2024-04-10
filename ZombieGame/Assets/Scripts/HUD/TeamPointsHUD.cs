using TMPro;

namespace Assets.Scripts.HUD
{
    public class TeamPointsHUD
    {
        private readonly TextMeshProUGUI HUDText;

        public TeamPointsHUD(TextMeshProUGUI pointsHUD)
        {
            HUDText = pointsHUD;
        }

        public void UpdateHUD(int friendlyTeamPoints, int hostileTeamPoints, int targetPoints)
        {
            string gameStatus;
            if (friendlyTeamPoints > hostileTeamPoints)
            {
                gameStatus = "Winning";
            }
            else if (friendlyTeamPoints < hostileTeamPoints)
            {
                gameStatus = "Losing";
            }
            else
            {
                gameStatus = "Tie";
            }

            var text = $"{gameStatus}\n{friendlyTeamPoints} / {targetPoints}\n{hostileTeamPoints} / {targetPoints}";

            if (HUDText.text != text)
            {
                HUDText.text = text;
            }
        }
    }
}
