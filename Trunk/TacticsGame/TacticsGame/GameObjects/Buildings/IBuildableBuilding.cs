using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TacticsGame.GameObjects.Buildings
{
    public interface IBuildableBuilding
    {        
        int MoneyCost { get; }

        List<ObjectValuePair<string>> ResourceCost { get; }                

        Texture2D Icon { get; }         
    }
}
