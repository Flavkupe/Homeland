using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.Xml;
using Nuclex.UserInterface.Controls;
using Nuclex.UserInterface;
using System.Diagnostics;

namespace TacticsGame
{
    public static class ExtensionFunctions 
    {
        /// <summary>
        /// Nudge the UniRect by an offset
        /// </summary>
        public static UniRectangle NudgeClone(this UniRectangle rect, float offsetX, float offsetY)
        {
            if (rect != null)
            {
                return new UniRectangle(rect.Left + offsetX, rect.Top + offsetY, rect.Size.X, rect.Size.Y);
            }

            return rect;
        }

        /// <summary>
        /// Relocate the UniRect to position
        /// </summary>
        public static UniRectangle RelocateClone(this UniRectangle rect, float locX, float locY)
        {
            if (rect != null)
            {
                return new UniRectangle(locX, locY, rect.Size.X, rect.Size.Y);
            }

            return rect;
        }

        /// <summary>
        /// Creates a replica of this object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Rectangle Clone(this Rectangle source)
        {
            Rectangle rect = new Rectangle();
            rect.X = source.X;
            rect.Y = source.Y;
            rect.Width = source.Width;
            rect.Height = source.Height;
            return rect;
        }

        /// <summary>
        /// Creates a replica of this object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static RectangleF ToRectangleF(this Rectangle source)
        {
            RectangleF rect = new RectangleF();
            rect.X = source.X;
            rect.Y = source.Y;
            rect.Width = source.Width;
            rect.Height = source.Height;
            return rect;
        }

        /// <summary>
        /// Creates a replica of this object.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static UniRectangle ToUniRectangle(this Rectangle source)
        {
            UniRectangle rect = new UniRectangle();
            rect.Location.X = source.X;
            rect.Location.Y = source.Y;
            rect.Size.X = source.Width;
            rect.Size.Y = source.Height;
            return rect;
        }

        /// <summary>
        /// Gets a clone of this object with the offsets added to the top-left location.
        /// </summary>
        public static RectangleF OffsetClone(this RectangleF source, int offsetX, int offsetY)
        {
            return new RectangleF(source.X + offsetX, source.Y + offsetY, source.Width, source.Height);
        }

        /// <summary>
        /// Gets a clone of this object with the offsets added to the top-left location.
        /// </summary>
        public static RectangleF OffsetClone(this RectangleF source, float offsetX, float offsetY)
        {
            return new RectangleF(source.X + offsetX, source.Y + offsetY, source.Width, source.Height);
        }

        /// <summary>
        /// Gets a clone of this object with the widths and heights changed.
        /// </summary>
        public static RectangleF ResizeClone(this RectangleF source, int newWidth, int newHeight)
        {
            return new RectangleF(source.X, source.Y, newWidth, newHeight);
        }

        /// <summary>
        /// Gets a clone of this object with the coordinates set to (newX, newY)
        /// </summary>
        public static RectangleF RelocatedClone(this RectangleF source, int newX, int newY)
        {
            return new RectangleF(newX, newY, source.Width, source.Height);
        }

        public static int GetWidth(this UniRectangle rect, UniRectangle inRelationTo)
        {
            return (int)rect.Size.ToOffset(inRelationTo.Size.X.Offset, inRelationTo.Size.Y.Offset).X;
        }

        public static int GetHeight(this UniRectangle rect, UniRectangle inRelationTo)
        {
            return (int)rect.Size.ToOffset(inRelationTo.Size.X.Offset, inRelationTo.Size.Y.Offset).Y;
        } 

        /// <summary>
        /// Gets width of a control that only has an offset specified on bounds.
        /// </summary>
        public static int GetWidth(this UniRectangle rect) 
        {
            Debug.Assert(rect.Size.X.Fraction == 0.0f, "GetWidth will not return proper stuff!");
            return (int)rect.Size.X.Offset;
        }

        /// <summary>
        /// Gets height of a control that only has an offset specified on bounds.
        /// </summary>
        public static int GetHeight(this UniRectangle rect)
        {
            Debug.Assert(rect.Size.Y.Fraction == 0.0f, "GetWidth will not return proper stuff!");
            return (int)rect.Size.Y.Offset;
        }

        /// <summary>
        /// Adds nullable item to the list iff the item is not null.
        /// </summary>
        public static void AddIfNotNull<T>(this List<T> list, T item) where T : class
        {
            if (item != null)
            {
                list.Add(item);
            }
        }

