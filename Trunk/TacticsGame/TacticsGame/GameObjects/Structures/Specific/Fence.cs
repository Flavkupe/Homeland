using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.GameObjects.Buildings;
using System;

namespace TacticsGame.GameObjects.Structures.Specific
{
    [Serializable]
    public class Fence : NeighborDependentStructure, IBuildable
    {
        public Fence()
            : base("Fence")
        {
        }

        public int MoneyCost
        {
            get { return 1; }
        }

        public List<ObjectValuePair<string>> ResourceCost
        {
            get 
            {
                return new List<ObjectValuePair<string>>() { }; 
            }
        }

        public bool BuildAgain
        {
            get { return true; }
        }

        public IconInfo Icon
        {
            get { return this.GetEntityIcon(); }
        }
    }
}
