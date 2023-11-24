using System;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.Player;
using Assets.Scripts.Weapons;
using UnityEngine;
using Logger = Assets.Scripts.Singletons.Logger;

namespace Assets.Scripts.Stores.WeaponStores
{
    public abstract class BaseWeaponStore : BaseStore
    {
        public abstract WeaponType Type { get; }
        protected abstract int CostForAmmo { get; }
        protected abstract int AmmoReplenish { get; }
        protected override string DisplayText => $"Buy: {CostToBuy}\nAmmo: {CostForAmmo}\nPress 'B' to buy";

        private readonly Logger _logger = Logger.GetLogger();

        public int BuyAmmo(ref int Points)
        {
            int ammoBought = 0;

            if (Points >= CostForAmmo)
            {
                Points -= CostForAmmo;
                PurchaseSound.TryPlay();

                ammoBought = AmmoReplenish;
                _logger.LogDebug($"Player has purchased ammo. | CostForAmmo: {CostForAmmo} | RemainingTotalPoints: {Points} | AmountOfAmmoBought: {ammoBought}");
            }
            else
            {
                _logger.LogDebug($"Player was not able to purchase ammo. | CostForAmmo: {CostForAmmo} | TotalPoints: {Points}");
            }

            return ammoBought;
        }

        public BaseWeapon PurchaseWeapon(ref int Points)
        {
            BaseWeapon weaponBought = null;

            if (Points >= CostToBuy)
            {
                Points -= CostToBuy;
                PurchaseSound.TryPlay();
                weaponBought = GameObject.Find(Enum.GetName(typeof(WeaponType), Type)).GetComponent<BasePlayer>().Weapon;
                _logger.LogDebug($"Player has purchased a weapon. | CostForWeapon: {CostToBuy} | RemainingTotalPoints: {Points} | WeaponBought: {weaponBought.Type}");
            }
            else
            {
                _logger.LogDebug($"Player was not able to purchase a weapon. | CostForAmmo: {CostToBuy} | TotalPoints: {Points}");
            }

            return weaponBought;
        }
    }
}