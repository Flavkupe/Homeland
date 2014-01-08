using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame
{
    public class EventArgsEx<T> : EventArgs
    {
        public T Value { get; set; }

        public object Value2 { get; set; }

        public EventArgsEx(T value) 
        {
            this.Value = value;
        }
    }
}
