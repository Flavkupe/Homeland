using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.GameObjects.Units;

namespace TacticsGame.AI.CombatMode
{
    [Serializable]
    public class UnitCombatActivity
    {
        private Unit unit;
        private UnitState state;

        public Unit Unit
        {
            get { return unit; }
            set { unit = value; }
        }        

        public UnitState State
        {
            get { return state; }
            set { state = value; }
        }

        public UnitCombatActivity(Unit unit, UnitState state)
        {
            this.unit = unit;
            this.state = state;
        }

        public IconInfo GetActivityIcon()
        {
            switch (this.State)
            {
                case UnitState.Active:
                    return TextureManager.Instance.GetIconInfo("Focus");
                case UnitState.Wait:
                    return TextureManager.Instance.GetIconInfo("SandClock");
                case UnitState.Done:                    
                default:
                    return null;
            }            
        }
    }

    public enum UnitState
    {
        Active,
        Wait,
        Done,           
    }
}
