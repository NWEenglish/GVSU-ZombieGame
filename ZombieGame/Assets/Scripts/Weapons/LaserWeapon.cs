using Assets.Scripts.Constants.Types;
using Assets.Scripts.Helpers;
using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class LaserWeapon : Weapon
    {
        private const int Damage = 600;
        private const int ClipSize = 5;
        private const int StartingAmmo = 25;

        private readonly GameObject Bullet;
        private readonly GameObject Muzzle;

        private GameObject LastBulletShot;

        public override int AmmoClip { get; protected set; }
        public override int RemainingAmmo { get; protected set; }
        public override int AmmoClipSize { get; protected set; }
        public override AudioSource ReloadSound { get; protected set; }
        public override Sprite Sprite { get; protected set; }
        public override WeaponType Type { get; protected set; }

        public override bool IsReloading() => ReloadSound.isPlaying;

        public LaserWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
        {
            Bullet = bullet;
            Muzzle = muzzle;
            Sprite = sprite;
            ReloadSound = reloadSound;

            AmmoClipSize = ClipSize;
            AmmoClip = AmmoClipSize;
            RemainingAmmo = StartingAmmo;
            Type = WeaponType.Laser;
        }

        public override bool CanShoot()
        {
            return LastBulletShot == null;
        }

        public override void PurchaseAmmo(ref int Points, StoreHelper store)
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
                LastBulletShot = ShootingHelper.Shoot(Bullet, Muzzle.transform.position, angle, Damage);
            }
        }
    }
}
