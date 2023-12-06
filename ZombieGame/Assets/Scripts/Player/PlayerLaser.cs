using Assets.Scripts.Constants.Names;
using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerLaser : BasePlayer
    {
        private BaseWeapon BaseWeapon;

        protected override string BulletObjectName => ObjectNames.Beam;
        protected override string PlayerWeaponObjectName => ObjectNames.HumanLaser;

        protected override BaseWeapon GetWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
        {
            return BaseWeapon ?? new LaserWeapon(bullet, muzzle, reloadSound, sprite);
        }
    }
}
