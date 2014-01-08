using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame
{
    public interface IExpire
    {
        bool IsExpired { get; }
    }
}
