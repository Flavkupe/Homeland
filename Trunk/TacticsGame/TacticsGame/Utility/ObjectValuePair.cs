using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame
{
    /// <summary>
    /// Object used to compare numerical values to other items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectValuePair<T> : IComparable<ObjectValuePair<T>> where T : class 
    {
        public T Object { get; set; }
        public int Value { get; set; }

        public ObjectValuePair(T obj, int value) 
        {
            this.Object = obj;
            this.Value = value;
        }

        public int CompareTo(ObjectValuePair<T> other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
