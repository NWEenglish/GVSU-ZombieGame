using Assets.Scripts.Constants.Types;
using Assets.Scripts.Extensions;
using Assets.Scripts.Singletons;

namespace Assets.Scripts.Stores.SupportStores
{
    public abstract class BaseSupportStore : BaseStore
    {
        public abstract SupportType Type { get; }
        protected override string DisplayText => $"Buy {Type} Support: {CostToBuy}\nPress 'B' to buy";

        private readonly Logger _logger = Logger.GetLogger();

        public void BuySupport(ref int Points)
        {
            if (Points >= CostToBuy)
            {
                Points -= CostToBuy;
                PurchaseSound.TryPlay();

                // Spawn support

                _logger.LogDebug($"Player has purchased support. | CostToBuy: {CostToBuy} | RemainingTotalPoints: {Points}");
            }
            else
            {
                _logger.LogDebug($"Player was not able to purchase support. | CostToBuy: {CostToBuy} | TotalPoints: {Points}");
            }
        }
    }
}
