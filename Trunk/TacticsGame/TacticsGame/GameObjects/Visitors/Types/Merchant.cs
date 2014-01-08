using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;
using TacticsGame.Utility;
using TacticsGame.GameObjects.EntityMetadata;

namespace TacticsGame.GameObjects.Visitors.Types
{
    [Serializable]
    public class Merchant : Visitor
    {
        /// <summary>
        /// Basic merchant
        /// </summary>
        public Merchant()
            : this("Shopkeep")
        {
            foreach (string itemName in ItemGenerationUtilities.GetRandomAssortment(Utilities.GetRandomNumber(5, 15)))
            {
                this.Inventory.AddItem(new Item(itemName));
            }

            this.Inventory.Money = Utilities.GetRandomNumber(200, 2000);

            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Appraisal, 2 + Utilities.GetRandomNumber(2, 4));
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Merchant, 4 + Utilities.GetRandomNumber(2, 4));
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Instructing, 3);

            this.CurrentStats.Cunning += Utilities.GetRandomNumber(0, 150);
            this.CurrentStats.Mental += Utilities.GetRandomNumber(0, 150);

            this.Preferences.GovernancePreference.TaxTolerance = Utilities.GetRandomNumber(0, 100);
        }

        public override bool SellsItems { get { return true; } }
        public override bool BuysItems { get { return true; } }

        /// <summary>
        /// Other type of merchant
        /// </summary>
        /// <param name="type"></param>
        public Merchant(string type)
            : base(type)
        {
        }
    }
}
