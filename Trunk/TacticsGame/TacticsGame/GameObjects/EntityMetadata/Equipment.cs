using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.EntityMetadata
{
    [Serializable]
    public class Equipment
    {
        Dictionary<EquipmentSlot, Item> items = new Dictionary<EquipmentSlot, Item>();

        public Equipment()
        {
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                this.items[slot] = null;
            }
        }        

        public Item EquippedItem(EquipmentSlot slot)
        {
            return this.items[slot];
        }

        public void EquipItem(Item item, EquipmentSlot slot)
        {
            this.items[slot] = item;
        }

        public Item this[EquipmentSlot i]
        {
            get { return this.items.ContainsKey(i) ? this.items[i] : null; }
            set { this.items[i] = value; }
        }

        public bool MissingWeapon { get { return this[EquipmentSlot.LeftHand] == null; } }

        public bool MissingArmor { get { return this[EquipmentSlot.Helm] == null || this[EquipmentSlot.Chest] == null || this[EquipmentSlot.Feet] == null; } }

        public Equipment Clone()
        {
            Equipment newEquipment = (Equipment)this.MemberwiseClone();            
            return newEquipment;
        }

        public void LoadContent()
        {
            foreach (EquipmentSlot slot in this.items.Keys)
            {
                if (this.items[slot] != null) 
                {
                    this.items[slot].LoadContent();
                }
            }
        }

        public Item GetWeapon()
        {
            if (this.items == null || this.items.Values == null || this.items.Values.Count == 0)
            {
                return null;
            } 

            return this[EquipmentSlot.LeftHand];
        }

        public List<Item> GetAllArmor()
        {
            if (this.items == null || this.items.Values == null || this.items.Values.Count == 0)
            {
                return new List<Item>();
            } 

            return this.items.Values.Where(a => a != null && a.Stats.Type == ItemType.Armor).ToList<Item>();            
        }

        public List<Item> GetAllItems()
        {
            return this.items.Values.ToList();
        }
    }
}
