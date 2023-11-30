using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores.WeaponStores
{
    public class RifleStore : BaseWeaponStore
    {
        protected override int CostToBuy => 1500;
        protected override int CostForAmmo => 500;
        protected override int AmmoReplenish => 100;
        public override WeaponType Type => WeaponType.Rifle;
    }
}
