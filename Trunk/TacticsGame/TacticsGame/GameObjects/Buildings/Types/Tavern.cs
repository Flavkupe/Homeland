using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Owners;
using TacticsGame.Utility;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Visitors;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Buildings.Types
{
    [Serializable]
    public class Tavern : Building, IBuildable
    {
        private Shopkeep shopkeep = null;  

        public Tavern() 
            : base("Building_Tavern")
        {
            this.shopkeep = new Shopkeep();
            this.shopkeep.Inventory.AddItems(ItemGenerationUtilities.GetFoodAssortment(5));
            this.shopkeep.OwnedBuilding = this;           
        }

        public override Inventory Stock
        {
            get { return this.Owner.Inventory; }
        }

        public bool BuildAgain
        {
            get { return false; }
        }

        public override bool IsBuildingWithStock
        {
            get { return true; }
        }

        public override bool IsBuildingWithOwner
        {
            get { return true; }
        }

        public override bool IsBuildingWithUnits
        {
            get { return false; }
        }

        public override bool IsBuildingThatSellsThings
        {
            get { return true; }
        }

        public override bool IsBuildingThatBuysThings
        {
            get { return false; }
        }

        public override bool IsBuildingWithVisitors
        {
            get { return true; }
        }

        public override Owner Owner
        {
            get { return this.shopkeep; }
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Gets visitors and units and stuff.
        /// </summary>
        public override void RefreshAtStartOfTurn()
        {
            this.Visitors = new List<DecisionMakingUnit>();
            for (int i = 0; i < Utilities.GetRandomNumber(0, 2); ++i)
            {
                Visitor visitor = Visitor.CreateRandomVisitor();
                if (visitor.WillBeBuildingVisitor(this))
                {
                    this.Visitors.Add(visitor);
                }
            }

            for (int i = 0; i < Utilities.GetRandomNumber(2, 4); ++i)
            {
                DecisionMakingUnit unit = DecisionMakingUnit.CreateRandomUnit();
                if (unit.WillBeBuildingVisitor(this))
                {
                    this.Visitors.Add(unit);
                }
            }
        }

        public List<ObjectValuePair<string>> ResourceCost
        {
            get
            {
                return new List<ObjectValuePair<string>>() { new ObjectValuePair<string>("Wood", 2),
                                                             new ObjectValuePair<string>("Stone", 2) };
            }
        }

        public int MoneyCost
        {
            get { return 200; }
        }
    }
}
