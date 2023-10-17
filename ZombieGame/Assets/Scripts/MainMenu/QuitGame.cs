using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class QuitGame : MonoBehaviour
    {
        private void Start()
        {
            Button button = gameObject.GetComponent<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif

            Application.Quit();
        }
    }
}
