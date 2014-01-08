using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TacticsGame.Managers
{
    public class GameSerializer
    {
        private static GameSerializer instance = null;

        private GameSerializer()
        {
        }

        public static GameSerializer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameSerializer();
                }

                return instance;
            }
        }

        public void Serialize(object obj, string fileName = "saveData.bin")
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        public object Deserialize(string fileName = "saveData.bin")
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            object obj = formatter.Deserialize(stream);
            stream.Close();

            return obj;
        }
    }
}
