using System;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Player;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Stores
{
    public class StoreHelper
    {
        public int BuyCost { get; private set; }
        public int AmmoCost { get; private set; }
        public int AmmoReplenish { get; private set; }
        public WeaponType Type { get; set; }

        private AudioSource PurchaseSound;

        public StoreHelper(int costToBuy, int costForAmmo, int ammoReplenish, AudioSource purchaseSound, WeaponType type)
        {
            BuyCost = costToBuy;
            AmmoCost = costForAmmo;
            AmmoReplenish = ammoReplenish;
            PurchaseSound = purchaseSound;
            Type = type;
        }

        public int BuyAmmo(ref int Points)
        {
            if (Points < AmmoCost)
            {
                return 0;
            }
            else
            {
                Points -= AmmoCost;
                PurchaseSound.Play();

                return AmmoReplenish;
            }
        }

        public Weapon PurchaseWeapon(ref int Points)
        {
            if (Points < BuyCost)
            {
                return null;
            }
            else
            {
                Points -= BuyCost;
                PurchaseSound.Play();

                switch (Type)
                {
                    case WeaponType.Pistol:
                        return GameObject.Find(ObjectNames.Pistol).GetComponent<PlayerPistol>().Weapon;
                    case WeaponType.Rifle:
                        return GameObject.Find(ObjectNames.Rifle).GetComponent<PlayerRifle>().Weapon;
                    case WeaponType.Laser:
                        return GameObject.Find(ObjectNames.Laser).GetComponent<PlayerLaser>().Weapon;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
