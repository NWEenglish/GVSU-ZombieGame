using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class LaserWeapon : BaseWeapon
    {
        public override int Damage => 600;
        public override int ClipSize => 5;
        public override int StartingAmmo => 25;
        public override double TimeBetweenShotsInMS => 500;
        public override WeaponType Type => WeaponType.Laser;

        public LaserWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
            : base(bullet, muzzle, reloadSound, sprite)
        { }
    }
}
