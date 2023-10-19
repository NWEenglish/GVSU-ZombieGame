using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores
{
    public class RifleStore : BaseStore
    {
        private const int CostToBuy = 1500;
        private const int CostForAmmo = 500;
        private const int AmmoReplenish = 100;
        private const WeaponType Type = WeaponType.Rifle;

        private void Start()
        {
            base.OnStart(CostToBuy, CostForAmmo, AmmoReplenish, Type);
        }
    }
}
