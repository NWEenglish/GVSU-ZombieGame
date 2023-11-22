using Assets.Scripts.Constants.Names;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.MainMenu
{
    public class StartGame : MonoBehaviour
    {
        private readonly Logger _logger = Logger.GetLogger();

        private void Start()
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            _logger.LogDebug("Starting a new game.");
            SceneManager.LoadScene(SceneNames.Game);
        }
    }
}
