using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects
{
    public interface ISubSprite
    {
        void AddSubSprite(ISubSprite subsprite);

        void Draw(GameTime time);

        void UpdateSprite(GameTime time);

        void SetDrawPosition(int x, int y);

        void ShiftDrawPosition(int x, int y);
    }
}
