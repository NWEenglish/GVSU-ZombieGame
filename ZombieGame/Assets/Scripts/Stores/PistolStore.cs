using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores
{
    public class PistolStore : BaseStore
    {
        public override int CostToBuy => 500;
        public override int CostForAmmo => 200;
        public override int AmmoReplenish => 50;
        public override WeaponType Type => WeaponType.Pistol;
    }
}
