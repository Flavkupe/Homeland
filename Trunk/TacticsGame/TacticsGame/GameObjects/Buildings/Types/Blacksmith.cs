using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Owners;
using TacticsGame.Utility;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Buildings.Types
{
    [Serializable]
    public class Blacksmith : Building, IBuildable
    {
        private Smithy smithy;

        public Blacksmith()
            : base("Building_Blacksmith")
        {
            this.smithy = new Smithy();
            this.smithy.OwnedBuilding = this;
            
            this.Stock.Money = 1000;
            //this.Stock.AddItems(ItemGenerationUtilities.GetArmorAssortment(5));
            //this.Stock.AddItems(ItemGenerationUtilities.GetWeaponAssortment(10));
        }

        public override Inventory Stock
        {
            get { return this.smithy.Inventory; }
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

        public override Owners.Owner Owner
        {
            get { return this.smithy; }
        }

        public override bool IsBuildingThatSellsThings
        {
            get { return true; }
        }

        public int MoneyCost
        {
            get { return 1000; }
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
                return new List<ObjectValuePair<string>>() { new ObjectValuePair<string>("Wood", 5),
                                                                      new ObjectValuePair<string>("Stone", 20) };
            }
        }

        public bool BuildAgain
        {
            get { return false; }
        }        
    }
}
