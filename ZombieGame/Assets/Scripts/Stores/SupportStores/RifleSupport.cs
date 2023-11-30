using Assets.Scripts.Constants.Types;

namespace Assets.Scripts.Stores.SupportStores
{
    public class RifleSupport : BaseSupportStore
    {
        protected override SupportType Type => SupportType.Rifle;
        protected override int AllowedTotalSupport => 5;
        protected override int CostToBuy => 1000;
    }
}
