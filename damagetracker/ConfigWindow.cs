using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace DamageTracker
{
    /// <summary>
    /// Window to prompt for Aion install path
    /// </summary>
    public partial class ConfigWindow : Form
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            try
            {
                txt_aion_path.Text = Config.get_game_path();
            }
            catch (Exception exc)
            { 
            }
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            Config.set_game_path(txt_aion_path.Text);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            cmdApply_Click(this, e);
            if (!Config.game_path_exists())
                MessageBox.Show("AION path is not correcty set. Please choose your AION install path", "AION path needed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Config.create_aion_config_file();
                this.Close();
            }
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selected_folder = folderBrowserDialog.SelectedPath;
                txt_aion_path.Text = selected_folder;
            }
        }
    }
}
