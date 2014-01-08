using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Abilities
{
    public enum AbilityType
    {
        Unknown,
        Self,
        TargetEnemy,
        TargetFriendly,
        SelfRadialAll,
        SelfRadialEnemy,
        SelfRadialFriendly,
        TargetRadialAll,
        TargetRadialEnemy,
        TargetRadialFriendly,
    }
}
