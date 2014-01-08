using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.EntityMetadata;
using TacticsGame.Map;
using Microsoft.Xna.Framework;
using TacticsGame.Utility;
using TacticsGame.Abilities;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.Units.Classes;
using TacticsGame.Items.SpecialStats;
using TacticsGame.Managers;

namespace TacticsGame.GameObjects.Units
{
    [Serializable]
    public abstract class Unit : GameEntity
    {        
        private List<AbilityInfo> knownAbilities = new List<AbilityInfo>();

        protected Inventory inventory = new Inventory();

        private Equipment equipment = new Equipment();

        private UnitStats baseStats;

        private UnitStats currentStats;

        private UnitStatus statusEffects = new UnitStatus();

        private int turnsSinceWeaponUpgrade = 0;
      
        private int turnsSinceArmorUpgrade = 0;

        private string unitClass;

        [NonSerialized]
        protected TextureInfo pictureFrame;

        public Unit(string unitType)
            : base(unitType, ResourceType.GameObject)
        {
            this.unitClass = unitType;
            this.PlayerCanCommand = true;                        

            this.BaseStats = new UnitStats();            
            
            this.InitializeAbilities();            
            this.InitializeStats();
            this.InitializePreferences();
            this.InitializeEquipment();
            this.LoadContent();

            this.CurrentStats = this.BaseStats.Clone();            

            this.DisplayName = this.GenerateUnitName();
        }        

        public string UnitClass
        {
            get { return unitClass; }            
        }
        

        public Equipment Equipment
        {
            get { return equipment; }
            set { equipment = value; }
        }

        public UnitStats BaseStats
        {
            get { return baseStats; }
            set { baseStats = value; }
        }
       
        public UnitStats CurrentStats
        {
            get { return currentStats; }
            set { currentStats = value; }
        }


        public UnitStatus StatusEffects
        {
            get { return this.statusEffects; }
            set { this.statusEffects = value; }
        }
        
        public Inventory Inventory 
        { 
            get { return this.inventory; }
            set { this.inventory = value; }
        }

        public virtual bool IsPlayer
        {
            get { return false; }
        }

        public int TurnsSinceWeaponUpgrade
        {
            get { return turnsSinceWeaponUpgrade; }
            protected set { turnsSinceWeaponUpgrade = value; }
        }

        public int TurnsSinceArmorUpgrade
        {
            get { return turnsSinceArmorUpgrade; }
            protected set { turnsSinceArmorUpgrade = value; }
        }

        /// <summary>
        /// List of weapons the character can equip... if null, can equip anything.
        /// </summary>
        protected virtual WeaponType[] WeaponRestriction { get { return null; } }

        /// <summary>
        /// List of armor types the character can equip... if null, can equip anything.
        /// </summary>
        protected virtual ArmorType[] ArmorRestriction { get { return null; } }        

        /// <summary>
        /// Whether or not this unit is able to attack right now, based on AP and other stat factors.
        /// </summary>
        public bool CanAttack { get { return this.CurrentStats.BaseAttackAP <= this.CurrentStats.ActionPoints; } }

        public virtual bool IsCombatUnit { get { return true; } }

        public virtual bool IsHostile { get { return false; } }

        public virtual string UnitClassDisplayName { get { return this.unitClass; } }

        /// <summary>
        /// The 96x96 detailed picture frame to represent this unit.
        /// </summary>
        public TextureInfo PictureFrame { get { return this.pictureFrame; } }

        /// <summary>
        /// list of abilities known by this unit
        /// </summary>
        public List<AbilityInfo> KnownAbilities { get { return knownAbilities; } }        

        /// <summary>
        /// Initializes the default abilities this unit would have.
        /// </summary>
        protected virtual void InitializeAbilities()
        {
            this.AddAbility(new AbilityInfo("Focus", this));
            this.AddAbility(new AbilityInfo("RockThrow", this));
        }

        /// <summary>
        /// Generates a random name for the unit
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateUnitName()
        {
            return Utilities.GenerateRandomName(this.unitClass);
        }

        /// <summary>
        /// Initializes the frame to something specific.
        /// </summary>
        public override void LoadContent() 
        {
            base.LoadContent();
            
            // load the item data
            this.inventory.LoadContent();

            this.equipment.LoadContent();

            this.pictureFrame = TextureManager.Instance.GetTextureInfo("Frame_Footsoldier", ResourceType.MiscObject);                           
        }        

        /// <summary>
        /// Initializes the unit stats.
        /// </summary>
        protected virtual void InitializeStats()
        {                        
            this.BaseStats.GenerateRandomStats(20, 40);            
        }

        protected virtual void InitializePreferences()
        { 
        }

        public void AddAbility(AbilityInfo ability)
        {
            this.knownAbilities.Add(ability);
        }

        /// <summary>
        /// Gets the ap if has weapon, or base ap otherwise.
        /// </summary>
        public int GetAttackAP()
        {
            Item weapon = this.Equipment[EquipmentSlot.LeftHand];
            if (weapon == null)
            {
                return this.CurrentStats.BaseAttackAP;
            }

            return (weapon.Stats as WeaponStats).APCost;
        }

