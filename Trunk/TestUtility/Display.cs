using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestUtility
{
    public class Display<T>
    {
        public T Value { get; set; }
        public string Name { get; set; }

        public Display(T value, string name)
        {
            this.Value = value;
            this.Name = name;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
