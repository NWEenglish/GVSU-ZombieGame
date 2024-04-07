using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.NPC;
using Assets.Scripts.Singletons;
using Assets.Scripts.Spawners;
using Assets.Scripts.Stores;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GeneralGameLogic
{
    public class DeathmatchLogic : BaseGameModeLogic
    {
        public override GameModeType GameMode => GameModeType.NonZombieMode;

        private int maxFriendlyBots => 4;
        private int maxHostileBots => 5;

        private Dictionary<Transform, InitialSpawnerLogic> InitialSpawners = new Dictionary<Transform, InitialSpawnerLogic>();
        private List<Transform> PointsOfInterest = new List<Transform>();

        private List<GameObject> FriendlyBots = new List<GameObject>();
        private List<GameObject> HostileBots = new List<GameObject>();

        private bool GameStarted;

        private void Start()
        {
            BaseStart();

            PointsOfInterest = GameObject.Find(ObjectNames.PointsOfInterest).GetComponentsInChildren<Transform>().ToList();

            var spawners = GameObject.Find(ObjectNames.InitialSpawnHolder).GetComponentsInChildren<Transform>().ToList();
            foreach (Transform t in spawners)
            {
                if (t.gameObject.TryGetComponent(out InitialSpawnerLogic spawner))
                {
                    InitialSpawners.Add(t, spawner);
                }
            }

            foreach (KeyValuePair<Transform, InitialSpawnerLogic> spawner in InitialSpawners)
            {
                TeamType team = spawner.Value.GetTeamType();
                GameObject bot = Spawn(spawner.Key.position, team);
                UpdateBotLists(bot, team);
            }

            GameStarted = true;
        }

        private void Update()
        {
            if (GameStarted)
            {
                FriendlyBots.RemoveAll(bot => bot == null);
                HostileBots.RemoveAll(bot => bot == null);

                RespawnBots(FriendlyBots.Count, maxFriendlyBots, TeamType.PlayerTeam);
                RespawnBots(HostileBots.Count, maxHostileBots, TeamType.HostileTeam);
            }
        }

        private void RespawnBots(int currentBots, int maxBots, TeamType team)
        {
            for (int i = currentBots; i < maxBots; i++)
            {
                // Pick Random Location
                SpawnerLogic spawner = null;
                while (spawner == null)
                {
                    int randomValue = (int)((Random.value * 100) % Spawners.Count);
                    spawner = Spawners[randomValue];

                    if (!spawner.CanSpawn)
                    {
                        spawner = null;
                    }
                }

                GameObject bot = Spawn(spawner.transform.position, team);
                UpdateBotLists(bot, team);
            }
        }

        private GameObject Spawn(Vector3 position, TeamType team)
        {
            GameObject originalHuman = GameObject.Find(ObjectNames.NPCHuman);
            GameObject retCreatedHuman = null;

            if (originalHuman != null)
            {
                retCreatedHuman = Instantiate(originalHuman, position, new Quaternion());
                retCreatedHuman.GetComponent<HumanLogic>().InitValues(PointsOfInterest, team);
            }

            return retCreatedHuman;
        }

        private void UpdateBotLists(GameObject bot, TeamType team)
        {
            switch (team)
            {
                case TeamType.PlayerTeam:
                    FriendlyBots.Add(bot);
                    break;
                case TeamType.HostileTeam:
                    HostileBots.Add(bot);
                    break;
                default:
                    break;
            }
        }
    }
}
