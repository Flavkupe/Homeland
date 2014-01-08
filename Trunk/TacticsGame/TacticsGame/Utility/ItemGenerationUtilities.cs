using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;

namespace TacticsGame.Utility
{
    /// <summary>
    /// Tools for generating random items
    /// </summary>
    public static class ItemGenerationUtilities
    {
        private static string[] bottleMerchantItems = new string[] { "Bottle", "Bottle", "Vial" };

        private static string[] allResources = new string[] { "Wood", "Stone", "Leather" };

        private static string[] allFood = new string[] { "Apple", "Ham" };

        private static string[] weapons = new string[] { "Sword", "BasicDagger", "Spear", "BasicBow" };
        private static string[] armor = new string[] { "LeatherBoots", "ClothShirt", "LightCap" };

        private static string[] scrap = new string[] { "Fur", "Rags", "Feather", "Bone", "TreeResin", "Bark", "Branch", "Rubble", "Talon" };

        private static string[] commonResources = new string[] { "Wood", "Stone" };

        private static string[] common = new string[] { "Apple", "Sword", "Bottle", "LeatherBoots", "Spear", "BasicDagger", "BasicBow", "Wood", "Stone", "ClothShirt", "LightCap" };

        private static string[] uncommon = new string[] { "GoldIngot", "UncutSapphire", "UncutRuby", "Ruby", "Sapphire", "SmallCrystal", "NiceFur", "Horn" };

        private static string[] rare = new string[] { "Diamond" };

        //private static string[] level0WeaponAssortment = Utilities.EnumsToStringArray(ItemById.CrackedSword, ItemById.Hatchet, ItemById.SmallKnife, ItemById.CheapBow);  
        //private static string[] level1WeaponAssortment = Utilities.EnumsToStringArray(ItemById.BasicBow, ItemById.BasicDagger, ItemById.Sword, ItemById.Spear, ItemById.ShortSword);
        //private static string[] level2WeaponAssortment = Utilities.EnumsToStringArray(ItemById.BattleAxe, ItemById.Glaive, ItemById.SharpSaber, ItemById.ShinyAxe, ItemById.StrongBow);
        //private static string[] level1ArmorAssortment = Utilities.EnumsToStringArray(ItemById.LeatherBreastplate, ItemById.LightCap, ItemById.LeatherBoots);
        //private static string[] level2ArmorAssortment = Utilities.EnumsToStringArray(ItemById.IronBreastplate, ItemById.MetalBoots, ItemById.MetalCap);

        public static string[] GetRandomAssortment(int number)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < number; ++i)
            {
                int num = Utilities.GetRandomNumber(0, 100);
                if (num < 95)
                {
                    items.Add(common.GetRandomItem<string>());
                }
                else if (num < 99)
                {
                    items.Add(uncommon.GetRandomItem<string>());
                }
                else 
                {
                    items.Add(rare.GetRandomItem<string>());
                }
            }

            return items.ToArray();
        }

        public static IEnumerable<Item> GetItemAssortment(int number)
        {
            List<Item> itemList = new List<Item>();
            string[] items = GetRandomAssortment(number);
            foreach (string item in items)
            {
                itemList.Add(new Item(item));
            }

            return itemList;
        }        

        public static IEnumerable<Item> GetResourceAssortment(int number)
        {           
            return GetRange(commonResources, number);        
        }

        private static IEnumerable<Item> GetRange(string[] list, int number)
        {
            List<Item> itemList = new List<Item>();
            for (int i = 0; i < number; ++i)
            {
                itemList.Add(new Item(list.GetRandomItem<string>()));
            }

            return itemList;        
        }

        public static List<string> GetAllResources()
        {
            return new List<string>(allResources);
        }

        public static IEnumerable<Item> GetJunkAssortment(int number)
        {
            return GetRange(scrap, number);
        }

        public static IEnumerable<Item> GetWeaponAssortment(int number, params int[] tiers)
        {
            List<Item> list = new List<Item>();            
            int numLeft = number;

            if (tiers == null || tiers.Length == 0)
            {
                tiers = new int[] { 0, 1, 2 };                
            }

            for(int i = 0; i < number; ++i) 
            {
                int num = tiers.GetRandomItem();
                //switch (num)
                //{
                //    case 0:
                //        list.Add(this)
                //}
            }

            return GetRange(weapons, number);
        }

        public static IEnumerable<Item> GetArmorAssortment(int number)
        {
            return GetRange(armor, number);
        }

        public static IEnumerable<Item> GetFoodAssortment(int number)
        {
            return GetRange(allFood, number);
        }

        public static IEnumerable<Item> GetBottleTraderAssortment(int number)
        {
            return GetRange(bottleMerchantItems, number);
        }
    }
}
