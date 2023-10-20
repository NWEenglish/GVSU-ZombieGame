using System;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
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
            if (Points < CostForAmmo)
            {
                return 0;
            }
            else
            {
                Points -= CostForAmmo;
                PurchaseSound.Play();

                return AmmoReplenish;
            }
        }

        public Weapon PurchaseWeapon(ref int Points)
        {
            if (Points < CostToBuy)
            {
                return null;
            }
            else
            {
                Points -= CostToBuy;
                PurchaseSound.Play();

                switch (Type)
                {
                    case WeaponType.Pistol:
                        return GameObject.Find(Enum.GetName(typeof(WeaponType), Type)).GetComponent<PlayerPistol>().Weapon; // TODO: These player objects will need a base class, then this can be shrunk 
                    case WeaponType.Rifle:
                        return GameObject.Find(Enum.GetName(typeof(WeaponType), Type)).GetComponent<PlayerRifle>().Weapon;
                    case WeaponType.Laser:
                        return GameObject.Find(Enum.GetName(typeof(WeaponType), Type)).GetComponent<PlayerLaser>().Weapon;
                    default:
                        throw new NotImplementedException();
                }
            }
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