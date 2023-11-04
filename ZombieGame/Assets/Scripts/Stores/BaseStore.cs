using System;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.Player;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Stores
{
    public abstract class BaseStore : MonoBehaviour
    {
        public abstract int CostToBuy { get; }
        public abstract int CostForAmmo { get; }
        public abstract int AmmoReplenish { get; }
        public abstract WeaponType Type { get; }

        private TextMesh TextMesh;
        private AudioSource PurchaseSound;

        public int BuyAmmo(ref int Points)
        {
            int ammoBought = 0;

            if (Points >= CostForAmmo)
            {
                Points -= CostForAmmo;
                PurchaseSound.TryPlay();

                ammoBought = AmmoReplenish;
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

                weaponBought = Type switch
                {
                    WeaponType.Pistol => GameObject.Find(Enum.GetName(typeof(WeaponType), Type)).GetComponent<PlayerPistol>().Weapon,// TODO: These player objects will need a base class, then this can be shrunk 
                    WeaponType.Rifle => GameObject.Find(Enum.GetName(typeof(WeaponType), Type)).GetComponent<PlayerRifle>().Weapon,
                    WeaponType.Laser => GameObject.Find(Enum.GetName(typeof(WeaponType), Type)).GetComponent<PlayerLaser>().Weapon,
                    _ => throw new NotImplementedException(),
                };
            }

            return weaponBought;
        }

        private void Start()
        {
            PurchaseSound = gameObject.GetComponent<AudioSource>();

            TextMesh = gameObject.GetComponentInChildren<TextMesh>();
            TextMesh.color = Color.clear;
            TextMesh.text = $"Buy: {CostToBuy}\nAmmo: {CostForAmmo}\nPress 'B' to buy";
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.name.Contains(ObjectNames.Player))
            {
                TextMesh.color = Color.yellow;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name.Contains(ObjectNames.Player))
            {
                TextMesh.color = Color.clear;
            }
        }
    }
}