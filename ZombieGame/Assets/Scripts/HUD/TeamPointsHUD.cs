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
                gameStatus = "<b><color=green>Winning</color></b>";
            }
            else if (friendlyTeamPoints < hostileTeamPoints)
            {
                gameStatus = "<b><color=red>Losing</color></b>";
            }
            else
            {
                gameStatus = "<b><color=orange>Tie</color></b>";
            }

            var text = $"{gameStatus}\n{friendlyTeamPoints} / {targetPoints}\n{hostileTeamPoints} / {targetPoints}";

            if (HUDText.text != text)
            {
                HUDText.text = text;
            }
        }
    }
}
