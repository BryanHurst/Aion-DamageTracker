using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace DamageTracker
{
    /// <summary>
    /// Awesome log parser for Aion Online which will calculate you and your party's dps
    /// and show you all the info in cool graphs!
    /// </summary>
    static class Program
    {
        //Main window to show
        public static MeterWindow main_window;

        /// <summary>
        /// Main method
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();

            //If Aion path is not configured yet, do so
            if (!Config.game_path_exists()) 
            {
                //Prompt user
                MessageBox.Show("Please set the path for your AION installation.", "Need AION path!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                //Setup Config window to get path from user
                ConfigWindow cfg = new ConfigWindow();
                cfg.ShowDialog();
            }

            //Setup the main window to display
            main_window = new MeterWindow();
            Application.Run(main_window);
        }
    }
}
