using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Owners
{
    [Serializable]
    public class Shopkeep : Owner
    {
        public Shopkeep()
            : base("Shopkeep")
        {
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Appraisal, 4);
            this.CurrentStats.Skills.LevelSkill(UnitSkills.SkillType.Merchant, 5);

            this.ItemTypePreference.Add(ItemType.Misc, 20);
            this.ItemTypePreference.Add(ItemType.Commodity, 40);
            this.ItemTypePreference.Add(ItemType.Luxury, 20);
            this.ItemTypePreference.Add(ItemType.Resource, 20);

            this.PriceMarkupRange = 15;

            this.Preferences.ItemPreference.QuantityIntoleranceModifier = 2; // much more tolerant of large item quantities because of resellers
        }        
    }
}
