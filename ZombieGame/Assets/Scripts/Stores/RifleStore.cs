using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores
{
    public class RifleStore : BaseStore
    {
        public override int CostToBuy => 1500;
        public override int CostForAmmo => 500;
        public override int AmmoReplenish => 100;
        public override WeaponType Type => WeaponType.Rifle;
    }
}
