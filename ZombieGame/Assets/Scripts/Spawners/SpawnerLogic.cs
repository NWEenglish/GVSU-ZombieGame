using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.NPC;
using UnityEngine;

namespace Assets.Scripts
{
    public class SpawnerLogic : MonoBehaviour
    {
        public Vector3 Position => gameObject.transform.position;

        private const float Range = 20;
        private GameObject Player;

        private void Start()
        {
            Player = GameObject.Find(ObjectNames.Player);
        }

        public bool CanSpawn()
        {
            return !IsPlayerInRange();
        }

        public bool CanSpawn(TeamType currentTeam)
        {
            // If opposing bots are nearby, don't spawn here.
            var currentBots = GameObject.FindGameObjectsWithTag(TagNames.NPC).Select(go => go.GetComponent<BaseNpcLogic>());
            var botsInRange = currentBots.Where(b => b != null && InRange(b.gameObject));
            bool areEnemiesInRange = botsInRange.Any(b => b.Team != currentTeam);

            // If player is in range, don't spawn hostile bots.
            if (currentTeam == TeamType.HostileTeam && IsPlayerInRange())
            {
                areEnemiesInRange = true;
            }

            return !areEnemiesInRange;
        }

        private bool IsPlayerInRange()
        {
            if (Player == null)
            {
                return false;
            }

            return InRange(Player);
        }

        private bool InRange(GameObject otherObject)
        {
            return Mathf.Abs(Vector2.Distance(gameObject.transform.position, otherObject.transform.position)) <= Range;
        }
    }
}
