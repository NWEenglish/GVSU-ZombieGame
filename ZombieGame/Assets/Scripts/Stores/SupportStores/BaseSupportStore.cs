using System;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.NPC;
using UnityEngine;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.Stores.SupportStores
{
    public abstract class BaseSupportStore : BaseStore
    {
        public abstract SupportType Type { get; }
        protected override string DisplayText => $"Buy {Type} Support: {CostToBuy}\nPress 'B' to buy";

        private readonly Logger _logger = Logger.GetLogger();

        public void PurchaseSupport(ref int Points, Vector3 position)
        {
            if (Points >= CostToBuy)
            {
                Points -= CostToBuy;
                PurchaseSound.TryPlay();
                SpawnSupport(position);

                _logger.LogDebug($"Player has purchased support. | CostToBuy: {CostToBuy} | RemainingTotalPoints: {Points}");
            }
            else
            {
                _logger.LogDebug($"Player was not able to purchase support. | CostToBuy: {CostToBuy} | TotalPoints: {Points}");
            }
        }

        private GameObject SpawnSupport(Vector3 position)
        {
            GameObject originalSupport = null;
            GameObject retSpawnedSupport = null;

            if (Type == SupportType.Rifle)
            {
                originalSupport = GameObject.Find(ObjectNames.NPCHuman);
            }
            else
            {
                _logger.LogError($"The provided support type has not been implemented. | SupportType: {Type}");
                throw new NotImplementedException();
            }

            if (originalSupport != null)
            {
                retSpawnedSupport = Instantiate(originalSupport, position, new Quaternion());
                retSpawnedSupport.GetComponent<BaseNpcLogic>().InitValues();
            }

            return retSpawnedSupport;
        }
    }
}
