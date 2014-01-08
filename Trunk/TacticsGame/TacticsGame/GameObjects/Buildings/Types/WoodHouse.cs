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
    public class WoodHouse : Building, IBuildable
    {
        public WoodHouse() 
            : base("Building_WoodHouse")
        {
        }

        public override int UnitCapacity { get { return 3; } }

        public bool BuildAgain
        {
            get { return false; }
        }

        public override bool IsBuildingWithStock
        {
            get { return false; }
        }

        public override bool IsBuildingWithOwner
        {
            get { return false; }
        }

        public override bool IsBuildingWithUnits
        {
            get { return false; }
        }

        public override bool IsBuildingThatSellsThings
        {
            get { return false; }
        }

        public override bool IsBuildingThatBuysThings
        {
            get { return false; }
        }

        public override bool IsBuildingWithVisitors 
        { 
            get { return false; } 
        }

        public override Owner Owner
        {
            get { return null; }
        }        

        public List<ObjectValuePair<string>> ResourceCost
        {
            get
            {
                return new List<ObjectValuePair<string>>() { new ObjectValuePair<string>("Wood", 3),
                                                             new ObjectValuePair<string>("Stone", 2) };
            }
        }

        public int MoneyCost
        {
            get { return 100; }
        }

        public override Inventory Stock
        {
            get { return null; }
        }
    }
}
