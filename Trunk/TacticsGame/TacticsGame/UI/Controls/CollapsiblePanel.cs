using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface.Controls.Desktop;
using Nuclex.UserInterface;
using Microsoft.Xna.Framework.Graphics;
using TacticsGame.GameObjects;
using Nuclex.UserInterface.Controls.Arcade;
using Nuclex.UserInterface.Controls;
using TacticsGame.GameObjects.Units;
using Microsoft.Xna.Framework;

namespace TacticsGame.UI
{
    /// <summary>Dialog that demonstrates the capabilities of the GUI library</summary>
    public partial class CollapsiblePanel : WindowControl
    {
        private UniRectangle startingBounds;

        private UniRectangle collapsedBounds;

        private UniRectangle bigBounds;

        private bool embiggened = false;

        private bool collapsed = false;                

        /// <summary>Initializes a new GUI demonstration dialog</summary>
        public CollapsiblePanel(UniRectangle startingBounds, UniRectangle collapsedBounds)
        {
            InitializeComponent();
            
            this.startingBounds = startingBounds;
            this.collapsedBounds = collapsedBounds;
            this.bigBounds = new UniRectangle(startingBounds.Location.X, startingBounds.Location.Y - 300.0f, startingBounds.Size.X, startingBounds.Size.Y + 300.0f);
            this.Bounds = this.startingBounds;
            this.SetButtonBounds();
            this.EnableDragging = false;
            this.AffectsOrdering = false;
        }

        private void HandleDragButtonClicked(object sender, EventArgs e)
        {
            this.Bounds = embiggened ? this.bigBounds : this.startingBounds;

            if (this.EnableDragging == false)
            {
                this.EnableDragging = true;                
            }
            else
            {
                this.EnableDragging = false;                
            }

            this.AffectsOrdering = this.EnableDragging;
        }

        private void HandleMakeBiggerButtonClicked(object sender, EventArgs e)
        {
            this.ToggleBigger();
        }

        private void HandleCollapseButtonClicked(object sender, EventArgs e)
        {
            if (collapsed)
            {
                this.Bounds = embiggened ? this.bigBounds : this.startingBounds;
                this.uxCollapseButton.Text = "-";
            }
            else
            {
                this.Bounds = this.collapsedBounds;
                this.uxCollapseButton.Text = string.Empty;
            }

            this.SetButtonBounds();
            this.collapsed = !this.collapsed;
            this.uxDragButton.Selected = false;
            this.EnableDragging = false; 
        }

        protected virtual void ToggleBigger()
        {
            if (this.embiggened)
            {
                this.embiggened = false;
                this.Bounds = this.startingBounds;
            }
            else
            {
                this.embiggened = true;
                this.Bounds = this.bigBounds;
            }

            this.SetButtonBounds();
        }

        private void SetButtonBounds()
        {
            this.uxCollapseButton.Bounds = new UniRectangle(new UniScalar(1.0f, -19.0f), new UniScalar(0.0f, 3.0f), 16.0f, 16.0f);
            this.uxMakeBiggerButton.Bounds = new UniRectangle(new UniScalar(1.0f, -41.0f), new UniScalar(0.0f, 3.0f), 16.0f, 16.0f);
            this.uxDragButton.Bounds = new UniRectangle(new UniScalar(1.0f, -63.0f), new UniScalar(0.0f, 3.0f), 16.0f, 16.0f);
            
        }
    }

    public partial class CollapsiblePanel
    {

        #region NOT Component Designer generated code

        /// <summary> 
        ///   Required method for user interface initialization -
        ///   do modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            
            this.uxCollapseButton = new ButtonControl();            
            this.uxCollapseButton.Pressed += this.HandleCollapseButtonClicked;
            this.uxCollapseButton.Text = "-";

            this.uxMakeBiggerButton = new ButtonControl();
            this.uxMakeBiggerButton.Pressed += this.HandleMakeBiggerButtonClicked;
            this.uxMakeBiggerButton.Text = "+";

            this.uxDragButton = new OptionControl();
            this.uxDragButton.Changed += this.HandleDragButtonClicked;

            //
            // UnitStatsPanel
            //            

            Children.Add(this.uxCollapseButton);
            Children.Add(this.uxMakeBiggerButton);
            Children.Add(this.uxDragButton);
        }

        #endregion // NOT Component Designer generated code                      

        protected ButtonControl uxCollapseButton;
        protected ButtonControl uxMakeBiggerButton;
        protected OptionControl uxDragButton;        
        
    }
}
