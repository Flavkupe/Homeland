using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Buildings.Types;
using TacticsGame.GameObjects.Units;
using TacticsGame.Edicts;
using TacticsGame.Items;
using TacticsGame.World.Caravans;
using TacticsGame.GameObjects.Obstacles;
using TacticsGame.GameObjects.Zones;
using TacticsGame.Map;

namespace TacticsGame.World
{
    /// <summary>
    /// Collection of serializable stuff like taxes and item orders used for keeping track of stuff in each town that the player owns and 
    /// controls.
    /// </summary>
    [Serializable]
    public class TownState
    {
        [NonSerialized]
        public GuildHouse townGuildhouse;

        private List<Building> buildings = new List<Building>();

        private List<Unit> units = new List<Unit>();

        private List<ItemOrder> itemOrders = new List<ItemOrder>();

        private List<Edict> edicts = new List<Edict>();

        private List<DecisionMakingUnit> decisionMakingUnits = new List<DecisionMakingUnit>();

        private List<Caravan> caravans = new List<Caravan>();

        private List<Obstacle> obstacles = new List<Obstacle>();

        private List<Zone> zones = new List<Zone>();

        private TileGrid tileGrid = null;

        private int dailyTaxes = 0;

        private int visitorTaxes = 0;

        public List<Unit> Units
        {
            get { return units; }
            set { units = value; }
        }

        public List<Building> Buildings
        {
            get { return buildings; }
            set { buildings = value; }
        }

        public List<DecisionMakingUnit> DecisionMakingUnits
        {
            get { return decisionMakingUnits; }
            set { decisionMakingUnits = value; }
        }

        public List<Caravan> Caravans
        {
            get { return caravans; }
            set { caravans = value; }
        }

        public List<Obstacle> Obstacles
        {
            get { return obstacles; }
            set { obstacles = value; }
        }

        public List<Zone> Zones
        {
            get { return zones; }
            set { zones = value; }
        }

        public IEnumerable<ExitZone> ExitZones
        {
            get { return this.Zones.OfType<ExitZone>(); }
        }

        public TileGrid TileGrid
        {
            get { return tileGrid; }
            set { tileGrid = value; }
        }

        /// <summary>
        /// Gets the number of units the town can hold at the moment. 
        /// </summary>
        public int GetUnitCapacity()
        {
            int cap = 0;
            this.buildings.ForEach(a => cap += a.UnitCapacity);
            return cap;
        }

        /// <summary>
        /// Returns whether or not an edict is active.
        /// </summary>
        public bool EdictIsActive(EdictType edict)
        {
            return this.edicts.Any(a => a.ObjectName == edict.ToString());
        }


        public void ToggleEdict(EdictType edict)
        {
            if (EdictIsActive(edict))
            {                
                this.ToggleEdict(edict, false);                
            }
            else
            {
                this.ToggleEdict(edict, true);
            }
        }

        /// <summary>
        /// Toggles the edict on or off
        /// </summary>
        /// <param name="edict">Type of edict to toggle.</param>
        /// <param name="toggleOn">If true, toggles edict on. Else toggles edict off.</param>
        public void ToggleEdict(EdictType edict, bool toggleOn)
        {
            if (toggleOn)
            {
                if (!EdictIsActive(edict))
                {
                    this.edicts.Add(new Edict(edict));
                }
            }
            else
            {
                if (EdictIsActive(edict))
                {
                    this.edicts.Remove(this.edicts.First(a => a.ObjectName == edict.ToString()));
                }
            }
        }

        public List<ItemOrder> ItemOrders
        {
            get { return itemOrders; }
            set { itemOrders = value; }
        }

        // TEMP
        public int MaxEdicts { get { return 2; } }
        public int CurrentEdictCount { get { return this.edicts.Count; } }

        public int DailyTaxes
        {
            get { return dailyTaxes; }
            set { dailyTaxes = value; }
        }

        public int VisitorTaxes
        {
            get { return visitorTaxes; }
            set { visitorTaxes = value; }
        }

        public GuildHouse TownGuildhouse
        {
            get { return townGuildhouse; }
            set { townGuildhouse = value; }
        }        
    }
}
