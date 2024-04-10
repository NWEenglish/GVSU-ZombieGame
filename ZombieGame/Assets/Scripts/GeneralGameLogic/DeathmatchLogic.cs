using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.HUD;
using Assets.Scripts.NPC;
using Assets.Scripts.Spawners;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GeneralGameLogic
{
    public class DeathmatchLogic : BaseGameModeLogic
    {
        public override GameModeType GameMode => GameModeType.NonZombieMode;

        private int MaxBotsPerTeam => 5;
        private int TargetPoints => 5000;

        private int MaxFriendlyBots => MaxBotsPerTeam - 1;
        private int MaxHostileBots => MaxBotsPerTeam;

        private Dictionary<Transform, InitialSpawnerLogic> InitialSpawners = new Dictionary<Transform, InitialSpawnerLogic>();
        private Dictionary<TeamType, int> TeamPoints;
        private List<Transform> PointsOfInterest = new List<Transform>();

        private List<GameObject> FriendlyBots = new List<GameObject>();
        private List<GameObject> HostileBots = new List<GameObject>();

        private TeamPointsHUD LivesHUD;

        private bool GameStarted = false;

        private void Awake()
        {
            TeamPoints = new Dictionary<TeamType, int>()
            {
                { TeamType.PlayerTeam, 0 },
                { TeamType.HostileTeam, 0 }
            };
        }

        private void Start()
        {
            BaseStart();

            PointsOfInterest = GameObject.Find(ObjectNames.PointsOfInterest).GetComponentsInChildren<Transform>().ToList();
            LivesHUD = new TeamPointsHUD(GameObject.Find(ObjectNames.Team_Points_HUD).GetComponent<TextMeshProUGUI>());

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
                // Update Points
                TeamPoints[TeamType.PlayerTeam] += HostileBots.RemoveAll(bot => bot == null) * 100;
                TeamPoints[TeamType.HostileTeam] += FriendlyBots.RemoveAll(bot => bot == null) * 100;

                TryRespawnPlayer();
                TryRespawnBots(FriendlyBots.Count, MaxFriendlyBots, TeamType.PlayerTeam);
                TryRespawnBots(HostileBots.Count, MaxHostileBots, TeamType.HostileTeam);

                LivesHUD.UpdateHUD(TeamPoints[TeamType.PlayerTeam], TeamPoints[TeamType.HostileTeam], TargetPoints);
            }
        }

        private bool TryRespawnPlayer()
        {
            return false;
        }

        private void TryRespawnBots(int currentBots, int maxBots, TeamType team)
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
