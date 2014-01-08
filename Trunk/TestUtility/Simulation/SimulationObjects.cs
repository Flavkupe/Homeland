using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Utility.Classes;
using TacticsGame;

namespace TestUtility
{
    using GameObjectByType = Dictionary<string, List<GameObject>>;
    using TacticsGame.GameObjects;
    using TacticsGame.GameObjects.Units;
    using TacticsGame.GameObjects.Visitors;
    using TacticsGame.GameObjects.Buildings;
    
    public class SimulationObjects : Singleton<SimulationObjects>
    {
        private GameObjectByType gameObjects = new GameObjectByType();        

        private SimulationObjects()
            : base()
        {
        }

        public GameObjectByType GameObjects
        {
            get { return gameObjects; }
            set { gameObjects = value; }
        }

        public void AddEntity(string entityTypeName, GameObject entity)
        {
            if (!gameObjects.ContainsKey(entityTypeName))
            {
                gameObjects[entityTypeName] = new List<GameObject>();
            }

            gameObjects[entityTypeName].Add(entity);
        }

        public List<GameObject> GetEntities(string entityTypeName)
        {
            return this.gameObjects.ContainsKey(entityTypeName) ? this.gameObjects[entityTypeName] : new List<GameObject>();
        }

        public List<T> GetEntities<T>(string entityTypeName) where T : GameObject
        {
            List<T> newList = new List<T>();
            foreach (GameObject item in this.GetEntities(entityTypeName)) 
            {
                if (item is T) 
                {
                    newList.Add((T)item);
                }
            }
            
            return newList;
        }        

        public void RemoveEntity(string entityTypeName, GameObject value)
        {
            this.GetEntities(entityTypeName).Remove(value);
        }

        public void LoadContents()
        {
            foreach (string key in this.GameObjects.Keys)
            {
                foreach (GameObject obj in this.GameObjects[key])
                {
                    obj.LoadContent();
                }
            }
            
            List<DecisionMakingUnit> decisionMakingUnits = SimulationObjects.Instance.GetEntities<DecisionMakingUnit>("DecisionMakingUnit");
            List<Visitor> visitors = SimulationObjects.Instance.GetEntities<Visitor>("Visitor");
            List<Building> buildings = SimulationObjects.Instance.GetEntities<Building>("Building");

            foreach (string key in this.GameObjects.Keys)
            {
                foreach (GameObject obj in this.GameObjects[key])
                {
                    if (obj is GameEntity)
                    {
                        ((GameEntity)obj).LoadReferencesFromLists(decisionMakingUnits, buildings);
                    }
                }
            }                                             
        }
    }
}
