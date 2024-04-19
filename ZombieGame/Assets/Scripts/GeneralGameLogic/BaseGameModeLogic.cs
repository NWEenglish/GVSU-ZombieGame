using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GeneralGameLogic
{
    // This should be expaned in phase3 - need to split into zombie and non-zombie modes (make this abstract in that phase)
    public abstract class BaseGameModeLogic : MonoBehaviour
    {
        public abstract GameModeType GameMode { get; }

        protected List<SpawnerLogic> Spawners;

        protected void BaseStart()
        {
            Spawners = GameObject.Find(ObjectNames.SpawnerHolder).GetComponentsInChildren<SpawnerLogic>().ToList();

            // Borrowed from TankGame
            int seed = (int)(new System.Random().NextDouble() * 1000000000);
            Random.InitState(seed);
            GameObject.Find(ObjectNames.Seed_HUD).GetComponent<TextMeshProUGUI>().text = $"Seed: {seed}";
        }
    }
}
