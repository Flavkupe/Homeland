using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame
{
    public class ValueEventArgs<T> : EventArgs
    {
        public T Value { get; set; }
        public ValueEventArgs(T Value)
            : base()
        {
            this.Value = Value;
        }

        public ValueEventArgs()
            : base()
        {
        }
    }
}
