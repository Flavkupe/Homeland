using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;

namespace TacticsGame.Preference
{
    [Serializable]
    public class ItemPreference
    {
        private int priceMarkupRange = 0;
        private int quantityIntoleranceModifier = 10;

        private bool onlyBuysPreferredItemTypes = false;     

        private Dictionary<string, int> preferenceByItemName = new Dictionary<string, int>();

        private Dictionary<ItemType, int> preferenceByItemType = new Dictionary<ItemType, int>();

        private Dictionary<Rarity, int> preferenceByItemRarity = new Dictionary<Rarity, int>();

        private Dictionary<ItemMetadata, int> preferenceByItemMetadata = new Dictionary<ItemMetadata, int>();

        /// <summary>
        /// Modifier to specify how little the unit is willing to tolerate repeats of items. 0 indicates they are indifferent to having more items, 10 is normal, 20 is highly intolerant 
        /// of having multiple repeat items, etc.
        /// </summary>
        public int QuantityIntoleranceModifier
        {
            get { return quantityIntoleranceModifier; }
            set { quantityIntoleranceModifier = value; }
        }

        public int PriceMarkupRange
        {
            get { return priceMarkupRange; }
            set { priceMarkupRange = value; }
        }

        public void SetPreference(string itemName, int preference)
        {
            this.preferenceByItemName[itemName] = preference;
        }

        public void SetPreference(ItemType itemType, int preference)
        {
            this.preferenceByItemType[itemType] = preference;
        }

        public void SetPreference(Rarity itemRarity, int preference)
        {
            this.preferenceByItemRarity[itemRarity] = preference;
        }

        public void SetPreference(ItemMetadata itemMetadata, int preference)
        {
            this.preferenceByItemMetadata[itemMetadata] = preference;
        }

        public int GetPreference(string itemName)
        {
            return this.preferenceByItemName.GetValueOrZero(itemName);
        }

        public int GetPreference(ItemType itemType)
        {
            return this.preferenceByItemType.GetValueOrZero(itemType);
        }

        public int GetPreference(Rarity rarity)
        {
            return this.preferenceByItemRarity.GetValueOrZero(rarity);
        }
        
        public int GetPreference(ItemMetadata itemMetadata)
        {
            return this.preferenceByItemMetadata.GetValueOrZero(itemMetadata);
        }

        public virtual int GetPreference(Item item)
        {
            return this.GetPreference(item.Stats.Rarity) +
                   this.GetPreference(item.ObjectName) +
                   this.GetPreference(item.Stats.Metadata) +
                   this.GetPreference(item.Stats.Type);
        }

        /// <summary>
        /// Gets the preference only by finding the maximum of all preference types, rather than adding the values all up.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetDominantPreference(Item item)
        {
            return Math.Max(Math.Max(this.GetPreference(item.Stats.Rarity), this.GetPreference(item.ObjectName)), 
                            Math.Max(this.GetPreference(item.Stats.Metadata), this.GetPreference(item.Stats.Type)));
        }

        public bool OnlyBuysPreferredItemTypes
        {
            get { return onlyBuysPreferredItemTypes; }
            set { onlyBuysPreferredItemTypes = value; }
        }

        public bool WillNotBuyItem(Item item)
        {
            return this.onlyBuysPreferredItemTypes == true && this.GetPreference(item) <= 0;
        }
    }
}
