using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.Utility
{
    /// <summary>
    /// Counts occurances of the items put into it... only hashes items by their name, using ToString(), so you must overload ToString()!!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class CountingDictionary<T> 
    {
        private Dictionary<string, int> dict = new Dictionary<string, int>();

        public CountingDictionary() 
        {            
        }

        /// <summary>
        /// Gets a list of each item, by name, present in the dictionary, no repeats.
        /// </summary>
        public List<string> GetListOfItems()
        {
            return this.dict.Keys.ToList<string>();
        }

        /// <summary>
        /// Uses ToString() to count values with same name
        /// </summary>
        /// <param name="items"></param>
        public void AddValuesUsingNames(IEnumerable<T> items) 
        {
            foreach (T item in items)
            {
                this.AddItemUsingName(item);
            }
        }

        public void AddItemUsingName(T item)
        {
            if (!this.dict.ContainsKey(item.ToString()))
            {
                this.dict[item.ToString()] = 1;
            }
            else
            {
                this.dict[item.ToString()]++;
            }
        }


        public void AddItemsAsString(IEnumerable<string> items)
        {
            foreach (string item in items)
            {
                this.AddItemAsString(item);
            }
        }

        public void AddItemAsString(string item)
        {
            if (!this.dict.ContainsKey(item))
            {
                this.dict[item] = 1;
            }
            else
            {
                this.dict[item]++;
            }
        }

        /// <summary>
        /// Removes one unit of item from list.
        /// </summary>
        public void RemoveItem(T item)
        {
            if (this.dict.ContainsKey(item.ToString())) 
            {
                this.dict[item.ToString()]--;
            }
        }

        /// <summary>
        /// Removes one unit of item from list.
        /// </summary>
        public void RemoveItems(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                if (this.dict.ContainsKey(item.ToString()))
                {
                    this.dict[item.ToString()]--;
                }
            }
        }

        /// <summary>
        /// Gets how many of the item there are... by item name.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>The number of those items.</returns>
        public int GetItemCount(T item)
        {
            return this.GetItemCount(item.ToString());
        }

        public int GetItemCount(string item)
        {
            return this.dict.ContainsKey(item) ? this.dict[item] : 0;
        }

        /// <summary>
        /// Gets name of item with most abundance.
        /// </summary>
        /// <returns></returns>
        public string GetMaxItem()
        {
            int max = 0;
            string maxKey = string.Empty;
            foreach (string key in this.dict.Keys)
            {
                if (this.dict[key] > max)
                {
                    max = this.dict[key];
                    maxKey = key;
                }
            }

            return maxKey;
        }

        /// <summary>
        /// Gets number of most abundant item.
        /// </summary>
        /// <returns></returns>
        public int GetMaxValue()
        {
            int max = 0;
            foreach (string key in this.dict.Keys)
            {
                if (this.dict[key] > max)
                {
                    max = this.dict[key];
                }
            }

            return max;
        }
    }
}
