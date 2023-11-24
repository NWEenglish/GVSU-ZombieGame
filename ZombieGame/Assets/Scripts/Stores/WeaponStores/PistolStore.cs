using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores.WeaponStores
{
    public class PistolStore : BaseWeaponStore
    {
        protected override int CostToBuy => 500;
        protected override int CostForAmmo => 200;
        protected override int AmmoReplenish => 50;
        public override WeaponType Type => WeaponType.Pistol;
    }
}
