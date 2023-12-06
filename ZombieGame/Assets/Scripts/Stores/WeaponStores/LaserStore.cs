using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores.WeaponStores
{
    public class LaserStore : BaseWeaponStore
    {
        protected override int CostToBuy => 6000;
        protected override int CostForAmmo => 750;
        protected override int AmmoReplenish => 35;
        public override WeaponType Type => WeaponType.Laser;
    }
}
