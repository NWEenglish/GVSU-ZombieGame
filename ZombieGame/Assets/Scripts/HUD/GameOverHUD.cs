using TMPro;
using UnityEngine;

namespace Assets.Scripts.HUD
{
    public class GameOverHUD
    {
        public TextMeshProUGUI GameOverTitle;
        public TextMeshProUGUI GameOverWave;

        public void ShowGameOver(int roundsSurvived)
        {
            GameOverTitle.color = Color.white;
            GameOverWave.color = Color.white;

            GameOverWave.text = $"You Survived {roundsSurvived} Rounds";
        }
    }
}
