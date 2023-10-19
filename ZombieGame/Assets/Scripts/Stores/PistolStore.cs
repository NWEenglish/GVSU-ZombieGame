using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores
{
    public class PistolStore : BaseStore
    {
        private const int CostToBuy = 500;
        private const int CostForAmmo = 200;
        private const int AmmoReplenish = 50;
        private const WeaponType Type = WeaponType.Pistol;

        private void Start()
        {
            base.OnStart(CostToBuy, CostForAmmo, AmmoReplenish, Type);
        }
    }
}
