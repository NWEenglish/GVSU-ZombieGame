using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores.SupportStores
{
    public class RifleSupport : BaseSupportStore
    {
        public override SupportType Type => SupportType.Rifle;
        protected override int CostToBuy => 1000;
    }
}