        /// <summary>
        /// Get attack min range based on weapon equipped or nothing.
        /// </summary>        
        public int GetAttackMinRange()
        {
            Item weapon = this.Equipment[EquipmentSlot.LeftHand];
            if (weapon == null)
            {
                return this.CurrentStats.BaseAttackMinRange;
            }

            return (weapon.Stats as WeaponStats).RangeMin;
        }

        /// <summary>
        /// Get attack max range based on weapon equipped or nothing.
        /// </summary>        
        public int GetAttackMaxRange()
        {
            Item weapon = this.Equipment[EquipmentSlot.LeftHand];
            if (weapon == null)
            {
                return this.CurrentStats.BaseAttackRange;
            }

            return (weapon.Stats as WeaponStats).RangeMax;
        }

        /// <summary>
        /// Gets combined defense against any attacks.
        /// </summary>        
        public int GetDamageMitigation()
        {
            int defense = 0;
            foreach (Item item in this.Equipment.GetAllArmor())
            {
                defense += (item.Stats as ArmorStats).Defense;
            }

            return defense;
        }

        /// <summary>
        /// Gets the attack if has weapon, or base attack otherwise.
        /// </summary>        
        public int GetAttackRoll()
        {
            Item weapon = this.Equipment[EquipmentSlot.LeftHand];

            int baseDamage = this.CurrentStats.BaseAttack + Utilities.GetRandomNumber(0, 2);

            if (weapon == null)
            {
                return baseDamage;
            }

            return (weapon.Stats as WeaponStats).Attack + baseDamage;
        }

        ////////
        #region Gaining Items and Equipment
        ////////

