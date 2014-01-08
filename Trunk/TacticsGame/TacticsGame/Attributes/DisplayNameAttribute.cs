using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Attributes
{
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }
        public DisplayNameAttribute(string name)
        {
            this.DisplayName = name;
        }
    }
}