        public static void AddIfNotExists<T>(this ICollection<T> collection, T item)
        {
            if (!collection.Contains(item))
            {
                collection.Add(item);
            }
        }

        public static void AddRangeIfNotExists<T>(this ICollection<T> collection, IEnumerable<T> list) 
        {
            foreach (T item in list) 
            {
                if (!collection.Contains(item))
                {
                    collection.Add(item);
                }
            }
        }

        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                stack.Push(item);
            }
        }

        public static string AttributeValue(this XmlElement element, string name, string defaultValue)
        {
            string attr = element.GetAttribute(name);
            return string.IsNullOrEmpty(attr) ? defaultValue : attr;
        }

        public static bool AttributeValueAsBool(this XmlElement element, string name, bool defaultValue)
        {
            string attr = element.GetAttribute(name);
            if (string.IsNullOrEmpty(attr))
            {
                return defaultValue;
            }

            bool result;
            return bool.TryParse(attr, out result) ? result : false;                                      
        }

        public static int AttributeValueAsInt(this XmlElement element, string name, int defaultValue)
        {
            string attr = element.GetAttribute(name);
            if (string.IsNullOrEmpty(attr))
            {
                return defaultValue;
            }

            int result;
            return int.TryParse(attr, out result) ? result : -1;
        }

        public static float AttributeValueAsFloat(this XmlElement element, string name, float defaultValue)
        {
            string attr = element.GetAttribute(name);
            if (string.IsNullOrEmpty(attr))
            {
                return defaultValue;
            }

            float result;
            return float.TryParse(attr, out result) ? result : -1.0f;
        }

        public static int? AttributeValueAsNullableInt(this XmlElement element, string name, int? defaultValue)
        {
            string attr = element.GetAttribute(name);
            if (string.IsNullOrEmpty(attr))
            {
                return defaultValue;
            }

            int result;
            return int.TryParse(attr, out result) ? result : -1;
        }

        public static T AttributeValueAsEnum<T>(this XmlElement element, string name, T defaultValue) where T : struct
        {
            string attr = element.GetAttribute(name);
            if (string.IsNullOrEmpty(attr))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse<T>(attr, out result) ? result : defaultValue;
        }

        public static void SetControlVisible(this Control source,  Control control, bool visibility)
        {
            if (!visibility && source.Children.Contains(control))
            {
                source.Children.Remove(control);
            }
            else if (visibility && !source.Children.Contains(control))
            {
                source.Children.Add(control);
            }
        }

        /// <summary>
        /// Gets a random item and removes it from the list.
        /// </summary>
        public static T PopRandomItem<T>(this List<T> list)
        {
            if (list == null)
            {
                return default(T);
            }

            int numElements = list.Count<T>();
            int random = Utilities.GetRandomNumber(0, numElements - 1);
            T item = list.ElementAt<T>(random);
            list.RemoveAt(random);
            return item;
        }

        public static T GetRandomItem<T>(this IEnumerable<T> list)
        {
            if (list == null || !list.Any())
            {
                return default(T);
            }

            int numElements = list.Count<T>();
            int random = Utilities.GetRandomNumber(0, numElements - 1);
            return list.ElementAt<T>(random);
        }

        public static List<T> GetRandomPick<T>(this IEnumerable<T> list, int count)
        {
            List<T> newList = new List<T>();
            for (int i = 0; i < count; ++i)
            {
                newList.Add(list.GetRandomItem());
            }

            return newList;                
        }

        /// <summary>
        /// Gets a random item such that lower indices in the list are more likely.
        /// </summary>
        public static T GetLeftSkewedRandomItem<T>(this IEnumerable<T> list)
        {
            if (list == null)
            {
                return default(T);
            }

            int numElements = list.Count<T>();
            int random = Utilities.GetLeftSkewedRandomNumber(0, numElements - 1);
            return list.ElementAt<T>(random);
        }

        public static int GetClampedValue(this int number, int low, int high)
        {
            if (number < low)
            {
                return Math.Min(low, high);
            }
            else if (number > high)
            {
                return Math.Max(low, high);
            }

            return number;            
        }

        public static void RemoveIfExists<T>(this ICollection<T> list, T itemToRemove)
        {
            if (list.Contains(itemToRemove))
            {
                list.Remove(itemToRemove);
            }
        }

        public static Rectangle CloneAndRelocate(this Rectangle rect, int newX, int newY)
        {
            return new Rectangle(newX, newY, rect.Width, rect.Height);
        }

        public static Rectangle CloneAndOffset(this Rectangle rect, int offsetX, int offsetY)
        {
            return new Rectangle(rect.X + offsetX, rect.Y + offsetY, rect.Width, rect.Height);
        }

        public static Rectangle CloneAndScale(this Rectangle rect, float scale)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width.Scale(scale), rect.Height.Scale(scale));
        }

        public static float GetClampedValue(this float number, float low, float high)
        {
            if (number < low)
            {
                return low;
            }
            else if (number > high)
            {
                return high;
            }

            return number;
        }

        /// <summary>
        /// Gets value of item in dictionary. If no such item, gets 0. 
        /// </summary>
        public static int GetValueOrZero<T>(this Dictionary<T, int> dict, T obj)
        {
            if (dict == null)
            {
                return 0;
            }

            if (dict.ContainsKey(obj)) 
            {
                return dict[obj];
            }

            return 0;
        }

        /// <summary>
        /// Increments the value of the dictionary, and returns the result. if the item doesnt exist, it sets value to increment. 
        /// </summary>
        /// <param name="dict">The dictionary.</param>
        /// <param name="obj">Object to increment.</param>
        /// <param name="increment">How much to increment by</param>
        /// <returns>Value after incrementing.</returns>
        public static int SetOrIncrement<T>(this Dictionary<T, int> dict, T obj, int increment)
        {
            if (dict.ContainsKey(obj))
            {
                dict[obj] += increment;
                return dict[obj];
            }
            else
            {
                dict[obj] = increment;
                return dict[obj];
            }            
        }

        public static T GetMaxItem<T>(this Dictionary<T, int> dict)
        {
            Debug.Assert(dict.Count > 0);

            int max = 0;
            T maxItem = dict.Keys.GetRandomItem<T>();
            foreach (T item in dict.Keys)
            {
                if (dict[item] > max)
                {
                    max = dict[item];
                    maxItem = item;
                }
            }

            return maxItem;
        }

        public static int GetMaxIndex<T>(this IEnumerable<T> list) where T : IComparable
        {
            int maxIndex = 0;
            int count = 0;
            foreach (T item in list)
            {
                if (item.CompareTo(maxIndex) > 0)
                {
                    maxIndex = count;
                }

                count++;
            }

            return maxIndex;
        }

        public static T GetMax<T>(this IEnumerable<ObjectValuePair<T>> list) where T : class
        {
            if (list.Count<ObjectValuePair<T>>() == 0)
            {
                return null;
            }

            T max = null;
            int maxVal = Int32.MinValue;
            int index = 0;
            foreach (ObjectValuePair<T> pair in list)
            {                
                if (pair.Value > maxVal)
                {
                    max = pair.Object;
                    maxVal = pair.Value;
                }

                index++;
            }

            return max;
        }

        /// <summary>
        /// Sets the value of the dictionary to value if it doesn't contain key, else it sets to the average of dict[key] and value.
        /// </summary>
        /// <param name="key">Key in the dictionary.</param>
        /// <param name="value">New value.</param>
        public static void SetOrAverage<T>(this Dictionary<T, int> dict, T key, int value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = (dict[key] + value) / 2;
            }
            else
            {
                dict[key] = value;
            }
        }

        /// <summary>
        /// Returns a value such that it's capped to be at least 'min'
        /// </summary>
        public static int MinCap(this int num, int min)
        {
            return Math.Max(num, min);
        }

        /// <summary>
        /// Returns a value such that it's capped to be at most 'max'
        /// </summary>
        public static int MaxCap(this int num, int max)
        {
            return Math.Min(num, max);
        }

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        /// <summary>
        /// Scales the num by a factor of scale. That is, multiplies num by scale and returns the truncated num.
        /// </summary>
        public static int Scale(this int num, float scale)
        {
            return scale == 1.0f ? num : (int)((float)num * scale);
        }

        /// <summary>
        /// Divides the num by a numToDivideBy, doing all the annoying casting. 
        /// </summary>
        public static int DivideBy(this int num, float numToDivideBy)
        {
            return numToDivideBy == 1.0f ? num : (int)((float)num / numToDivideBy);
        }

        public static float GetBottom(this Control control)
        {
            return control.Bounds.Bottom.Offset;
        }

        public static float GetTop(this Control control)
        {
            return control.Bounds.Top.Offset;
        }

        public static float GetLeft(this Control control)
        {
            return control.Bounds.Left.Offset;
        }

        public static float GetRight(this Control control)
        {
            return control.Bounds.Right.Offset;
        }
    }
}
