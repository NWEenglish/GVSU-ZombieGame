using Assets.Scripts.Constants.Names;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Player
{
    // Borrowed from TankGame
    public class PlayerCamera : MonoBehaviour
    {
        private const float CameraHeight = 5f;
        private GameObject Player;

        private bool PlayerHitEsc => Input.GetKeyDown(KeyCode.Escape);

        void Start()
        {
            Player = GameObject.Find(ObjectNames.Player);
        }

        void Update()
        {
            if (Player != null)
            {
                Vector3 targetPosition = Player.transform.transform.position;
                gameObject.transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - CameraHeight);
            }
            else
            {
                // TODO - Player has died - follow another entity?

            }

            if (PlayerHitEsc)
            {
                SceneManager.LoadScene(SceneNames.MainMenu);
            }
        }
    }
}
