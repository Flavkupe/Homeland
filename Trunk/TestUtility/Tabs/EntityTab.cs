using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TacticsGame.GameObjects.Units;
using TacticsGame.GameObjects.Buildings;
using TacticsGame.GameObjects.Visitors;
using TacticsGame;
using TacticsGame.Managers;
using TacticsGame.Utility;
using TacticsGame.GameObjects;
using TacticsGame.GameObjects.EntityMetadata;
using TacticsGame.Items;
using TacticsGame.Metrics;

namespace TestUtility.Tabs
{
    public partial class EntityTab : UserControl
    {       
        public EntityTab()
        {
            InitializeComponent();

            this.uxTypeCombo.Items.AddRange(new object[] { new Display<Type>(typeof(DecisionMakingUnit), "DecisionMakingUnit"),
                                                           new Display<Type>(typeof(Building), "Building"),
                                                           new Display<Type>(typeof(Visitor), "Visitor") } );
            this.RefreshControlsEnabled();
        }

        private void uxTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO: cache these

            this.uxObjectCombo.Items.Clear();
            this.uxObjectCombo.SelectedItem = null;

            Type objectType = ((Display<Type>)this.uxTypeCombo.SelectedItem).Value;
            List<Type> types = new List<Type>(objectType.Assembly.GetTypes().Where(a => !a.IsAbstract && (objectType.IsAssignableFrom(a))));
            types.ForEach(a => this.uxObjectCombo.Items.Add(new Display<Type>(a, a.Name)));

            this.RefreshList();
        }

        private void uxGenerateButton_Click(object sender, EventArgs e)
        {
            if (this.uxObjectCombo.Items.Count > 0 && this.uxObjectCombo.SelectedItem != null)
            {
                Type objectType = ((Display<Type>)this.uxObjectCombo.SelectedItem).Value;

                if (typeof(GameObject).IsAssignableFrom(objectType)) 
                {
                    GameObject newObject = (GameObject)Activator.CreateInstance(objectType);
                    SimulationObjects.Instance.AddEntity(this.uxTypeCombo.Text, newObject);
                }

                this.RefreshList();
            }
        }

        private void RefreshList()
        {
            this.uxObjectList.Items.Clear();

            if (this.uxTypeCombo.SelectedItem == null) 
            {
                return;
            }
            
            List<GameObject> objects = SimulationObjects.Instance.GetEntities(this.uxTypeCombo.Text);
            foreach (GameObject obj in objects)
            {
                string name = (obj is Building && ((Building)obj).Owner != null) ? ((Building)obj).Owner.DisplayName : obj.DisplayName;               
                Display<GameObject> disp = new Display<GameObject>(obj, string.Format("{0} ({1})", name, obj.ObjectName)); 
                this.uxObjectList.Items.Add(disp);
            }                       
        }

        private void uxDeleteSelected_Click(object sender, EventArgs e)
        {
            if (this.uxObjectList.SelectedItem != null)
            {
                GameObject value = (this.uxObjectList.SelectedItem as Display<GameObject>).Value;
                SimulationObjects.Instance.RemoveEntity(this.uxTypeCombo.Text, value);
            }

            this.RefreshList();
        }

        private void uxSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.uxSaveBox.Text))
            {
                GameSerializer.Instance.Serialize(SimulationObjects.Instance.GameObjects, this.uxSaveBox.Text);
            }
        }

        private void uxRestore_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.uxSaveBox.Text))
            {
                SimulationObjects.Instance.GameObjects = (Dictionary<string, List<GameObject>>)GameSerializer.Instance.Deserialize(this.uxSaveBox.Text);
                SimulationObjects.Instance.LoadContents();
            }

            this.RefreshList();
        }

        private void uxRandom_Click(object sender, EventArgs e)
        {
            this.uxItem.Text = ItemGenerationUtilities.GetRandomAssortment(1)[0];
        }

        private void uxObjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshControlsEnabled();
            this.RefreshGoldLabel();
            this.RefreshItems();
            this.RefreshEquipment();
        }

        private void RefreshControlsEnabled()
        {
            this.uxInventoryTab.Enabled = false;
            if (this.uxObjectList.SelectedItem != null)
            {
                this.uxInventoryTab.Enabled = this.GetInventory() != null;
            }
        }

        private GameObject Selection { get { return this.uxObjectList.SelectedItem == null ? null : (this.uxObjectList.SelectedItem as Display<GameObject>).Value; } }

        private Inventory GetInventory()
        {
            if (this.uxObjectList.SelectedItem != null)
            {
                GameObject obj = this.Selection;
                if (obj is IHasInventory)
                {
                    return ((IHasInventory)obj).Inventory;
                }
            }

            return null;
        }

        private Equipment GetEquipment()
        {
            if (this.uxObjectList.SelectedItem != null)
            {
                GameObject obj = this.Selection;
                if (obj is DecisionMakingUnit)
                {
                    return ((DecisionMakingUnit)obj).Equipment;
                }
            }

            return null;
        }

        private void GiveMoneyClick(object sender, EventArgs e)
        {                                
            Inventory inventory = this.GetInventory();
            int money;
            if (int.TryParse(this.uxMoney.Text, out money))
            {
                inventory.Money += money;
                this.RefreshGoldLabel();
            }
        }

        private void uxGive_Click(object sender, EventArgs e)
        {
            try
            {
                Inventory inventory = GetInventory();
                if (inventory != null)
                {
                    inventory.AddItem(new Item(this.uxItem.Text));
                    this.RefreshItems();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshItems()
        {
            Inventory inventory = this.GetInventory();
            if (inventory != null)
            {
                this.uxItemBox.Items.Clear();
                inventory.Items.ToList<Item>().ForEach(a => this.uxItemBox.Items.Add(a.DisplayName));
            }
        }

        private void RefreshEquipment()
        {
            Equipment equipment = this.GetEquipment();
            if (equipment != null)
            {
                this.uxChestItem.Text = equipment[EquipmentSlot.Chest] == null ? string.Empty : equipment[EquipmentSlot.Chest].DisplayName;
                this.uxBootsItem.Text = equipment[EquipmentSlot.Feet] == null ? string.Empty : equipment[EquipmentSlot.Feet].DisplayName;
                this.uxLeftHandItem.Text = equipment[EquipmentSlot.LeftHand] == null ? string.Empty : equipment[EquipmentSlot.LeftHand].DisplayName;
                this.uxRightHandItem.Text = equipment[EquipmentSlot.RightHand] == null ? string.Empty : equipment[EquipmentSlot.RightHand].DisplayName;
                this.uxHelmItem.Text = equipment[EquipmentSlot.Helm] == null ? string.Empty : equipment[EquipmentSlot.Helm].DisplayName;
            }
        }

        private void RefreshGoldLabel()
        {
            Inventory inventory = this.GetInventory();
            if (inventory != null) 
            {
                this.uxGoldLabel.Text = string.Format("Gold: {0}", inventory.Money);
            }
        }

        private void uxReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You Sure?", "Resetting", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                SimulationObjects.Instance.GameObjects.Clear();
                MetricsLogger.Instance.ClearAll();
                this.uxObjectList.Items.Clear();
            }
        }
    }
}
