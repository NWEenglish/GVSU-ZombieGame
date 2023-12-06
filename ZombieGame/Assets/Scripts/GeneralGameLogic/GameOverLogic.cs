using Assets.Scripts.Constants.Names;
using Assets.Scripts.Extensions;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.GeneralGameLogic
{
    public class GameOverLogic : MonoBehaviour
    {
        private readonly Logger _logger = Logger.GetLogger();

        private bool InGameOver = false;
        private PlayerLogic Player;
        private AudioSource GameOverMusic;

        private void Start()
        {
            Player = GameObject.Find(ObjectNames.Player).GetComponent<PlayerLogic>();
            GameOverMusic = gameObject.GetComponents<AudioSource>()[2];
        }

        private void Update()
        {
            if (Player.Status.IsPlayerDead && !InGameOver)
            {
                _logger.LogDebug("Player has died. Beginning game over sequence.");
                InGameOver = true;
                GameOverMusic.TryPlay();
            }
            else if (Player.Status.IsPlayerDead && !GameOverMusic.isPlaying)
            {
                _logger.LogDebug("Game over sequence completed. Returning to main menu.");
                SceneManager.LoadScene(SceneNames.MainMenu);
            }
        }
    }
}
