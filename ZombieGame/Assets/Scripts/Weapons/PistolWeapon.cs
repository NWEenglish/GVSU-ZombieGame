using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class PistolWeapon : BaseWeapon
    {
        public override int Damage => 25;
        public override int ClipSize => 10;
        public override int StartingAmmo => 200;
        public override double TimeBetweenShotsInMS => 0;
        public override WeaponType Type => WeaponType.Pistol;

        public PistolWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
            : base(bullet, muzzle, reloadSound, sprite)
        { }
    }
}
