using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.NPC;
using Assets.Scripts.Stores;
using TMPro;
using UnityEngine;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.GeneralGameLogic
{
    public class WaveLogic : BaseGameModeLogic
    {
        public int Health => CalculateHealth();
        public int Wave { get; private set; }
        public bool ShouldSprint => CalculateShouldSprint();

        public override GameModeType GameMode => GameModeType.ZombieMode;

        private readonly Logger _logger = Logger.GetLogger();
        private const int WaitTime = 15;
        private const int TimeBetweenZombieSpawnsInMS = 750;
        private const int MaxZombiesAtOnce = 30;

        private List<GameObject> ZombiesInGame = new List<GameObject>();

        private int RemainingZombiesToSpawn;
        private int RoundPlayedMusicForLast;
        private GameObject Zombie;
        private AudioSource RoundOverMusic;
        private TextMeshProUGUI Wave_HUD;
        private System.DateTime LastZombieSpawn;
        private System.DateTime EndOfWave;

        private void Start()
        {
            BaseStart();

            Zombie = GameObject.Find(ObjectNames.Zombie);
            RoundOverMusic = gameObject.GetComponents<AudioSource>()[1];
            Wave_HUD = GameObject.Find(ObjectNames.Wave_HUD).GetComponent<TextMeshProUGUI>();

            Wave_HUD.color = Color.black;
            LastZombieSpawn = System.DateTime.Now;
            EndOfWave = System.DateTime.Now;
            Wave = 0;
            RoundPlayedMusicForLast = Wave;
        }

        private void Update()
        {
            // Simple cleanup
            if (ZombiesInGame.Count > 0)
            {
                ZombiesInGame.RemoveAll(z => z == null);
            }

            // Spawning and Wave logic
            if (RemainingZombiesToSpawn + ZombiesInGame.Count > 0)
            {
                AttemptSpawnZombies();
                EndOfWave = System.DateTime.Now;
            }
            // Once there are no more zombies and there are no more to spawn, begin next wave.
            else if (System.DateTime.Now > EndOfWave.AddSeconds(WaitTime))
            {
                StartNextWave();
            }
            else if (RoundPlayedMusicForLast != Wave)
            {
                _logger.LogDebug("Playing sound for start of next wave.");
                RoundOverMusic.TryPlay();
                RoundPlayedMusicForLast = Wave;
                Wave_HUD.color = Color.black;
            }
        }

        private void StartNextWave()
        {
            Wave++;
            Wave_HUD.text = $"Wave: {Wave}";
            Wave_HUD.color = Color.red;
            CalculateZombiesForWave();

            _logger.LogDebug($"Starting the next wave. | Wave: {Wave} | ZombieCount: {RemainingZombiesToSpawn}");
        }

        private int CalculateHealth()
        {
            return 100 + (50 * Wave);
        }

        private bool CalculateShouldSprint()
        {
            // Round 1 will never spawn runners
            if (Wave == 1)
            {
                return false;
            }

            int randomValue = (int)(Random.value * 100);

            // Want to ensure even high waves have a chance of spawning walkers
            if (Wave > 10)
            {
                return randomValue < 95;
            }
            else
            {
                // Gradually increase chance for runners
                int chance = (Wave - 1) * 10;
                return randomValue < chance;
            }
        }

        private void CalculateZombiesForWave()
        {
            const int baseEnemyCount = 3;
            RemainingZombiesToSpawn = baseEnemyCount * (Wave + 2);
        }

        private void AttemptSpawnZombies()
        {
            if (ZombiesInGame.Count < MaxZombiesAtOnce && RemainingZombiesToSpawn > 0 && LastZombieSpawn.AddMilliseconds(TimeBetweenZombieSpawnsInMS) <= System.DateTime.Now)
            {
                SpawnZombie();
                LastZombieSpawn = System.DateTime.Now;
            }
        }

        private void SpawnZombie()
        {
            // Pick Random Location
            SpawnerLogic spawner = null;
            while (spawner == null)
            {
                int randomValue = (int)((Random.value * 100) % Spawners.Count);
                spawner = Spawners[randomValue];

                if (!spawner.CanSpawn())
                {
                    spawner = null;
                }
            }

            // Spawn Zombie
            GameObject zombie = Instantiate(Zombie, spawner.Position, new Quaternion());
            zombie.GetComponent<BaseNpcLogic>().InitValues();
            ZombiesInGame.Add(zombie);
            RemainingZombiesToSpawn--;
        }
    }
}
