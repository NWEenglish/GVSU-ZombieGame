using Assets.Scripts.Constants.Types;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    public class GameOverHUD
    {
        public TextMeshProUGUI GameOverTitle;
        public TextMeshProUGUI GameOverSubtext;

        public void ShowZombiesGameOver(int roundsSurvived)
        {
            GameOverTitle.color = Color.white;
            GameOverSubtext.color = Color.white;

            GameOverSubtext.text = $"You Survived {roundsSurvived} Rounds";
        }

        public void ShowNonZombiesGameOver(GameOutcome outcome)
        {
            GameOverTitle.color = Color.white;
            GameOverSubtext.color = Color.white;

            if (outcome == GameOutcome.Won)
            {
                GameOverTitle.text = "VICTORY!";
                GameOverSubtext.text = "You're work is appreciated\nWell done";
            }
            else if (outcome == GameOutcome.Lost)
            {
                GameOverTitle.text = "DEFEAT!";
                GameOverSubtext.text = "Mission failed\nWe'll get them next time";
            }
            else
            {
                GameOverTitle.text = "It's a draw\nStand-down";
                GameOverSubtext.text = "Regroup and try again";
            }
        }
    }
}
