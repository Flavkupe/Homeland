using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Items
{
    public class ItemStats
    {
        public int Cost { get; set; }

        public ItemType Type { get; set; }

        public Rarity Rarity { get; set; }

        private ItemMetadata metadata = ItemMetadata.None;

        public ItemMetadata Metadata
        {
            get { return metadata; }
            set { metadata = value; }
        }

        public string Description { get; set; }

        public ItemStats Clone()
        {
            return (ItemStats)this.MemberwiseClone();
        }

        /// <summary>
        /// How common the item is. Scrap is "5" and Artifact is "0". 
        /// </summary>
        public int RarityInverseValue { get { return (int)Rarity.Artifact - (int)this.Rarity; } } 
    }

    public enum Rarity
    {
        Scrap = 1,
        Common = 2,
        Uncommon = 4,
        Rare = 8,
        Ancient = 16,
        Artifact = 32,
    }

    public enum ItemType
    {
        Scrap = 1,
        Resource = 2,
        Weapon = 4,
        Armor = 8,         
        Luxury = 16,
        Food = 32,
        Consumable = 64,
        Misc = 128,
        Commodity = 256,
        Ingredient = 512,
    }
}
