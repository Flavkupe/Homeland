using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TacticsGame.Managers;
using TacticsGame;

namespace TestUtility
{
    public partial class MasterControl : Form
    {
        public MasterControl()
        {
            TextureManager.Instance.SetToIgnoreTextures();
            GameResourceManager.Instance.LoadAllResources(true);

            InitializeComponent();
        }
    }
}
