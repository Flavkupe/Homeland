using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Crafting;

namespace TacticsGame.Managers
{
    public class RecipeResourceInfo : GameResourceInfo
    {
        public RecipeResourceInfo()            
        {
        }

        [NonSerialized]
        List<ItemAndCost> ingredients;

        [NonSerialized]
        List<ItemAndCost> results;

        public List<ItemAndCost> Ingredients
        {
            get { return ingredients; }
            set { ingredients = value; }
        }

        public List<ItemAndCost> Results
        {
            get { return results; }
            set { results = value; }
        }
    }
}
