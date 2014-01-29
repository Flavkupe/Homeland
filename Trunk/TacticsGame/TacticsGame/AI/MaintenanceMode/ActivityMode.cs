using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.AI.MaintenanceMode
{
    public enum ActivityState
    {
        PreparingToStartActivity,
        GoingToActivity,
        ReturningFromActivity,
        InActivity,
        AwaitingNextActivity,
        PreparingToReturnFromActivity,
        Idle,
        DoneWithActivity,
    }
}
