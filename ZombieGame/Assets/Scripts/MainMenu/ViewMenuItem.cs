using Assets.Scripts.Constants.Names;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class ViewMenuItem : MonoBehaviour
    {
        public GameObject MenuItemHolder;
        public GameObject MenuItemButton;

        private bool CurrentlyVisable;
        private GameObject MainMenuHolder;

        private bool PlayerHitEsc => Input.GetKeyDown(KeyCode.Escape);

        private void Start()
        {
            MainMenuHolder = GameObject.Find(ObjectNames.MainMenuHolder);

            Button button = MenuItemButton.GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            CurrentlyVisable = MenuItemHolder.activeSelf;
            if (CurrentlyVisable)
            {
                ChangeVisibility();
            }
        }

        private void Update()
        {
            if (PlayerHitEsc && CurrentlyVisable)
            {
                ChangeVisibility();
            }
        }

        private void OnClick()
        {
            ChangeVisibility();
        }

        private void ChangeVisibility()
        {
            MenuItemHolder.SetActive(!CurrentlyVisable);
            MainMenuHolder.SetActive(CurrentlyVisable);
            CurrentlyVisable = !CurrentlyVisable;
        }
    }
}
