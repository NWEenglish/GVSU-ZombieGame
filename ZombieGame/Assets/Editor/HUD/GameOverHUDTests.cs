using Assets.Scripts.HUD;
using NUnit.Framework;
using TMPro;

namespace Assets.Editor.HUD
{
    public class GameOverHUDTests
    {
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(10)]
        [TestCase(0)]
        public void ShowGameOverTests(int roundsSurvived)
        {
            // Arrange
            TextMeshProUGUI gameOverTitle = new();
            TextMeshProUGUI gameOverWave = new();

            GameOverHUD gameOverHUD = new()
            {
                GameOverTitle = gameOverTitle,
                GameOverSubtext = gameOverWave
            };

            // Act
            gameOverHUD.ShowZombiesGameOver(roundsSurvived);

            // Assert
            Assert.That(gameOverWave.text.Contains(roundsSurvived.ToString()));
        }
    }
}
