using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DamageTracker
{
    /// <summary>
    /// Window to display indepth combat details
    /// Kinda like an Excel Spreadsheet
    /// </summary>
    public partial class DetailsWindow : Form
    {
        public DetailsWindow()
        {
            InitializeComponent();
            load_players();
        }

        /// <summary>
        /// Create list of all players in the current group
        /// </summary>
        private void load_players()
        {
            foreach (Player player in Meter.active_meter.group.members.Values)
            {
                combobox_players.Items.Add(player.name);
            }
            if (combobox_players.Items.Count > 0)
            {
                combobox_players.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Load the combat data for the currently selected player
        /// </summary>
        private void load_meter_data()
        {
            //Clear all current items in the list
            listView.Items.Clear();

            //Grab the current player in focus
            string name = combobox_players.Items[combobox_players.SelectedIndex].ToString();
            Player player = (Player)Meter.active_meter.group[name];

            //Go through all the current players actions
            foreach (Action action in player.details)
            {
                ListViewItem i = new ListViewItem(action.time.ToString());
                i.SubItems.Add(action.skill.name);
                i.SubItems.Add(action.who.name);
                i.SubItems.Add(action.target);

                string amount = "";
                switch (action.skill.sub_type)
                {
                    case SUB_TYPES.ATTACK:
                    case SUB_TYPES.DEBUFF:
                        amount += action.damage.ToString();
                        if (action.critical) amount += " CRITICAL!";
                        break;
                    case SUB_TYPES.HEAL:
                    case SUB_TYPES.BUFF:
                        amount += action.healing.ToString();
                        if (action.critical) amount += " CRITICAL!";
                        break;
                }
                i.SubItems.Add(amount);

                if (action.ticks != null)
                {
                    string ticks = "";
                    foreach (int tick in action.ticks)
                    {
                        ticks += tick + ", ";
                    }
                    i.SubItems.Add(ticks);
                }
                listView.Items.Add(i);
            }

            //Resize columns depending on what is being displayed
            if (listView.Items.Count > 0)
            {
                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
            {
                listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        /// <summary>
        /// Change the current player we are looking at
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void combobox_players_SelectedIndexChanged(object sender, EventArgs e)
        {
            load_meter_data();
        }
    }
}
