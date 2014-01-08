using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Units
{
    public abstract class EnemyUnit : Unit, IDropLoot
    {
        public EnemyUnit(string unitType)
            : base(unitType)
        {
            this.PlayerCanCommand = false;            
        }

        public override bool IsHostile { get { return true; } }

        protected override void InitializeStats()
        {
            base.InitializeStats();

            this.BaseStats.HP = 10;
            this.BaseStats.ActionPoints = 3;            
        }

        public virtual IEnumerable<Item> GetLootDrop()
        {
            List<Item> items = this.Equipment.GetAllItems();
            items.AddRange(this.Inventory.Items.ToList());
            return items;
        }
    }
}
