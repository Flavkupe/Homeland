using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using System.Diagnostics;
using TacticsGame.Items.SpecialStats;
using TacticsGame.GameObjects.Units.Classes;
using TacticsGame.Crafting;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.Managers;
using TacticsGame.Preference;

namespace TacticsGame.GameObjects.Units
{
    [Serializable]
    public abstract class DecisionMakingUnit : Unit, IMakeDecisions
    {
        private List<Recipe> knownRecipes = null;

        private bool isMakingADecision;

        private Decision previousDecision;

        private ActivityResult lastResult;

        private Preferences preferences = new Preferences();

        private Dictionary<string, int> transactionHistory = new Dictionary<string, int>();

        public DecisionMakingUnit(string unitType)
            : base(unitType)
        {
            this.IsMakingADecision = false;
            this.PreviousDecision = Decision.Idle;
        }

        /// <summary>
        /// Sets stats for a new round of management mode.
        /// </summary>
        public virtual void RefreshStatsForNewManagementModeTurn()
        {
            this.CurrentStats.ActionPoints = this.BaseStats.ActionPoints;

            if (this.BaseStats.HP != this.CurrentStats.HP)
            {
                // Lose up to 5 Action points based on your HP. 50% hp is 2 points, etc. 100% health loses 0 AP.
                this.CurrentStats.ActionPoints -= Math.Min((int)Math.Ceiling((double)this.BaseStats.HP / (double)this.CurrentStats.HP), 5);
            }

            this.TurnsSinceWeaponUpgrade++;
            this.TurnsSinceArmorUpgrade++;
        }

        /// <summary>
        /// Whether this unit can do something else this turn right now.
        /// </summary>
        public bool CanMakeDecision { get { return !this.IsMakingADecision && this.CurrentStats.ActionPoints > 0; } }

        public bool IsMakingADecision
        {
            get { return isMakingADecision; }
            set { isMakingADecision = value; }
        }        

        public ActivityResult LastResult
        {
            get { return lastResult; }
            set { lastResult = value; }
        }

        public Decision PreviousDecision
        {
            get { return previousDecision; }
            set { previousDecision = value; }
        }

        public Preferences Preferences { get { return preferences; } }

        public virtual bool OnlyBuysPreferredItemTypes
        {
            get { return false; }
        }

        public List<Recipe> KnownRecipes
        {
            get { return this.knownRecipes; }
            protected set { this.knownRecipes = value; }
        }

        /// <summary>
        /// Whether or not this unit is able to craft items.
        /// </summary>
        public bool CanCraft 
        { 
            get 
            {
                if (this.knownRecipes == null) 
                {
                    return false; 
                }

                foreach (Recipe recipe in this.knownRecipes)
                {
                    if (recipe.CanCraft(this.Inventory)) 
                    {
                        return true; 
                    }
                }

                return false;
            } 
        }

        public virtual bool WantsToUpgradeWeapon { get { return TurnsSinceWeaponUpgrade > 3 || this.Equipment.MissingWeapon; } }

        public virtual bool WantsToUpgradeArmor { get { return TurnsSinceArmorUpgrade > 3 || this.Equipment.MissingArmor; } }     

        /// <summary>
        /// Preferences for item type for unit. Can be overriden.
        /// </summary>
        public virtual Dictionary<ItemType, int> ItemTypePreference
        {
            get { return this.preferences.ItemPreference.ItemTypePreference; }
            protected set { this.preferences.ItemPreference.ItemTypePreference = value; }
        }
        
        /// <summary>
        /// Innate item preferences for the unit. Can be overriden.
        /// </summary>
        public virtual Dictionary<string, int> ItemPreference
        {
            get { return this.preferences.ItemPreference.ItemNamePreferences; }
            protected set { this.preferences.ItemPreference.ItemNamePreferences = value; }
        }        

        public virtual int PriceMarkupRange
        {
            get { return this.preferences.ItemPreference.PriceMarkupRange; }
            set { this.preferences.ItemPreference.PriceMarkupRange = value; }
        }

        /// <summary>
        /// Recorded history of transaction sales and purchases
        /// </summary>
        public Dictionary<string, int> TransactionHistory
        {
            get { return this.transactionHistory; }
        }

        /// <summary>
        /// Whether or not the unit is a trader by trade. Hireable units and travellers are not traders by trade.
        /// </summary>
        public virtual bool IsTrader { get { return true; } }

        public static DecisionMakingUnit CreateRandomUnit()
        {
            Type[] types = new Type[] { typeof(Fool), typeof(Ranger), typeof(Footman) };
            Type selection = types.GetRandomItem<Type>();
            return Activator.CreateInstance(selection) as DecisionMakingUnit;
        }

        /// <summary>
        /// Whether or not the visitor/hireable unit wants to or can visit the building.
        /// </summary>
        /// <param name="building"></param>
        /// <returns></returns>
        public bool WillBeBuildingVisitor(Building building)
        {
            int visitorTax = PlayerStateManager.Instance.ActiveTown.VisitorTaxes;
            if (this.Inventory.Money < visitorTax || this.Preferences.GovernancePreference.TaxTolerance < visitorTax)
            {
                return false;
            }

            return true;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (this.knownRecipes != null)
            {
                this.knownRecipes.ForEach(a => a.LoadContent());
            }
        }         
    }
}
