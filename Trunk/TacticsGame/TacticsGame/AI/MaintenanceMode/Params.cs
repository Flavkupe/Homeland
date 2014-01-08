using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.AI.MaintenanceMode
{
    public static class Params
    {
        /// <summary>
        /// 100
        /// </summary>
        public const int VeryHigh = 100;
        
        /// <summary>
        /// 80
        /// </summary>
        public const int High = 80;
        
        /// <summary>
        /// 50
        /// </summary>
        public const int Mid = 50;

        /// <summary>
        /// 25
        /// </summary>
        public const int MidLow = 25;

        /// <summary>
        /// 20
        /// </summary>
        public const int Low = 20; 

        /// <summary>
        /// 10
        /// </summary>
        public const int Incremental = 10;

        /// <summary>
        /// -10000
        /// </summary>
        public const int Prohibitive = -10000;

        public static int RandomVariance = Mid;

        public static int CoupleOfItems = 3;
        public static int ModerateNumberOfItems = 6;
        public static int TooManyItems = 10;

        public static int BuildingToBuyFrom_Starting = 0;
        public static int BuildingToBuyFrom_ExtraVariance = Incremental;
        public static int BuildingToBuyFrom_HasWeapUpgrade = High;

        public static int BuildingToSellTo_Starting = Incremental;
        public static int BuildingToSellTo_ExtraVariance = Low;
        public static int BuildingToSellTo_MinimumMoney = 10; // TODO

        public static int PropensityToBuy_Starting = 0;
        public static int PropensityToBuy_JustSold = High;
        public static int PropensityToBuy_LotsOfMoney = MidLow;
        public static int PropensityToBuy_WantsToUpgradeWeaponOrArmor = Mid;        
        public static int PropensityToBuy_LotsOfMoneyAmount = 100; // TODO
        public static int PropensityToBuy_TooLittleMoneyAmount = 30; // TODO

        public static int PropensityToLeisure_Starting = Incremental;
        
        public static int VisitorToSellTo_ExtraVariance = Incremental;        
    }

    public static class ParamUtils
    {
        /// <summary>
        /// Willingness or unwillingness a unit has to get more items based only the itemNumber of that item.
        /// </summary>
        public static int PropensityToGetMoreItemsByQuantity(int itemNumber)
        {
            if (itemNumber > Params.TooManyItems) { return -Params.High; }
            if (itemNumber > Params.ModerateNumberOfItems) { return -Params.Mid; }
            if (itemNumber > Params.CoupleOfItems) { return -Params.Incremental; }
            if (itemNumber == 0) { return 10; }
            return 0;
        }
    } 
}
