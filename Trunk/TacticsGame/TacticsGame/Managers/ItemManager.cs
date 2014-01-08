using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility.Classes;
using TacticsGame.Items;
using System.Diagnostics;
using TacticsGame.Items.SpecialStats;
using TacticsGame.GameObjects.EntityMetadata;

namespace TacticsGame.Managers
{
    public class ItemManager : Singleton<ItemManager>
    {
        private List<Item> items = null;

        private ItemManager()
        {    
        }

        public void Initialize(List<GameResourceInfo> resources)
        {
            List<ItemResourceInfo> itemsResources = resources.Where(a => a.GetType() == typeof(ItemResourceInfo)).Cast<ItemResourceInfo>().ToList();
            this.items = new List<Item>();
            foreach (ItemResourceInfo itemResource in itemsResources)
            {
                this.items.Add(new Item(itemResource.Id));
            }
        }

        public List<Item> GenerateItemList(ItemType? itemType, Rarity? rarity)
        {
            List<Item> newItems = new List<Item>();
            this.items.ForEach(a => newItems.Add(a.Clone()));

            if (itemType.HasValue)
            {
                newItems = newItems.Where(a => (a.Stats.Type & itemType.Value) != 0).ToList();
            }

            if (rarity.HasValue)
            {
                newItems = newItems.Where(a => (a.Stats.Rarity & rarity.Value) != 0).ToList();
            }

            return newItems;
        }

        public List<Item> GenerateArmorList(Rarity? rarity, ArmorType? armorType, EquipmentSlot? armorSlot)
        {
            List<Item> newItems = new List<Item>();
            this.items.Where(a => a.Stats.Type == ItemType.Armor).ToList().ForEach(a => newItems.Add(a.Clone()));

            if (armorType.HasValue)
            {
                newItems = newItems.Where(a => (((ArmorStats)a.Stats).ArmorType & armorType.Value) != 0).ToList();
            }

            if (armorSlot.HasValue)
            {
                newItems = newItems.Where(a => (((ArmorStats)a.Stats).ArmorSlot & armorSlot.Value) != 0).ToList();
            }

            if (rarity.HasValue)
            {
                newItems = newItems.Where(a => (a.Stats.Rarity & rarity.Value) != 0).ToList();
            }

            return newItems;
        }

        public List<Item> GenerateWeaponList(Rarity? rarity, WeaponType? weaponType)
        {            
            List<Item> newItems = new List<Item>();
            this.items.Where(a => a.Stats.Type == ItemType.Weapon).ToList().ForEach(a => newItems.Add(a.Clone()));

            if (weaponType.HasValue)
            {
                newItems = newItems.Where(a => (((WeaponStats)a.Stats).WeaponType & weaponType.Value) != 0).ToList();
            }

            if (rarity.HasValue)
            {
                newItems = newItems.Where(a => (a.Stats.Rarity & rarity.Value) != 0).ToList();
            }

            return newItems;
        }
    }
}
