using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Utility
{
    public static class NamingUtilities
    {
        public static string GenerateRandomName(string classType = null)
        {
            switch (classType)
            {
                case "Ranger":
                    return GenerateRangerName();
                case "Fool":
                    return GenerateFoolName();
                case "Footman":
                    return GenerateFootmanName();
                case "Shopkeep":
                    return GenerateShopkeepName();
                case "Traveller":
                    return GenerateTravellerName();
                case "JunkCollector":
                case "Crafter":
                    return GenerateScrapDealerName();
                case "Smithy":
                    return GenerateSmithyName();
                case "BottleTrader":
                    return GenerateBottleTraderName();
                default:                    
                    return GenericUnitNames.GetRandomItem();
            }
        }

        private static string GenerateBottleTraderName()
        {
            string[] firstName = { "Quartz", "Crystal", "Glass", "Silikin", "Bottelita", "Garrafa" };
            return firstName.GetRandomItem();
        }

        public static string GenerateTownName()
        {
            return new string[] { "Townsville", "Villagsville", "Townland", "Villageland", "Foreignaria", "Landland" }.GetRandomItem();            
        }

        private static string GenerateSmithyName()
        {
            string[] firstName = { "Hammer", "Tongs", "Anvil", "Forge", "Smelter", "Mallet", "Gauntlet", "Glove", "Ingot" };
            string[] lastName = { "Ironfist", "Ironshoes", "Ironhat", "Ironface", "Metalteeth", "Steeltoes", "Steelhands", "Metalman", "Forgefist", "Of the Forge", "Molteniron" };
            return Generate(firstName, lastName);
        }

        private static string GenerateScrapDealerName()
        {
            return new string[] { "Scrappy", "Scrubs", "Rubble", "Blinky", "Boing", "Blue", "Bubs", "Popeye", "Splinky" }.GetRandomItem();
        }

        private static string GenerateTravellerName()
        {
            string[] firstName = { "Pilgrim", "Walker", "Wanderer", "Will", "Bill", "Morgan", "Blue", "Gerald", "Foster" };
            string[] lastName = { "From Afar", "The Wanderer", "McWilkins", "Smith", "Wallace", "Wanderlust", "The Dreamer", "Wonka", "The Boot", "Talespinner", "Forester" };
            return Generate(firstName, lastName);
        }

        private static string Generate(string[] list)
        {
            return list.GetRandomItem<string>();
        }

        private static string Generate(string[] first, string[] last)
        {
            return string.Format("{0} {1}", first.GetRandomItem<string>(), last.GetRandomItem<string>());
        }

        private static string GenerateFootmanName()
        {
            string[] firstName = { "Pigsy", "Puggle", "Rufus", "Blanko", "Ponk", "Moby", "Bubs", "Piggle", "Piggly", "Piggy" };
            string[] lastName= { "McPigs", "McTot", "De La Cerda", "Wiggly", "Coilytail", "Wonkers", "McSwine", "McPork", "McOink", "McBoingle" };
            return Generate(firstName, lastName);
        }

        private static string GenerateShopkeepName()
        {
            string[] firstName = { "Old Man", "Bill", "Coins", "Dollar", "Dough", "Buck" };
            string[] lastName = { "Bing", "Beard", "O' Reilly", "McGavin", "McMoney", "Cash" };
            return Generate(firstName, lastName);
        }

        private static string GenerateFoolName()
        {
            return GenericUnitNames.GetRandomItem();
        }

        private static string GenerateRangerName()
        {
            string[] firstName = { "Wolfy", "Wolfsie", "Hound", "Red", "Rover", "Woof", "Arf", "Arty", "Titus" };
            string[] lastName = { "The Gray", "Of The Wolf", "Everlost", "Pathfinder", "McHound", "The Wanderer", "The Wolf", "Of The Woods" };  
            return Generate(firstName, lastName);
        }

        private static string[] GenericUnitNames = { "Bucky", "Flav", "Buggaroo", "Bop", "Iggy", "Pogo", "Pinkie", "Left-Hand", "Bongo", "Burlap", "Pasta",
                                               "Yoggy", "Tintin", "Tots", "Rin McPan", "Rollo", "Godfreid", "Rodrigo", "Pesto", "Liebritik", "Pajaritik", "Snuffles", "Wig", "Song Sok" };
    }
}
