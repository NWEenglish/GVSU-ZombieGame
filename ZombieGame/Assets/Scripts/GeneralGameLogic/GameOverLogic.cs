using Assets.Scripts.Constants.Names;
using Assets.Scripts.Extensions;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GeneralGameLogic
{
    public class GameOverLogic : MonoBehaviour
    {
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
                InGameOver = true;
                GameOverMusic.TryPlay();
            }
            else if (Player.Status.IsPlayerDead && !GameOverMusic.isPlaying)
            {
                SceneManager.LoadScene(SceneNames.MainMenu);
            }
        }
    }
}