        /// <summary>
        /// Checks if the item is an upgrade and equips if it is. Also returns true if it is an upgrade, false otherwise..
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool EquipItemIfUpgrade(Item item)
        {
            if (UpgradesEquipment)
            {
                EquipmentSlot slot;
                if (this.ItemIsUpgrade(item, item.Stats.Type, out slot))
                {
                    this.Equip(item, slot);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Equips item in slot and returns the replaced item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public Item Equip(Item item, EquipmentSlot? slot = null)
        {
            Item oldItem = null;
            if (slot == null) 
            {
                if (item.Stats.Type == ItemType.Armor)
                {
                    slot = ((ArmorStats)item.Stats).ArmorSlot;
                }
                else
                {
                    slot = EquipmentSlot.LeftHand;
                }
            }
            
            switch (slot.Value) 
            {
                case EquipmentSlot.LeftHand:
                    oldItem = this.Equipment[EquipmentSlot.LeftHand];
                    this.Equipment[EquipmentSlot.LeftHand] = item;
                    this.turnsSinceWeaponUpgrade = 0;
                    break;
                default:
                    oldItem = this.Equipment[slot.Value];
                    this.Equipment[slot.Value] = item;
                    this.turnsSinceArmorUpgrade = 0;
                    break;
            }                               

            if (oldItem != null)
            {
                this.Inventory.AddItem(oldItem);
            }

            return oldItem;
        }

        /// <summary>
        /// Whether this unit cares about equipment at all
        /// </summary>
        public virtual bool UpgradesEquipment { get { return false; } }   

        /// <summary>
        /// Whether the item is considered by the unit to be an upgrade for itemType. If itemType is null, it checks if the item is an upgrade for any type.
        /// </summary>
        /// <returns></returns>
        public bool ItemIsUpgrade(Item item, ItemType? itemType = null)
        {
            EquipmentSlot dummy;
            return this.ItemIsUpgrade(item, itemType ?? item.Stats.Type, out dummy);
        }

        /// <summary>
        /// Checks whether or not the passed argument is an upgrade to the equipment.
        /// </summary>
        private bool ItemIsUpgrade(Item item, ItemType upgradeWanted, out EquipmentSlot slot)
        {
            slot = EquipmentSlot.None;
            if (upgradeWanted == ItemType.Weapon && item.Stats.Type == ItemType.Weapon)
            {
                WeaponStats stats = (WeaponStats)item.Stats;
                if (this.WeaponRestriction != null && !this.WeaponRestriction.Contains(stats.WeaponType))
                {
                    return false;
                }

                // TODO: right hand, 2handed weapons
                if (this.Equipment[EquipmentSlot.LeftHand] == null)
                {
                    slot = EquipmentSlot.LeftHand;
                    return true;
                }

                WeaponStats currentWeapon = (WeaponStats)this.Equipment[EquipmentSlot.LeftHand].Stats;
                int rating = stats.Attack - currentWeapon.Attack; // Higher attack is better
                rating += (currentWeapon.APCost - stats.APCost) * 2; // Less AP cost is much better
                rating += currentWeapon.WeaponType == this.BaseStats.WeaponTypePreference ? 2 * rating : 0; // Double rating for preferred weapon types.
                slot = EquipmentSlot.LeftHand;
                return rating > 0;
            }
            else if (upgradeWanted == ItemType.Armor && item.Stats.Type == ItemType.Armor)
            {
                ArmorStats stats = (ArmorStats)item.Stats;
                Item equippedItem = this.Equipment[stats.ArmorSlot];
                slot = stats.ArmorSlot;
                if (equippedItem == null)
                {
                    // Nothing equipped
                    return true;
                }

                ArmorStats equippedStats = equippedItem.Stats as ArmorStats;

                if (stats.Defense > equippedStats.Defense)
                {
                    // Better defense.
                    return true;
                }

                return false;
            }

            // Not weapon or armor
            return false;
        }

        public void AcquireItems(List<Item> list, AcquiredItemSource source)
        {
            list.ForEach(a => this.AcquireItem(a, source));
        }

        public void AcquireItems(int number, string itemName, AcquiredItemSource source)
        {
            for (int i = 0; i < number; ++i)
            {
                this.AcquireItem(new Item(itemName), source);
            }
        }

        public void AcquireItems(IEnumerable<string> items, AcquiredItemSource source)
        {
            foreach (string item in items)
            {
                this.AcquireItem(new Item(item), source);
            }
        }

        public virtual void AcquireItem(Item item, AcquiredItemSource source)
        {
            if (!this.EquipItemIfUpgrade(item))
            {
                this.Inventory.AddItem(item);
            }
        }

        /// <summary>
        /// For trading, whether this unit has an item that the other unit can use as upgrade
        /// </summary>
        /// <param name="decisionMakingUnit"></param>
        /// <returns></returns>
        public bool HasItemUpgradeInStock(DecisionMakingUnit unitThatWantsUpgrade, ItemType typeForUpgrade)
        {
            return this.GetListOfItemUpgrades(unitThatWantsUpgrade, typeForUpgrade).Count > 0;
        }

        public List<Item> GetListOfItemUpgrades(DecisionMakingUnit unitThatWantsUpgrade, ItemType typeForUpgrade)
        {
            List<Item> upgrades = new List<Item>();
            foreach (Item item in this.Inventory.Items)
            {
                if (unitThatWantsUpgrade.ItemIsUpgrade(item, typeForUpgrade))
                {
                    upgrades.Add(item);
                }
            }

            return upgrades;
        }

        /// <summary>
        /// Whether or not the unit can equip the item.
        /// </summary>
        public bool CanEquipItem(Item item)
        {
            if (item.Stats.Type == ItemType.Weapon) 
            {
                return this.WeaponRestriction == null || this.WeaponRestriction.Contains((item.Stats as WeaponStats).WeaponType);
            }
            else if (item.Stats.Type == ItemType.Armor)
            {
                return this.ArmorRestriction == null || this.ArmorRestriction.Contains((item.Stats as ArmorStats).ArmorType);
            }

            return false;
        }

        protected virtual void InitializeEquipment()
        {
            Item item = ItemManager.Instance.GenerateWeaponList(Rarity.Scrap, this.BaseStats.WeaponTypePreference).GetRandomItem();
            
            if (item != null) 
            {
                this.AcquireItem(item, AcquiredItemSource.Initial);
            }

            if (Utilities.GetRandomNumber(0, 2) == 2)
            {
                Item item2 = ItemManager.Instance.GenerateArmorList(Rarity.Scrap, ArmorType.Light, null).GetRandomItem();
                if (item2 != null) 
                { 
                    this.AcquireItem(item2, AcquiredItemSource.Initial); 
                }
            }

            this.AcquireItems(ItemManager.Instance.GenerateItemList(ItemType.Food, null).GetRandomPick(Utilities.GetRandomNumber(1, 3)), AcquiredItemSource.Initial);
        }

        ////////
        #endregion
        ////////       

        protected override void ReachedTransitionDestination(Tile transitionTarget)
        {
            this.CurrentStats.ActionPoints -= transitionTarget.Cost;
            base.ReachedTransitionDestination(transitionTarget);
        }

        public void PrepareForNextCombatTurn()
        {
            this.CurrentStats.ActionPoints = this.BaseStats.ActionPoints;

            foreach (AbilityInfo ability in this.knownAbilities)
            {
                if (ability.Cooldown > 0)
                {
                    ability.Cooldown--;
                }
            }

            this.StatusEffects.Update();            
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime time)
        {
            if (this.CurrentDrawMode == DrawMode.Waiting)
            {
                this.Animated = false;
                this.colorFilter = new Color(100, 50, 100, 200);
            }
            else if (this.CurrentDrawMode == DrawMode.Done)
            {
                this.Animated = false;
                this.colorFilter = new Color(0, 0, 0, 200);
            }
            else
            {
                this.Animated = true;
                this.colorFilter = null;
            }

            base.Draw(time);

            // Draw HP
            int hpWidth = Math.Max(0, (int)(((float)this.CurrentStats.HP / (float)this.BaseStats.HP) * (float)this.DrawPosition.Width - 4));
            Utilities.DrawRectangle(new Rectangle(this.DrawPosition.Left + 2, this.DrawPosition.Top - 4, hpWidth, 2), Color.Red);
        }        
    }
}
