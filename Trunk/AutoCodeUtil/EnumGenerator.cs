using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace AutoCodeUtil
{
    public class EnumGenerator
    {

        #region CodeBlox       
        string capStart = @"
namespace TacticsGame
{
    public class ResourceId
    {          
        ";

        string capEnd = @"
    }    
}
";
        string enumBlockStart = @"
        public enum {0}
        {{          
            ";

        string enumBlockEnd = @"        
        }          
        ";

        #endregion

        public void Run()
        {
            string pathToXmls = @"..\..\..\TacticsGame\TacticsGame\Xml";

            List<Pair> codeBlocks = new List<Pair>();

            foreach (string xmlFilePath in Directory.GetFiles(pathToXmls))
            {
                if (Path.GetExtension(xmlFilePath).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = Path.GetFileNameWithoutExtension(xmlFilePath);
                    codeBlocks.Add(new Pair(fileName, this.GenerateCodeBlockForFile(xmlFilePath)));
                }
            }

            string pathToItemsFile = @"..\..\..\TacticsGame\TacticsGame\Managers\Resources\ResourceId.cs";
            using (FileStream stream = File.OpenWrite(pathToItemsFile))
            {
                using(StreamWriter writer = new StreamWriter(stream)) 
                {
                    writer.Write(this.capStart);

                    foreach (Pair item in codeBlocks)
                    {
                        writer.Write(this.enumBlockStart, item.Left);
                        writer.Write(item.Right);                        
                        writer.Write(this.enumBlockEnd);
                    }

                    writer.Write(this.capEnd);
                }
            }            
        }

        private string GenerateCodeBlockForFile(string xmlFilePath)
        {           
            List<string> itemNames = new List<string>();
            XmlDocument itemXml = new XmlDocument();
            itemXml.Load(xmlFilePath);

            if (itemXml.FirstChild == null || itemXml.FirstChild.NextSibling == null)
            {
                return null;
            }

            XmlElement topElement = itemXml.FirstChild.NextSibling as XmlElement;

            if (topElement == null)
            {
                return null;
            }
            
            foreach (XmlElement itemElement in topElement.GetElementsByTagName("*"))
            {                
                string id = itemElement.GetAttribute("Id");
                if (!string.IsNullOrEmpty(id))
                {
                    itemNames.Add("\t\t\t" + itemElement.GetAttribute("Id"));
                }                
            }

            return string.Join(",\n", itemNames).Trim();
        }
    }

    public class Pair
    {
        public Pair(string left, string right)
        {
             this.Right = right;
             this.Left = left;
        }
    
        public string Left { get; set; }
        public string Right { get; set; }        
    }
}
