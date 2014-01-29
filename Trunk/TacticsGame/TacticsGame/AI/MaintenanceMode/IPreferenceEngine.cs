using System;
namespace TacticsGame.AI.MaintenanceMode
{
    /// <summary>
    /// Unit preferences guided only by the unit itself, unaffected by market conditions; those types of decisions fall under Decision Engine
    /// </summary>
    public interface IPreferenceEngine
    {
        int DetermineWillingnessToBuyItem(TacticsGame.GameObjects.Units.IMakeDecisions unit, TacticsGame.Items.Item item);
        int DetermineWillingnessToSellItem(TacticsGame.GameObjects.Units.IMakeDecisions unit, TacticsGame.Items.Item item);
        int ImproveBid(TacticsGame.GameObjects.Units.IMakeDecisions unit, TacticsGame.Items.Item item, int currentBid);
        int MakeAppraisal(TacticsGame.GameObjects.Units.IMakeDecisions unit, TacticsGame.Items.Item item, int boost = 0);
        int MakeBid(TacticsGame.GameObjects.Units.IMakeDecisions unit, TacticsGame.Items.Item item, bool sellingBid);
        bool UnitWantsToBuyItem(TacticsGame.GameObjects.Units.IMakeDecisions unitBuying, TacticsGame.GameObjects.Units.IMakeDecisions unitSelling, TacticsGame.Items.Item item, int ask);

        int ExpectedSellValue(GameObjects.Units.DecisionMakingUnit a, Items.Item item);

        int GetMinimumSellValue(GameObjects.Units.IMakeDecisions unit, Items.Item item);
        
    }
}
