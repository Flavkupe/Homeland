using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects.EntityMetadata
{
    [Serializable]
    public class UnitStatus
    {
        Dictionary<UnitStatusEffect, UnitStatusEffectInfo> status = new Dictionary<UnitStatusEffect, UnitStatusEffectInfo>();

        public void AddStatus(UnitStatusEffectInfo effect)
        {
            this.AddStatus(effect.Effect, effect.Duration, effect.Modifier);
        }

        public void SetStatus(UnitStatusEffectInfo effect)
        {
            this.SetStatus(effect.Effect, effect.Duration, effect.Modifier);
        }

        /// <summary>
        /// Sets the status to the value, or increments both duration and modifier if it was there before.
        /// </summary>
        public void AddStatus(UnitStatusEffect effect, int duration = 1, int modifier = 0)
        {
            if (!this.status.ContainsKey(effect) || this.status[effect] == null) 
            { 
                this.SetStatus(effect, duration, modifier); 
            }
            else 
            {
                this.status[effect].Duration += duration;
                this.status[effect].Modifier += modifier;
            }
        }

        /// <summary>
        /// Sets the status to the value, no matter what it was at before.
        /// </summary>
        public void SetStatus(UnitStatusEffect effect, int duration = 1, int modifier = 0)
        {
            this.status[effect] = new UnitStatusEffectInfo(effect, duration, modifier);            
        }

        /// <summary>
        /// Whether or not the unit has the status
        /// </summary>
        public bool HasStatus(UnitStatusEffect effect)
        {
            return this.status.ContainsKey(effect);
        }

        /// <summary>
        /// Returns the EffectState for the status. Value is 0, 0 is the status is not present.
        /// </summary>
        public UnitStatusEffectInfo GetStatus(UnitStatusEffect effect)
        {
            return this.status.ContainsKey(effect) ? this.status[effect] : new UnitStatusEffectInfo(effect, 0, 0);
        }

        /// <summary>
        /// Gets all active stati.
        /// </summary>
        public List<UnitStatusEffectInfo> GetAllStatuses()
        {
            return this.status.Values.ToList();
        }

        /// <summary>
        /// Removes effect from the list. Returns true if removed, false otherwise.
        /// </summary>
        public bool RemoveStatus(UnitStatusEffect effect)
        {
            if (this.HasStatus(effect))
            {
                this.status.Remove(effect);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the cooldowns on the status by removing 1 from the duration. If duration is less than 1, it will remove the effect.
        /// </summary>
        public void Update() 
        {
            foreach(UnitStatusEffect key in status.Keys.ToList()) 
            {
                UnitStatusEffectInfo effect = status[key];

                // Note the order of effects... if the duration is 1 right now, it will go to 0 and be removed next turn. Duration of 1 lasts for "this" turn *and* the next. Duration
                //  0 lasts for only the remainder of "this" turn.
                if (effect.Duration <= 0)
                {
                    this.status.Remove(key);
                }

                effect.Duration--;                
            }
        }
    }
    
    public enum UnitStatusEffect
    {
        FreeMove, // Effect of "Charge"
        MoveAfterAttacking,
        ActAgain,
        SkipTurn,
        Stun,
    }
}
