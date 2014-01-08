using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework.Graphics;

namespace TacticsGame.UI
{
    /// <summary>Dialog that demonstrates the capabilities of the GUI library</summary>
    public partial class TileSelectionDialog : WindowControl
    {

        /// <summary>Initializes a new GUI demonstration dialog</summary>
        public TileSelectionDialog()
        {
            InitializeComponent();            
        }

        public string MainLabelText
        {
            get { return this.uxMainLabel.Text; }
            set { this.uxMainLabel.Text = value; }
        }

        public event EventHandler CloseButtonClicked;

        private void HandleCloseButtonClicked(object sender, EventArgs e)
        {
            if (this.CloseButtonClicked != null)
            {
                this.CloseButtonClicked(sender, e);
            }
        }

        public void SetTileIcon(Texture2D icon)
        {
            this.uxIconButton.ImageTexture = icon;
        }
    }    
   
    public partial class TileSelectionDialog
    {

        #region NOT Component Designer generated code

        /// <summary> 
        ///   Required method for user interface initialization -
        ///   do modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uxMainLabel = new Nuclex.UserInterface.Controls.LabelControl();            
            this.uxCloseButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();
            this.uxIconButton = new Nuclex.UserInterface.Controls.Desktop.ButtonControl();

            //
            // mainLabel
            //
            this.uxMainLabel.Text = "Hello World! This is a label.";
            this.uxMainLabel.Bounds = new UniRectangle(38.0f, 26.0f, 110.0f, 30.0f);
            
            //
            // closeButton
            //
            this.uxCloseButton.Bounds = new UniRectangle(
              new UniScalar(1.0f, -90.0f), new UniScalar(1.0f, -40.0f), 80, 24
            );
            this.uxCloseButton.Text = "Close";

            this.uxCloseButton.Pressed += this.HandleCloseButtonClicked;            

            //
            // iconbutton
            //
            this.uxIconButton.Bounds = new UniRectangle(new UniScalar(0.0f, 6.0f), new UniScalar(0.1f, 6.0f), 32, 32);
            this.uxIconButton.Enabled = false;

            //
            // TileSelectionDialog
            //
            this.Bounds = new UniRectangle(100.0f, 10.0f, 200.0f, 200.0f);

            Children.Add(this.uxIconButton);
            Children.Add(this.uxMainLabel);
            Children.Add(this.uxCloseButton);
        }

        #endregion // NOT Component Designer generated code

        /// <summary>A label used to display a 'hello world' message</summary>
        protected Nuclex.UserInterface.Controls.LabelControl uxMainLabel;

        /// <summary>Button which exits the dialog and discards the settings</summary>
        protected Nuclex.UserInterface.Controls.Desktop.ButtonControl uxCloseButton;

        /// <summary>Button which exits the dialog and discards the settings</summary>
        protected Nuclex.UserInterface.Controls.Desktop.ButtonControl uxIconButton;
    }
}
