using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores
{
    public class LaserStore : BaseStore
    {
        private const int CostToBuy = 6000;
        private const int CostForAmmo = 750;
        private const int AmmoReplenish = 35;
        private const WeaponType Type = WeaponType.Laser;

        private void Start()
        {
            base.OnStart(CostToBuy, CostForAmmo, AmmoReplenish, Type);
        }
    }
}
