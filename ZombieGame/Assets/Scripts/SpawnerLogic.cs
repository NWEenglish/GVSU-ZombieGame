using Assets.Scripts.Constants.Names;
using UnityEngine;

namespace Assets.Scripts
{
    public class SpawnerLogic : MonoBehaviour
    {
        public bool CanSpawn => !IsPlayerInRange();
        public Vector3 Position => gameObject.transform.position;

        private const float Range = 20;
        private GameObject Player;

        private void Start()
        {
            Player = GameObject.Find(ObjectNames.Player);
        }

        private bool IsPlayerInRange()
        {
            if (Player == null)
            {
                return false;
            }

            return Mathf.Abs(Vector2.Distance(gameObject.transform.position, Player.transform.position)) <= Range;
        }
    }
}
