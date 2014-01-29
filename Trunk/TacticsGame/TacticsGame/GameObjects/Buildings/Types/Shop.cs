using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Owners;
using TacticsGame.GameObjects.Visitors;
using TacticsGame.Utility;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Visitors.Types;
using TacticsGame.Managers;

namespace TacticsGame.GameObjects.Buildings.Types
{
    [Serializable]
    public class Shop : Building, IBuildable
    {
        public Shopkeep shopKeep;
        
        public Shop()
            : this(32)
        {
        }

        public Shop(int tileDimensions)
            : base("Building_Shop", tileDimensions)
        {
            shopKeep = new Shopkeep();
            shopKeep.OwnedBuilding = this;
            
            //this.Stock.AddItems(ItemGenerationUtilities.GetItemAssortment(Utilities.GetRandomNumber(20, 40)));
            //this.Stock.AddItems(ItemGenerationUtilities.GetJunkAssortment(Utilities.GetRandomNumber(5, 10)));            
            this.Stock.AddItems(ItemGenerationUtilities.GetItemAssortment(Utilities.GetRandomNumber(3, 6)));

            this.Stock.Money = 500;   
         
            // TEMP
            //this.RefreshAtStartOfTurn();
        }

        public override Inventory Stock { get { return this.shopKeep.Inventory; } }

        public override bool IsBuildingWithStock
        {
            get { return true; }
        }

        /// <summary>
        /// Refreshes the building for the start of every turn.
        /// </summary>
        public override void RefreshAtStartOfTurn()
        {
            this.Visitors = new List<DecisionMakingUnit>();
            for (int i = 0; i < Utilities.GetRandomNumber(2, 7); ++i)
            {
                Visitor visitor = Visitor.CreateRandomVisitor();
                if (visitor.WillBeBuildingVisitor(this))
                {
                    this.Visitors.Add(visitor);
                }
            }

            // TEMP
            if (GameStateManager.Instance.GameStatus.CurrentDay < 3)
            {                
                Visitor visitor = new WeaponTrader();
                if (visitor.WillBeBuildingVisitor(this))
                {
                    this.Visitors.Add(visitor);
                }                
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override bool IsBuildingWithOwner
        {
            get { return true; }
        }

        public override Owner Owner
        {
            get { return this.shopKeep; }
        }

        public bool BuildAgain
        {
            get { return false; }
        }

        public override bool IsBuildingWithUnits
        {
            get { return false; }
        }

        public override bool IsBuildingThatSellsThings
        {
            get { return true; }
        }

        public int MoneyCost
        {
            get { return 100; }
        }

        public override bool IsBuildingThatBuysThings
        {
            get { return true; }
        }

        public List<ObjectValuePair<string>> ResourceCost
        {
            get
            {
                return new List<ObjectValuePair<string>>() { new ObjectValuePair<string>("Wood", 2),
                                                             new ObjectValuePair<string>("Stone", 2) };
            }
        }        
    }
}
