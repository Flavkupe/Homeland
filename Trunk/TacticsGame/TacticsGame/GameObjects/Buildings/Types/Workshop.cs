using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Owners;
using TacticsGame.Utility;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Visitors;
using TacticsGame.GameObjects.Visitors.Types;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Buildings.Types
{
    [Serializable]
    public class Workshop : Building, IBuildable
    {
        private Crafter owner;

        public Workshop()
            : base("Building_Workshop")
        {
            this.owner = new Crafter();
            this.owner.OwnedBuilding = this;
            
            this.Stock.Money = 1000;                       
        }

        public override Inventory Stock
        {
            get { return this.owner.Inventory; }
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

        public override Owner Owner
        {
            get { return this.owner; }
        }

        public override bool IsBuildingThatSellsThings
        {
            get { return true; }
        }

        public int MoneyCost
        {
            get { return 200; }
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override bool IsBuildingThatBuysThings
        {
            get { return true; }
        }

        public List<ObjectValuePair<string>> ResourceCost
        {
            get
            {
                return new List<ObjectValuePair<string>>() { new ObjectValuePair<string>("Wood", 1),
                                                                      new ObjectValuePair<string>("Stone", 1) };
            }
        }

        /// <summary>
        /// Refreshes the building for the start of every turn.
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

            for (int i = 0; i < Utilities.GetRandomNumber(1, 2); ++i)
            {
                BottleTrader visitor = new BottleTrader();
                if (visitor.WillBeBuildingVisitor(this))
                {
                    this.Visitors.Add(visitor);
                }
            }
        }

        public bool BuildAgain
        {
            get { return false; }
        }        
    }
}
