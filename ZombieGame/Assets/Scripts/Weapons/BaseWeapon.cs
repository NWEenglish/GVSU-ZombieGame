using System;
using Assets.Scripts.Constants.Names;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.Helpers;
using Assets.Scripts.Stores.WeaponStores;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public abstract class BaseWeapon
    {
        public abstract int Damage { get; }
        public abstract int ClipSize { get; }
        public abstract int StartingAmmo { get; }
        public abstract double TimeBetweenShotsInMS { get; }
        public abstract WeaponType Type { get; }
        public abstract FireType FireType { get; }

        public int RemainingClipAmmo { get; private set; }
        public int RemainingTotalAmmo { get; private set; }

        private readonly AudioSource ReloadSound;
        private readonly Sprite Sprite;
        private readonly GameObject Muzzle;
        private readonly GameObject Bullet;
        private DateTime LastShot;
        private bool IsUnlimitedAmmo = false;

        public BaseWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
        {
            Bullet = bullet;
            Muzzle = muzzle;
            Sprite = sprite;
            ReloadSound = reloadSound;
            LastShot = DateTime.MinValue;

            RemainingClipAmmo = ClipSize;
            RemainingTotalAmmo = StartingAmmo;
        }

        public bool CanShoot()
        {
            return LastShot.AddMilliseconds(TimeBetweenShotsInMS) <= DateTime.Now;
        }

        public void Shoot(float angle)
        {
            if (RemainingClipAmmo > 0 && !IsReloading() && CanShoot())
            {
                RemainingClipAmmo--;
                LastShot = DateTime.Now;
                ShootingHelper.Shoot(Bullet, Muzzle.transform.position, angle, Damage);
            }
        }

        public bool IsReloading()
        {
            return ReloadSound?.isPlaying ?? false;
        }

        public void PurchaseAmmo(ref int Points, BaseWeaponStore store)
        {
            if (store.Type == Type)
            {
                RemainingTotalAmmo += store.BuyAmmo(ref Points);
            }
        }

        public void Reload()
        {
            if (RemainingClipAmmo != ClipSize && RemainingTotalAmmo > 0 && !IsReloading())
            {
                while (RemainingTotalAmmo > 0 && RemainingClipAmmo < ClipSize)
                {
                    RemainingTotalAmmo--;
                    RemainingClipAmmo++;
                }
                ReloadSound.TryPlay();
            }

            if (IsUnlimitedAmmo)
            {
                RemainingTotalAmmo = StartingAmmo;
            }
        }

        // Need to destroy PolygonCollider2D and re-add for the new physics to take effect.
        public void Equip(GameObject player)
        {
            player.GetComponent<SpriteRenderer>().sprite = Sprite;
        }

        internal void EnableUnlimitedAmmo(GameObject requestingGameObject)
        {
            if (requestingGameObject?.tag == TagNames.NPC)
            {
                IsUnlimitedAmmo = true;
            }
        }
    }
}
