using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility.Classes;
using TacticsGame.World;
using TacticsGame.Utility;

namespace TacticsGame.Managers
{
    public class WorldGenerationManager : Singleton<WorldGenerationManager>
    {
        private WorldGenerationManager()
        {
        }

        public GameWorld GenerateNewWorld()
        {
            GameWorld world = new GameWorld();
            world.WorldTime = new DateTime(100, 10, 10, 8, 0, 0, DateTimeKind.Unspecified);

            for(int i = 0; i < 8; ++i) 
            {
                ForeignTownInfo town = new ForeignTownInfo(NamingUtilities.GenerateTownName());                
                world.ForeignTowns.Add(town);                    
            }

            return world;
        }        
    }
}
