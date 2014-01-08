using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;
using TacticsGame.AI.MaintenanceMode;
using TacticsGame.GameObjects.EntityMetadata;

namespace TacticsGame.Items
{
    public class LootGenerationEngine : IGenerateLoot
    {
        /// <summary>
        /// Can get up to 5 items based on luck
        /// </summary>
        private const int bonusItemCap = 5;

        /// <summary>
        /// Luck roll needed for first item
        /// </summary>
        private const int firstItemRoll = 60;


        private const int junkThreshold = 20;

        /// <summary>
        /// Luck + skill rolls needed to reach tier2
        /// </summary>
        private const int tier2Threshold = 85;

        /// <summary>
        /// Luck + skill roll needed for tier3
        /// </summary>
        private const int tier3Threshold = 95;

        /// <summary>
        /// Luck + skill needed for extra item
        /// </summary>
        private const int bonusItemRoll = 95;

        public IEnumerable<string> GetActivityLoot(IMakeDecisions unit, Decision activity, int modifier = 0)
        {
            switch (activity)
            {
                case Decision.GetLumber:
                    return this.GetLumberLoot(unit);
                case Decision.MineStone:
                    return this.GetMineStoneLoot(unit);
                case Decision.MineOre:
                    return this.GetMineOreLoot(unit);
                case Decision.Hunt:
                    return this.GetHuntLoot(unit);
                case Decision.Forage:
                    return this.GetForageLoot(unit, modifier);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets items based on tiers and rolls.
        /// </summary>
        private IEnumerable<string> GetItemsByCount(string[] tier1, string[] tier2, string[] tier3, string[] junkTier, IMakeDecisions unit, int skillRoll, int count)
        {
            List<string> items = new List<string>();            

            int numBonus = 1;
            while (numBonus < bonusItemCap)
            {
                if (unit.CurrentStats.LuckRoll() > bonusItemRoll)
                {
                    numBonus++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < numBonus + count; ++i)
            {
                int roll = unit.CurrentStats.LuckRoll() + skillRoll;

                if (junkTier != null && roll < junkThreshold)
                {
                    items.Add(junkTier.GetRandomItem());
                }
                else if (roll > tier3Threshold)
                {
                    items.Add(tier3.GetRandomItem());
                }
                else if (roll > tier2Threshold)
                {
                    items.Add(tier2.GetRandomItem());
                }
                else
                {
                    items.Add(tier1.GetRandomItem());
                }
            }

            return items;
        }

        /// <summary>
        /// Gets items based on tiers and rolls.
        /// </summary>
        private IEnumerable<string> GetItemsByRoll(string[] tier1, string[] tier2, string[] tier3, string[] junkTier, IMakeDecisions unit, int skillRoll)
        {
            List<string> items = new List<string>();
            if (unit.CurrentStats.LuckRoll() + skillRoll < firstItemRoll)
            {
                return items;
            }

            int numBonus = 1;
            while (numBonus < bonusItemCap)
            {
                if (unit.CurrentStats.LuckRoll() + skillRoll > bonusItemRoll)
                { 
                    numBonus++; 
                }
                else 
                {
                    break;
                }
            }

            for (int i = 0; i < numBonus; ++i)
            {
                int roll = unit.CurrentStats.LuckRoll() + skillRoll;

                if (junkTier != null && roll < junkThreshold)
                {
                    items.Add(junkTier.GetRandomItem());
                }
                else if (roll > tier3Threshold)
                {
                    items.Add(tier3.GetRandomItem());
                }
                else if (roll > tier2Threshold)
                {
                    items.Add(tier2.GetRandomItem());
                }
                else
                {
                    items.Add(tier1.GetRandomItem());
                }
            }

            return items;
        }

        private IEnumerable<string> GetForageLoot(IMakeDecisions unit, int itemCount)
        {
            string[] junk = new string[] { "Branch", "Talon", "Bark", "Bone", "Fur" };
            string[] tier1 = new string[] { "LeafCluster", "RedShroom", "GreenShroom", "BulbousRoot" };
            string[] tier2 = new string[] { "HerbCluster", "WhiteFlowers", "Root", "LeafCluster" };            
            string[] tier3 = new string[] { "YellowFlowers", "WildMushroom", "WhiteFlowers", "HerbCluster" };

            int skill = unit.CurrentStats.SkillRoll(UnitSkills.SkillType.Herbalism);
            return this.GetItemsByCount(tier1, tier2, tier3, junk, unit, skill, itemCount);
        }

        private IEnumerable<string> GetHuntLoot(IMakeDecisions unit)
        {            
            string[] tier1 = new string[] { "Bone", "Talon", "Fur", "Feather" };
            string[] tier2 = new string[] { "Ham", "Apple", "Leather", "Horn" };
            string[] tier3 = new string[] { "NiceFur", "Ham", "Horn" };

            int skill = unit.CurrentStats.SkillRoll(UnitSkills.SkillType.Woodsman);
            return this.GetItemsByRoll(tier1, tier2, tier3, null, unit, skill);
        }

        private IEnumerable<string> GetMineOreLoot(IMakeDecisions unit)
        {
            string[] tier1 = new string[] { "Bone", "Rubble", "Shale", "Flint" };
            string[] tier2 = new string[] { "Opal", "Coal", "Flint" };
            string[] tier3 = new string[] { "SmallCrystal", "UncutRuby", "UncutSapphire" };

            int skill = unit.CurrentStats.SkillRoll(UnitSkills.SkillType.Mining);            
            return this.GetItemsByRoll(tier1, tier2, tier3, null, unit, skill);
        }

        private IEnumerable<string> GetMineStoneLoot(IMakeDecisions unit)
        {
            string[] tier1 = new string[] { "Bone", "Rubble", "Shale", "Flint", "Coal" };
            string[] tier2 = new string[] { "Opal", "Flint", "Shale" };
            string[] tier3 = new string[] { "SmallCrystal" };

            int skill = unit.CurrentStats.SkillRoll(UnitSkills.SkillType.Mining);
            
            return this.GetItemsByRoll(tier1, tier2, tier3, null, unit, skill);
        }

        private IEnumerable<string> GetLumberLoot(IMakeDecisions unit)
        {
            string[] tier1 = new string[] { "Fur", "Branch", "Feather", "TreeResin", "Bone", "Bark" };
            string[] tier2 = new string[] { "TreeSap", "TreeResin", "Bark" };
            string[] tier3 = new string[] { "TreeSap" };

            int skill = unit.CurrentStats.SkillRoll(UnitSkills.SkillType.Woodsman);
            return this.GetItemsByRoll(tier1, tier2, tier3, null, unit, skill);
        }

        public IEnumerable<Item> GetCombatLoot(IDropLoot enemy)
        {
            return enemy.GetLootDrop();            
        }
    }
}
