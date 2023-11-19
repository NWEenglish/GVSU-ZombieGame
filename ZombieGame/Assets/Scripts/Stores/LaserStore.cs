using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores
{
    public class LaserStore : BaseStore
    {
        public override int CostToBuy => 6000;
        public override int CostForAmmo => 750;
        public override int AmmoReplenish => 35;
        public override WeaponType Type => WeaponType.Laser;
    }
}
