using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nuclex.UserInterface;
using TacticsGame.UI;
using Nuclex.UserInterface.Visuals.Flat;
using TacticsGame.UI.Controls;
using TacticsGame.UI.Dialogs;
using Nuclex.UserInterface.Controls;
using TacticsGame.UI.Panels;
using TacticsGame.GameObjects.Units;
using TacticsGame.Items;
using TacticsGame.Utility.Classes;

namespace TacticsGame
{
    public class InterfaceManager : Singleton<InterfaceManager>
    {
        private InterfaceManager()
        {
        }

        GuiManager gui;

        public event EventHandler DialogClosed;

        private int currentZOrder = 0;        

        public GuiManager Gui
        {
            get { return this.gui; }
            set { this.gui = value; }
        }

        /// <summary>
        /// Increments and returns the next best Z-order for modality.
        /// </summary>
        /// <returns></returns>
        public int GetNextZOrder(bool setGlobalZ = true)
        {
            currentZOrder++;

            if (setGlobalZ)
            {
                Control.ModalZ = currentZOrder;
            }

            return currentZOrder;
        }

        /// <summary>
        /// Decrements the Z order, such as if a modal dialog was closed.
        /// </summary>
        /// <returns></returns>
        public void RevertZOrder(bool setGlobalZ = true)
        {
            --currentZOrder;

            if (setGlobalZ)
            {
                Control.ModalZ = currentZOrder;
            }            
        }

        public TileSelectionDialog TileSelectionDialog { get; set; }

        public CommandPane CommandPane { get; set; }

        public ActionFeedPanel ActionFeedPane { get; set; }

        public void MakeControlVisible(Control control, bool visible)
        {
            if (visible && !this.gui.Screen.Desktop.Children.Contains(control))
            {
                this.gui.Screen.Desktop.Children.Add(control);
                control.BringToFront();
                
                if (control is ICanBeClosed) 
                {
                    ((ICanBeClosed)control).CloseClicked += this.HandleControlCloseClicked;
                }
            }
            else if (!visible && this.gui.Screen.Desktop.Children.Contains(control))
            {
                this.gui.Screen.Desktop.Children.Remove(control);
            }
        }

        public void AddAdditionalControls()
        {            
            FlatGuiVisualizer visualizer = (FlatGuiVisualizer)this.gui.Visualizer;                
            visualizer.RendererRepository.AddAssembly(typeof(TooltipButtonControl).Assembly);
        }
        
        public void CreateDialogsAndPanes()
        {
            CommandPane commandPane = new CommandPane();
            this.CommandPane = commandPane;        

            int width = GameStateManager.Instance.CameraView.Width - this.CommandPane.Width;

            UniRectangle actionFeedRegular = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(1.0f, -100f), new UniScalar(0.0f, width), new UniScalar(0.0f, 100.0f));
            UniRectangle actionFeedCollapsed = new UniRectangle(new UniScalar(0.0f, 0.0f), new UniScalar(1.0f, -20f), new UniScalar(0.0f, width), new UniScalar(0.0f, 20.0f));
            this.ActionFeedPane = new ActionFeedPanel(actionFeedRegular, actionFeedCollapsed);            
        }

        private void HandleControlCloseClicked(object sender, EventArgs e) 
        {
            if (sender is Control)
            {                
                this.MakeControlVisible(sender as Control, false);

                if (this.DialogClosed != null)
                {
                    this.DialogClosed(sender, e);
                }
            }
        }
    }
}
