using System;
using Assets.Scripts.Constants.Types;
using Assets.Scripts.Helpers;
using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class RifleWeapon : Weapon
    {
        private const int Damage = 55;
        private const int ClipSize = 30;
        private const int StartingAmmo = 250;
        private const double TimeBetweenShotsInMS = 200;

        private readonly GameObject Bullet;
        private readonly GameObject Muzzle;
        private DateTime LastShot = DateTime.MinValue;

        public override int AmmoClip { get; protected set; }
        public override int RemainingAmmo { get; protected set; }
        public override int AmmoClipSize { get; protected set; }
        public override AudioSource ReloadSound { get; protected set; }
        public override Sprite Sprite { get; protected set; }
        public override WeaponType Type { get; protected set; }

        public override bool IsReloading() => ReloadSound.isPlaying;

        public RifleWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
        {
            Bullet = bullet;
            Muzzle = muzzle;
            Sprite = sprite;
            ReloadSound = reloadSound;

            AmmoClipSize = ClipSize;
            AmmoClip = AmmoClipSize;
            RemainingAmmo = StartingAmmo;
            Type = WeaponType.Rifle;
        }

        public override bool CanShoot()
        {
            return LastShot.AddMilliseconds(TimeBetweenShotsInMS) <= DateTime.Now;
        }

        public override void PurchaseAmmo(ref int Points, BaseStore store)
        {
            if (store.Type == Type)
            {
                RemainingAmmo += store.BuyAmmo(ref Points);
            }
        }

        public override void Shoot(float angle)
        {
            if (AmmoClip > 0 && !ReloadSound.isPlaying && CanShoot())
            {
                AmmoClip--;
                LastShot = DateTime.Now;
                ShootingHelper.Shoot(Bullet, Muzzle.transform.position, angle, Damage);
            }
        }
    }
}
