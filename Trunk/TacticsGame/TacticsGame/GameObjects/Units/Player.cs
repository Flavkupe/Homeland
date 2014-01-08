using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TacticsGame.GameObjects.Units
{
    [Serializable]
    public class Player : DecisionMakingUnit
    {
        public Player()
            : base("Fool")
        {
        }

        public override string UnitClassDisplayName { get { return "Fool"; } }

        public override void LoadContent()
        {
            base.LoadContent();            
            this.pictureFrame = TextureManager.Instance.GetTextureInfo("Frame_Fool", ResourceType.MiscObject);
        }

        protected override void InitializeAbilities()
        {
            base.InitializeAbilities();
        }

        protected override void InitializeStats()
        {
        }        
    }
}
