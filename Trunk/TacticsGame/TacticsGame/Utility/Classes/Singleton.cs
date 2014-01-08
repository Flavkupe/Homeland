using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TacticsGame.Utility.Classes
{
    public abstract class Singleton<T> where T : class
    {
        private static T instance = null;

        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)Activator.CreateInstance(typeof(T), true);
                }

                return instance;
            }
        }
    }
}
