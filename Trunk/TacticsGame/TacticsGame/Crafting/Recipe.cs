using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Managers;
using System.Diagnostics;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;

namespace TacticsGame.Crafting
{
    [Serializable]
    public class Recipe : GameObject
    {
        [NonSerialized]
        protected TextureInfo textureInfo = null;

        [NonSerialized]
        List<ItemAndCost> ingredients;

        [NonSerialized]
        List<ItemAndCost> results;

        public Recipe(string recipeName)
            : base(recipeName, ResourceType.Recipe)
        {
            this.LoadContent();
        }

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

        /// <summary>
        /// Loads all the graphical stuff and other hard-to-serialize crap
        /// </summary>
        public override void LoadContent()
        {
            RecipeResourceInfo info = GameResourceManager.Instance.GetResourceByResourceType(this.ObjectName, ResourceType.Recipe) as RecipeResourceInfo;

            Debug.Assert(info != null, "Could not find resource!");

            this.textureInfo = info.TextureInfo;
            this.DisplayName = info.DisplayName;
            this.Ingredients = info.Ingredients;
            this.Results = info.Results;   

            Debug.Assert(this.textureInfo != null, "Item with no texture!");
        }

        public bool CanCraft(IMakeDecisions unit)
        {
            return this.CanCraft(unit.Inventory);
        }

        /// <summary>
        /// Whether the item can be crafted with the given inventory.
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public bool CanCraft(Inventory inventory)
        {
            foreach (ItemAndCost pair in this.ingredients)
            {
                if (inventory.GetItemCount(pair.Item) < pair.Number)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class ItemAndCost
    {
        public string Item { get; set; }
        public int Number { get; set; }

        public ItemAndCost(string item, int cost)
        {
            this.Item = item;
            this.Number = cost;
        }
    }
}
