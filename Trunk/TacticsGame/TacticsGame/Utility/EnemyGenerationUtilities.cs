using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units.Enemy.Types;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.Utility
{
    public static class EnemyGenerationUtilities
    {
        private static Type[] EasyEnemies = new Type[] { typeof(Wolf), typeof(Bandit) };

        private static List<EnemyUnit> GetEnemies(Type[] list, int number)
        {
            List<EnemyUnit> newList = new List<EnemyUnit>();

            for (int i = 0; i < number; ++i)
            {
                Type current = list.GetRandomItem<Type>();
                newList.Add((EnemyUnit)Activator.CreateInstance(current));
            }

            return newList;
        }

        /// <summary>
        /// Gets a bundle of easy enemies.
        /// </summary>
        public static List<EnemyUnit> GetEasyEnemies(int number)
        {
            return GetEnemies(EasyEnemies, number);
        } 

    }
}
