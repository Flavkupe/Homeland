using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TacticsGame.GameObjects
{
    public class SubSpriteCollection : ISubSprite
    {
        private List<ISubSprite> subsprites = new List<ISubSprite>();

        public void AddSubSprite(ISubSprite subsprite)
        {
            this.subsprites.Add(subsprite);
        }

        public void Draw(GameTime time)
        {
            foreach (ISubSprite subsprite in this.subsprites)
            {
                subsprite.Draw(time);
            }
        }

        public void UpdateSprite(GameTime time)
        {
            foreach (ISubSprite subsprite in this.subsprites.ToList())
            {
                subsprite.UpdateSprite(time);

                if (subsprite is IExpire)
                {
                    if (((IExpire)subsprite).IsExpired)
                    {
                        this.subsprites.Remove(subsprite);
                    }                    
                }
            }
        }

        public void SetDrawPosition(int x, int y)
        {
            foreach (ISubSprite subsprite in this.subsprites)
            {
                subsprite.SetDrawPosition(x, y);
            }
        }

        public void ShiftDrawPosition(int x, int y)
        {
            foreach (ISubSprite subsprite in this.subsprites)
            {
                subsprite.ShiftDrawPosition(x, y);
            }
        }
    }
}
