using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.Helpers;
using Assets.Scripts.HUD;
using Assets.Scripts.NPC;
using Assets.Scripts.Player;
using Assets.Scripts.Spawners;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GeneralGameLogic
{
    public class DeathmatchLogic : BaseGameModeLogic
    {
        public override GameModeType GameMode => GameModeType.NonZombieMode;

        private int MaxBotsPerTeam => 5;
        private int TargetPoints => 7500;
        private int PointsPerKill => 100;

        private int MaxFriendlyBots => MaxBotsPerTeam - 1;
        private int MaxHostileBots => MaxBotsPerTeam;

        private Dictionary<Transform, InitialSpawnerLogic> InitialSpawners = new Dictionary<Transform, InitialSpawnerLogic>();
        private Dictionary<TeamType, int> TeamPoints;
        private List<Transform> PointsOfInterest = new List<Transform>();

        private List<GameObject> FriendlyBots = new List<GameObject>();
        private List<GameObject> HostileBots = new List<GameObject>();

        private const double MaxGameTimeMin = 5;
        private float? TimerMs = null;

        private TeamPointsHUD LivesHUD;
        private TimerHUD TimerHUD;
        private GameOverHUD GameOverHUD;
        private Image RespawnPanel;
        private TextMeshProUGUI RespawnMessage;
        private PlayerLogic PlayerLogic;

        private bool GameStarted = false;
        private bool RanGameOverLogic = false;
        private float? PlayerRespawnTimerMs = null;

        private bool PlayerInProcessOfRespawning => PlayerRespawnTimerMs.HasValue;

        private void Awake()
        {
            TimerMs = float.Parse(TimeSpan.FromMinutes(MaxGameTimeMin).TotalMilliseconds.ToString());

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

            RespawnPanel = GameObject.Find(ObjectNames.Respawn_Panel_HUD).GetComponent<Image>();
            RespawnMessage = GameObject.Find(ObjectNames.Respawn_Message_HUD).GetComponent<TextMeshProUGUI>();

            PlayerLogic = GameObject.Find(ObjectNames.Player).GetComponent<PlayerLogic>();
            LivesHUD = new TeamPointsHUD(GameObject.Find(ObjectNames.Team_Points_HUD).GetComponent<TextMeshProUGUI>());
            TimerHUD = new TimerHUD(GameObject.Find(ObjectNames.Timer_HUD).GetComponent<TextMeshProUGUI>());
            GameOverHUD = new GameOverHUD()
            {
                GameOverTitle = GameObject.Find(ObjectNames.GameOver_Title).GetComponent<TextMeshProUGUI>(),
                GameOverSubtext = GameObject.Find(ObjectNames.GameOver_Subtext).GetComponent<TextMeshProUGUI>()
            };

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
            // Game in-progress
            if (GameStarted && !RanGameOverLogic)
            {
                // Update Points
                TeamPoints[TeamType.PlayerTeam] += HostileBots.RemoveAll(bot => bot == null) * PointsPerKill;
                TeamPoints[TeamType.HostileTeam] += FriendlyBots.RemoveAll(bot => bot == null) * PointsPerKill;

                // If player was killed, respawn and award points
                if (TryRespawnPlayer() && !PlayerInProcessOfRespawning)
                {
                    TeamPoints[TeamType.HostileTeam] += PointsPerKill;
                }
                else if (PlayerInProcessOfRespawning)
                {
                    ContinuePlayerRespawnProcess();
                }

                // Attempt bot respawns
                TryRespawnBots(FriendlyBots.Count, MaxFriendlyBots, TeamType.PlayerTeam);
                TryRespawnBots(HostileBots.Count, MaxHostileBots, TeamType.HostileTeam);

                // Update HUDs
                LivesHUD.UpdateHUD(TeamPoints[TeamType.PlayerTeam], TeamPoints[TeamType.HostileTeam], TargetPoints);
            }

            // Game over
            if (IsGameOver() && !RanGameOverLogic)
            {
                // If respawning, just hide the message, else gray out the screen.
                if (PlayerInProcessOfRespawning)
                {
                    RespawnMessage.color = Color.clear;
                }
                else
                {
                    RespawnPanel.color = new Color(0, 0, 0, 0.25f);
                }

                RanGameOverLogic = true;

                GameOutcome outcome = FindGameOutcome();
                PlayMusic(outcome);
                GameOverHUD.ShowNonZombiesGameOver(outcome);

                PlayerLogic.Disable();
                List<GameObject> allBots = GameObject.FindGameObjectsWithTag(TagNames.NPC).ToList();
                foreach (GameObject bot in allBots)
                {
                    bot.GetComponent<HumanLogic>().Disable();
                }
            }

            if (RanGameOverLogic)
            {
                // If an additional 15secs have elapsed, return to main menu
                double seconds = TimeSpan.FromMilliseconds(Math.Abs(TimerMs.Value)).TotalSeconds;
                if (seconds > 15)
                {
                    SceneManager.LoadScene(SceneNames.MainMenu);
                }
            }

            // Update timer
            if (TimerMs.HasValue)
            {
                TimerMs -= Time.deltaTime * 1000f;
                TimerHUD.UpdateHUD(TimerMs.Value);
            }
        }

        private void PlayMusic(GameOutcome outcome)
        {
            int index = outcome switch
            {
                GameOutcome.Lost => 1,
                GameOutcome.Tied => 2,
                GameOutcome.Won => 3,
                _ => throw new NotImplementedException("The game outcome is not supported.")
            };

            AudioSource song = gameObject.GetComponents<AudioSource>()[index];
            AudioSource ambiance = gameObject.GetComponents<AudioSource>()[0];

            if (song != null && !song.isPlaying)
            {
                ambiance.Stop();
                song.TryPlay();
            }
        }

        private GameOutcome FindGameOutcome()
        {
            GameOutcome retOutcome;

            // Won
            if (TeamPoints[TeamType.PlayerTeam] > TeamPoints[TeamType.HostileTeam])
            {
                retOutcome = GameOutcome.Won;
            }
            // Lost
            else if (TeamPoints[TeamType.PlayerTeam] < TeamPoints[TeamType.HostileTeam])
            {
                retOutcome = GameOutcome.Lost;
            }
            // Draw
            else
            {
                retOutcome = GameOutcome.Tied;
            }

            return retOutcome;
        }

        private bool IsGameOver()
        {
            bool retIsGameOver = false;

            if (TimerMs.HasValue && TimerMs.Value < 0f)
            {
                retIsGameOver = true;
            }
            else if (TeamPoints.Any(kvp => kvp.Value >= TargetPoints))
            {
                retIsGameOver = true;
            }

            return retIsGameOver;
        }

        private bool TryRespawnPlayer()
        {
            bool wasKilled = PlayerLogic.Status.IsPlayerDead;

            if (wasKilled)
            {
                // Black out screen, start timer
                RespawnPanel.color = Color.black;
                RespawnMessage.color = Color.white;

                // Move off map
                Vector3 hidingSpot = Vector3.zero;
                hidingSpot.x = 1000f;
                PlayerLogic.gameObject.transform.position = hidingSpot;

                // Reset health and weapons
                PlayerLogic.ResetPlayer();
                PlayerLogic.Disable();

                // Set timer to 5 seconds
                PlayerRespawnTimerMs = (float)TimeSpan.FromSeconds(5).TotalMilliseconds;
            }

            return wasKilled;
        }

        private void ContinuePlayerRespawnProcess()
        {
            // Move to respawn points and remove black screen.
            if (PlayerRespawnTimerMs < 0f)
            {
                PlayerRespawnTimerMs = null;
                SpawnerLogic spawner = GetRandomSpawner(TeamType.PlayerTeam);
                PlayerLogic.gameObject.transform.position = spawner.Position;
                PlayerLogic.Enable();

                RespawnPanel.color = Color.clear;
                RespawnMessage.color = Color.clear;
            }
            else
            {
                PlayerRespawnTimerMs -= Time.deltaTime * 1000f;
            }
        }

        private void TryRespawnBots(int currentBots, int maxBots, TeamType team)
        {
            for (int i = currentBots; i < maxBots; i++)
            {
                SpawnerLogic spawner = GetRandomSpawner(team);
                GameObject bot = Spawn(spawner.transform.position, team);
                UpdateBotLists(bot, team);
            }
        }

        private SpawnerLogic GetRandomSpawner(TeamType team)
        {
            float failSafeSpawnTimer = (float)TimeSpan.FromSeconds(5).TotalMilliseconds;

            // Pick Random Location
            SpawnerLogic retSpawner = null;
            while (retSpawner == null)
            {
                int randomValue = (int)((Random.value * 100) % Spawners.Count);
                retSpawner = Spawners[randomValue];

                if (!retSpawner.CanSpawn(team))
                {
                    retSpawner = null;
                    failSafeSpawnTimer -= Time.deltaTime * 1000f;
                }
                // If we cannot find a safe spawner, the random one will have to do.
                else if (failSafeSpawnTimer < 0f)
                {
                    break;
                }
            }

            return retSpawner;
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
