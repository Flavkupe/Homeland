using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TestUtility.Simulation;
using System.Threading;
using TacticsGame;

namespace TestUtility.Tabs
{
    public partial class SimulationTab : UserControl
    {
        private Thread simThread = null;
        private SimulationScene sim;

        public SimulationTab()
        {            
            InitializeComponent();            

            this.ResetScene();            
        }

        private void ResetScene()
        {
            this.uxNextTurn.Enabled = false;
            this.uxStartSimulation.Enabled = true;
            this.uxFeed.Items.Clear();
            sim = new BasicSimulationScene();
            sim.SendMessage += this.HandleSendMessage;
        }        

        public void HandleSendMessage(object sender, EventArgsEx<string> e)
        {
            this.AddToFeed(e.Value);
        }

        private void AddToFeed(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action<string>)AddToFeed, message);
            }
            else
            {
                lock(this)
                {
                    this.uxFeed.Items.Add(message);

                    if (this.uxFeed.Items.Count > 300)
                    {
                        this.uxFeed.Items.RemoveAt(0);
                    }
                }
            }
        }

        protected virtual void TurnDone()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)TurnDone);
            }
            else
            {
                this.uxNextTurn.Enabled = true;
            }
        }

        private void uxClearFeed_Click(object sender, EventArgs e)
        {
            this.uxFeed.Items.Clear();
        }
        
        private void uxStartSimulation_Click(object sender, EventArgs e)
        {
            this.uxStartSimulation.Enabled = false;

            sim.SetupScene();
            simThread = new Thread(Run);
            simThread.Start();                
        }

        protected virtual void Run()
        {
            sim.NewTurn();
            while (sim.RunStep())
            {}
            this.TurnDone();
        }

        private void uxNextTurn_Click(object sender, EventArgs e)
        {
            this.uxNextTurn.Enabled = false;
            simThread = new Thread(Run);
            simThread.Start();   
        }

        private void uxResetClick(object sender, EventArgs e)
        {
            this.ResetScene();
        }        
    }
}
