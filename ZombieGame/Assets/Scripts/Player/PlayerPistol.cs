using Assets.Scripts.Constants.Names;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerPistol : BasePlayer
    {
        private BaseWeapon BaseWeapon;

        protected override string BulletObjectName => ObjectNames.Bullet;
        protected override string PlayerWeaponObjectName => ObjectNames.PlayerPistol;

        protected override BaseWeapon GetWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
        {
            return BaseWeapon ?? new PistolWeapon(bullet, muzzle, reloadSound, sprite);
        }
    }
}
