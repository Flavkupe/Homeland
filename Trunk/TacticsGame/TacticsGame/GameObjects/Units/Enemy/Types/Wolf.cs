using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Items;

namespace TacticsGame.GameObjects.Units.Enemy.Types
{
    public class Wolf : EnemyUnit
    {
        public Wolf()
            : base("BlackWolf")
        {            
        }

        protected override void InitializeStats()
        {
            this.BaseStats.GenerateRandomStats(10, 15);
            this.BaseStats.ActionPoints = 10;
            this.BaseStats.BaseAttack = 2;
            this.BaseStats.BaseAttackAP = 5;
            this.BaseStats.HP = 15;
        }

        protected override void InitializeEquipment()
        {            
            Utilities.DoWithPercentChance(95, this.Inventory.AddItem, new Item(ResourceId.Items.Fur));
            Utilities.DoWithPercentChance(30, this.Inventory.AddItem, new Item(ResourceId.Items.Fur));
            Utilities.DoWithPercentChance(60, this.Inventory.AddItem, new Item(ResourceId.Items.Bone));
            Utilities.DoWithPercentChance(10, this.Inventory.AddItem, new Item(ResourceId.Items.NiceFur));
            Utilities.DoWithPercentChance(20, this.Inventory.AddItem, new Item(ResourceId.Items.Talon));            
        }
    }
}
