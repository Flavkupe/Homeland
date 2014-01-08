using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Buildings;
using Microsoft.Xna.Framework.Graphics;

namespace TacticsGame.GameObjects.Structures.Specific
{
    [Serializable]
    public class Cobblestone : NeighborDependentStructure, IBuildable
    {
        public Cobblestone()
            : base("Cobblestone")
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
