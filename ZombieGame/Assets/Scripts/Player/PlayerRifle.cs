using Assets.Scripts.Constants.Names;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerRifle : BasePlayer
    {
        private BaseWeapon BaseWeapon;

        protected override string BulletObjectName => ObjectNames.Bullet;
        protected override string PlayerWeaponObjectName => ObjectNames.PlayerRifle;

        protected override BaseWeapon GetWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
        {
            return BaseWeapon ?? new RifleWeapon(bullet, muzzle, reloadSound, sprite);
        }
    }
}
