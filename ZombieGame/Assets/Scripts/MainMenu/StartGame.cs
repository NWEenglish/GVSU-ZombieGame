using System;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.MainMenu
{
    public class StartGame : MonoBehaviour
    {
        public GameModeType GameMode;

        private readonly Logger _logger = Logger.GetLogger();

        private void Start()
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            string sceneName = GameMode switch
            {
                GameModeType.NonZombieMode => SceneNames.NonZombieGameMode,
                GameModeType.ZombieMode => SceneNames.ZombieGameMode,
                _ => throw new ArgumentException($"The provided game mode type does not have a scene associated with it. | GameModeType: {GameMode}")
            };

            SceneManager.LoadScene(sceneName);
            _logger.LogDebug($"Starting a new game. | GameModeType: {GameMode} | SceneName: {sceneName}");
        }
    }
}
