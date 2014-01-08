using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TacticsGame.Utility;
using System.Reflection;

namespace TacticsGame
{
    public class Utilities
    {
        public static Random RandomGenerator = new Random();

        private static Vector2 origin = new Vector2(0.0f,0.0f);

        /// <summary>
        /// This rectangle does not change positions based on camera
        /// </summary>
        public static void DrawFixedRectangle(Rectangle destination, Color color)
        {
            TextureManager.Instance.DrawRectangle(destination, color);
        }

        /// <summary>
        /// Rectangle that does change position based on camera.
        /// </summary>
        public static void DrawRectangle(Rectangle destination, Color color)
        {
            GameStateManager instance = GameStateManager.Instance;
            Rectangle camera = instance.CameraView;
            float zoom = instance.ZoomLevel;            

            if (camera.Intersects(destination))
            {
                int x = destination.X.Scale(instance.ZoomLevel) - camera.X;
                int y = destination.Y.Scale(instance.ZoomLevel) - camera.Y;
                int width = destination.Width.Scale(instance.ZoomLevel);
                int height = destination.Height.Scale(instance.ZoomLevel);                                                  
                Rectangle position = new Rectangle(x, y, width, height);
                TextureManager.Instance.DrawRectangle(position, color);
            }
        }

        public static void DrawTexture2D(Texture2D texture, Rectangle destination, Rectangle source, Color? color = null)
        {
            DrawTexture2D(texture, destination, source, color, 0.0f, 1.0f);
        }

        /// <summary>
        /// Draws the 2D texture
        /// </summary>
        /// <param name="texture">Texture to draw</param>
        /// <param name="destination">Where to draw, plus dimensions</param>
        /// <param name="color">Color. Null defaults to white</param>
        /// <param name="overrideCameraCheck">If false, will first check for camera intersection. Otherwise will just draw. Set to true if you know this is already inside the camera view.</param>
        public static void DrawTexture2D(Texture2D texture, Rectangle destination, Color? color = null, bool overrideCameraCheck = false)
        {
            DrawTexture2D(texture, destination, null, color, 0.0f, 1.0f, overrideCameraCheck);
        }

        public static void DrawTexture2D(Texture2D texture, Rectangle destination, Rectangle? source, Color? color = null, float rotation = 0.0f, float scale = 1.0f, bool overrideCameraCheck = false)
        {
            GameStateManager instance = GameStateManager.Instance;
            Rectangle camera = instance.CameraView;

            int destX = destination.X.Scale(instance.ZoomLevel);
            int destY = destination.Y.Scale(instance.ZoomLevel);            

            if (overrideCameraCheck || camera.Intersects(destination.CloneAndRelocate(destX, destY)))
            {                    
                Vector2 position; 
                
                scale = scale * instance.ZoomLevel;
                position = new Vector2(destX - camera.X, destY - camera.Y);                

                TextureManager.Instance.Draw(texture, position, source, !color.HasValue ? Color.White : color.Value, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public static void DrawText(string text, Vector2 position, Color? color = null, float scale = 1.0f, float rotation = 0.0f)
        {
            Rectangle camera = GameStateManager.Instance.CameraView;
            float zoom = GameStateManager.Instance.ZoomLevel;

            TextureManager.Instance.DrawText(text, new Vector2((position.X * zoom) - camera.X, (position.Y * zoom) - camera.Y), color.HasValue ? color.Value : Color.Red, scale, rotation);
        }

        public static void DrawDebugText(string text, Vector2 position)
        {
            TextureManager.Instance.DrawDebugText(text, position);
        }

        /// <summary>
        /// Gets a number in the range from "from" to "to", inclusive.
        /// </summary>
        public static int GetRandomNumber(int from, int to)
        {
            if (from == to)
            {
                return from;
            }

            return RandomGenerator.Next(from, to + 1);
        }

        public static string GenerateRandomName(string classType = null) 
        {
            return NamingUtilities.GenerateRandomName(classType);
        }        

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace, Type type)
        {
            Type[] types = assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith(nameSpace) && t.GetInterfaces().Contains(type)).ToArray();
            return types;
        }

        /// <summary>
        /// Given value, gets a percent of value which is anywhere from low% to high%.
        /// </summary>
        /// <returns></returns>
        public static int GetRangePercent(int value, int low, int high)
        {
            double percent = (double)GetRandomNumber(low, high) / 100.0d;
            return (int)((double)value * percent);
        }

        /// <summary>
        /// Gets percent
        /// </summary>
        /// <param name="value">Value to get percent of.</param>
        /// <param name="percent">Percent</param>
        /// <returns>Percent of value</returns>
        public static int GetPercent(int value, int percent)
        {
            double val = (double)percent / 100.0d;
            return (int)((double)value * val);
        }

        /// <summary>
        /// Gets a skewed random number such that max is the most likely and min in the least likely. 0.5d skew is a linear skew.
        /// </summary>
        /// <param name="min">Min value (least likely)</param>
        /// <param name="max">Max value (most likely)</param>
        /// <param name="skew">0.5 is linear skew. 1 is uniform distribution. 2 is all crazy.</param>
        public static int GetRightSkewedRandomNumber(int min, int max, double skew = 0.5d) 
        {
            max++;
            return (int)(Math.Pow(RandomGenerator.NextDouble(), skew) * (double)(max - min)) + min; 
        }

        /// <summary>
        /// Gets a skewed random number such that min is the most likely and max in the least likely. 0.5d skew is a linear skew.
        /// </summary>
        /// <param name="min">Min value (most likely)</param>
        /// <param name="max">Max value (least likely)</param>
        /// <param name="skew">0.5 is linear skew. 1 is uniform distribution. 2 is all crazy.</param>        
        public static int GetLeftSkewedRandomNumber(int min, int max, double skew = 0.5d)
        {
            max++;
            return (int)(((1 - Math.Pow(RandomGenerator.NextDouble(), skew)) * (double)(max - min)))+ min;
        }

        public static string[] EnumsToStringArray(params Enum[] enums) 
        {
            List<string> items = new List<string>();
            foreach (Enum item in enums)
            {
                items.Add(item.ToString());
            }

            return items.ToArray();
        }

        public static void DoWithPercentChance<T>(int chance, Action<T> action, T arg)
        {
            if (Utilities.GetRandomNumber(0, 100) < chance) 
            {
                action(arg);
            }            
        }

        public static void DoWithPercentChance(int chance, Action action)
        {
            if (Utilities.GetRandomNumber(0, 100) < chance) 
            {
                action();
            }            
        }        
    }
}
