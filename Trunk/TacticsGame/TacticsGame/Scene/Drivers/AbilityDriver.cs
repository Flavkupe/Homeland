using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TacticsGame.Abilities;
using TacticsGame.GameObjects.Units;
using TacticsGame.Map;
using TacticsGame.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using TacticsGame.GameObjects.Effects;
using TacticsGame.GameObjects;

namespace TacticsGame.Scene
{
    /// <summary>
    /// A class that handles ability behavior for a scene
    /// </summary>
    public class AbilityDriver
    {
        public static AbilitySceneResults HandleAbilityEffects(AbilityInfo ability, Tile targetTile = null)
        {
            AbilityType type = ability.Stats.Type;
            Unit sourceUnit = ability.Owner;
            sourceUnit.CurrentStats.ActionPoints -= ability.APCost;

            AbilitySceneResults results = new AbilitySceneResults();
            if (type == AbilityType.Self)
            {
                AbilitySceneResults result = ApplyAbilityToTargetUnit(ability, sourceUnit);
                results.Accumulate(result);
            }
            else if (type == AbilityType.TargetEnemy)
            {
                Debug.Assert(targetTile != null && targetTile.TileResident is Unit);
                Unit targetUnit = targetTile.TileResident as Unit;
                Debug.Assert(targetUnit.IsHostile != sourceUnit.IsHostile, "Should not have accepted target as enemy.");
                AbilitySceneResults result = ApplyAbilityToTargetUnit(ability, targetUnit); // For most effects
                result.Accumulate(ApplyAbilityToTargetUnit(ability, sourceUnit)); // For effects that might also affect source
                result.Accumulate(ApplyConditionalAbilityEffectsToTargetUnit(ability, targetUnit, result));
                result.Accumulate(ApplyConditionalAbilityEffectsToTargetUnit(ability, sourceUnit, result));
                results.Accumulate(result);
            }
            else if (type == AbilityType.SelfRadialAll || type == AbilityType.TargetRadialAll)
            {
                Tile sourceTile = type == AbilityType.SelfRadialAll ? sourceUnit.CurrentTile : targetTile;
                AbilitySceneResults result = new AbilitySceneResults();
                HashSet<Tile> tiles = sourceTile.Grid.GetTileRadius(sourceTile, ability.Stats.MaxRange, true, ability.Stats.MinRange);

                foreach (Tile surrounding in tiles)
                {
                    if (surrounding.TileResident != null && surrounding.TileResident is Unit)
                    {
                        result.Accumulate(ApplyAbilityToTargetUnit(ability, surrounding.TileResident as Unit));
                    }
                }
            }

            return results;
        }

        private static AbilitySceneResults ApplyAbilityToTargetUnit(AbilityInfo ability, Unit targetUnit)
        {
            AbilitySceneResults result = new AbilitySceneResults();
            Unit sourceUnit = ability.Owner;

            foreach (AbilityVisualEffectInfo effect in ability.VisualEffects)
            {
                // TODO: actual target
                if (effect.VisualEffectType == VisualEffectType.Animation)
                {
                    AnimatedEffect animation = GenerateAnimatedEffect(effect, sourceUnit, targetUnit.CurrentTile);
                    result.Animations.Add(animation);
                }
            }
            
            foreach (AbilityEffect effect in ability.Stats.AbilityEffects)
            {
                int yAdjustment = 0;

                if (!EffectShouldHitTarget(effect, sourceUnit, targetUnit))
                {
                    continue;
                }

                if (effect.HPModification != 0)
                {
                    targetUnit.Sprite.AddSubSprite(CreateNumberText(targetUnit, effect.HPModification, Color.Red, Color.LightBlue, ref yAdjustment));
                    targetUnit.CurrentStats.HP += effect.HPModification; 
                }

                if (effect.APModification != 0)
                {
                    targetUnit.Sprite.AddSubSprite(CreateNumberText(targetUnit, effect.APModification, Color.LightGray, Color.Green, ref yAdjustment));
                    targetUnit.CurrentStats.ActionPoints += effect.APModification;
                }

                if (effect.MoraleModification != 0)
                {
                    targetUnit.Sprite.AddSubSprite(CreateNumberText(targetUnit, effect.MoraleModification, Color.Purple, Color.Blue, ref yAdjustment));
                    targetUnit.CurrentStats.Morale += effect.MoraleModification;
                }

                if (effect.StatusEffect != null)
                {
                    targetUnit.StatusEffects.AddStatus(effect.StatusEffect.Clone());
                }

                if (effect.CooldownEffect != null)
                {
                    if (effect.CooldownEffect.Condition == CooldownModificationEffect.CooldownEffectCondition.None)
                    {
                        HandleCooldownEffect(targetUnit, effect.CooldownEffect);
                    }
                }

                if (effect.DirectEffects.HasFlag(AbilityDirectEffect.SkipRestOfTurn))
                {
                    // TODO
                }
            }

            if (targetUnit.CurrentStats.HP <= 0)
            {
                result.KilledUnits.Add(targetUnit);                    
            }

            return result;
        }

