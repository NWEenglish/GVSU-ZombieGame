using Assets.Scripts.Constants.Types;
using Assets.Scripts.Helpers;
using Assets.Scripts.Stores;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class PistolWeapon : Weapon
    {
        private const int StartingAmmo = 200;
        private const int Damage = 25;
        private const int ClipSize = 10;

        private readonly GameObject Bullet;
        private readonly GameObject Muzzle;

        public override int AmmoClip { get; protected set; }
        public override int RemainingAmmo { get; protected set; }
        public override int AmmoClipSize { get; protected set; }
        public override AudioSource ReloadSound { get; protected set; }
        public override Sprite Sprite { get; protected set; }
        public override WeaponType Type => WeaponType.Pistol;

        public override bool CanShoot() => true;

        public PistolWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
        {
            Bullet = bullet;
            Muzzle = muzzle;
            Sprite = sprite;
            ReloadSound = reloadSound;

            AmmoClipSize = ClipSize;
            AmmoClip = AmmoClipSize;
            RemainingAmmo = StartingAmmo;
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
            if (AmmoClip > 0 && !ReloadSound.isPlaying)
            {
                AmmoClip--;
                ShootingHelper.Shoot(Bullet, Muzzle.transform.position, angle, Damage);
            }
        }
    }
}
