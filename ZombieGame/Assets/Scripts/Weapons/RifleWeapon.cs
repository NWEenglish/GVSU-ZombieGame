using Assets.Scripts.Constants.Types;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class RifleWeapon : BaseWeapon
    {
        public override int Damage => 55;
        public override int ClipSize => 30;
        public override int StartingAmmo => 250;
        public override double TimeBetweenShotsInMS => 200;
        public override WeaponType Type => WeaponType.Rifle;
        public override FireType FireType => FireType.FullyAutomatic;

        public RifleWeapon(GameObject bullet, GameObject muzzle, AudioSource reloadSound, Sprite sprite)
            : base(bullet, muzzle, reloadSound, sprite)
        { }
    }
}
