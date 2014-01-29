using System;
namespace TacticsGame.World
{
    public interface ITownMarketManager
    {
        TacticsGame.GameObjects.Units.DecisionMakingUnit GetActualBestDealVendorByItem(System.Collections.Generic.List<TacticsGame.GameObjects.Units.DecisionMakingUnit> units, TacticsGame.Items.Item wantedItem);
    }
}