        /// <summary>
        /// Apply ability effects that rely on a special condition, such as they might only be known after all other effects take place (such as OnKill). 
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="targetUnit"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static AbilitySceneResults ApplyConditionalAbilityEffectsToTargetUnit(AbilityInfo ability, Unit targetUnit, AbilitySceneResults result)
        {
            AbilitySceneResults results = result;
            Unit sourceUnit = ability.Owner;

            foreach (AbilityEffect effect in ability.Stats.AbilityEffects)
            {
                if (!EffectShouldHitTarget(effect, sourceUnit, targetUnit))
                {
                    continue;
                }

                if (effect.CooldownEffect != null)
                {
                    if (effect.CooldownEffect.Condition == CooldownModificationEffect.CooldownEffectCondition.OnKill && result.KilledUnits.Count > 0)
                    {
                        HandleCooldownEffect(targetUnit, effect.CooldownEffect);
                    }
                }
            }

            return results;
        }

        private static void HandleCooldownEffect(Unit targetUnit, CooldownModificationEffect effect)
        {           
            if (effect.Target == CooldownModificationEffect.TargetAbilityCooldown.RandomDisabled ||
                effect.Target == CooldownModificationEffect.TargetAbilityCooldown.RandomEnabled)
            {
                bool randomDisabled = effect.Target == CooldownModificationEffect.TargetAbilityCooldown.RandomDisabled;
                List<AbilityInfo> validItems = randomDisabled ? targetUnit.KnownAbilities.Where(HasCooldown).ToList() : targetUnit.KnownAbilities.Where(HasNoCooldown).ToList();
                if (validItems.Count > 0)
                {
                    AbilityInfo item = validItems.GetRandomItem();
                    item.Cooldown = GetModifiedCooldown(item.Cooldown, effect.Modification);
                }
            }
            else
            {
                foreach (AbilityInfo unitAbility in targetUnit.KnownAbilities.ToList())
                {
                    switch (effect.Target)
                    {
                        case CooldownModificationEffect.TargetAbilityCooldown.All:
                            unitAbility.Cooldown = GetModifiedCooldown(unitAbility.Cooldown, effect.Modification);
                            break;
                        case CooldownModificationEffect.TargetAbilityCooldown.AllSpells:
                            // TODO
                            break;
                        case CooldownModificationEffect.TargetAbilityCooldown.Specific:
                            Debug.Assert(!string.IsNullOrEmpty(effect.SpecificAbilityName), "Undefined AbilityName for specific");
                            if (string.Equals(effect.SpecificAbilityName, unitAbility.AbilityName, StringComparison.OrdinalIgnoreCase))
                            {
                                unitAbility.Cooldown = GetModifiedCooldown(unitAbility.Cooldown, effect.Modification);
                            }
                            break;
                        case CooldownModificationEffect.TargetAbilityCooldown.Select:
                            // TODO
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static int GetModifiedCooldown(int cooldown, int? modifier)
        {
            return Math.Max(0, (cooldown + modifier) ?? 0);
        }

        /// <summary>
        /// Simple callback
        /// </summary>
        private static bool HasCooldown(AbilityInfo ability)
        {
            return ability.Cooldown > 0;
        }

        /// <summary>
        /// Simple callback
        /// </summary>
        private static bool HasNoCooldown(AbilityInfo ability)
        {
            return ability.Cooldown == 0;
        }

        /// <summary>
        /// Returns true if the effect, caused by sourceUnit's ability, is supposed to affect the target. False otherwise.
        /// </summary>
        private static bool EffectShouldHitTarget(AbilityEffect effect, Unit sourceUnit, Unit targetUnit)
        {
            switch (effect.TargetType)
            {
                case AbilityTargetType.Any:
                    return true;
                case AbilityTargetType.Self:
                    return sourceUnit.ID == targetUnit.ID;
                case AbilityTargetType.Enemy:
                    return sourceUnit.IsHostile != targetUnit.IsHostile;                                    
                case AbilityTargetType.Ally:
                    return sourceUnit.IsHostile == targetUnit.IsHostile;
                case AbilityTargetType.EmptyTile:
                default:
                    return false;
            }
        }

        private static FloatingText CreateNumberText(Unit targetUnit, int number, Color negativeColor, Color positiveColor, ref int yAdjustment)
        {
            FloatingText text = FloatingText.CreateRandomlyHorizontalFloatingText(Math.Abs(number).ToString(), targetUnit.DrawPosition);
            text.Color = number < 0 ? negativeColor : positiveColor;
            text.DrawPosition = new Vector2(text.DrawPosition.X, text.DrawPosition.Y + yAdjustment);
            yAdjustment += 10;
            return text;
        }

        /// <summary>
        /// Returns whether the target tile is a valid target for the ability, based on the ability properties, the tile contents, and ability owner.
        /// </summary>
        /// <param name="abilityInfo">The ability being used.</param>
        /// <param name="targetTile">The ability target.</param>
        public static bool TileIsValidTarget(AbilityInfo abilityInfo, Tile targetTile)
        {            
            bool sourceIsHostile = abilityInfo.Owner.IsHostile;
            if (abilityInfo.Stats.TargetType == AbilityTargetType.Enemy)
            {
                return targetTile.TileResident != null && targetTile.TileResident is Unit && (sourceIsHostile != (targetTile.TileResident as Unit).IsHostile);
            }
            else if (abilityInfo.Stats.TargetType == AbilityTargetType.Ally)
            {
                return targetTile.TileResident != null && targetTile.TileResident is Unit && (sourceIsHostile == (targetTile.TileResident as Unit).IsHostile);
            }
            else if (abilityInfo.Stats.TargetType == AbilityTargetType.EmptyTile)
            {
                return targetTile.TileResident == null;
            }
            else 
            {
                // Always valid for Self and Any
                return true;
            }
        }

        public static AbilitySceneResults InitiateAbility(AbilityInfo ability, Unit source, Tile target)
        {
            AbilitySceneResults result = new AbilitySceneResults();

            HandleVisualEffects(ability, source, target, result);
          
            return result;
        }

        private static void HandleVisualEffects(AbilityInfo ability, Unit source, Tile target, AbilitySceneResults result)
        {
            foreach (AbilityVisualEffectInfo effect in ability.VisualEffects)
            {
                if (effect.VisualEffectType == VisualEffectType.LinearProjectile)
                {
                    ProjectileEffect projectile = GenerateAnimatedEffect(effect, source, target) as ProjectileEffect;
                    result.Projectiles.Add(projectile);
                }
            }
        }

        public static AnimatedEffect GenerateAnimatedEffect(AbilityVisualEffectInfo effect, Unit source, Tile target)
        {
            if (effect.VisualEffectType == VisualEffectType.LinearProjectile)
            {
                ProjectileEffect projectile = new ProjectileEffect(effect.EffectName, source.CurrentTile, target, 4); // TODO: 4
                projectile.Scale = effect.Scale; // TODO
                return projectile;
            }
            else if (effect.VisualEffectType == VisualEffectType.Animation)
            {
                AnimatedEffect animation = new AnimatedEffect(effect.EffectName, target, ResourceType.VisualEffect, effect.Cylces);
                return animation;
            }
            
            return null;            
        }
    }
}
