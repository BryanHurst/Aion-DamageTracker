using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Resources;
using System.Collections;

namespace DamageTracker
{
    /// <summary>
    /// Get or store config file settings
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Return the path to the game's log
        /// </summary>
        /// <returns></returns>
        public static string get_game_log_path() 
        {
            return Properties.Settings.Default.AION_Path + "\\Chat.log";
        }

        /// <summary>
        /// Returns the path to the game
        /// </summary>
        /// <returns></returns>
        public static string get_game_path() 
        {
            return Properties.Settings.Default.AION_Path;
        }

        /// <summary>
        /// Sets the path to the game
        /// </summary>
        /// <param name="path"></param>
        public static void set_game_path(string path)
        {
            Properties.Settings.Default.AION_Path = path;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Check if the game path is correct.
        /// Do we find the aion.bin file?
        /// Do we find the chat log?
        /// </summary>
        /// <returns></returns>
        public static Boolean game_path_exists()
        {
            string path = get_game_path();
            string aion_exe = path + "/bin32/aion.bin";
            string chat_log = path + "/Chat.log";

            //Check that game path exists and aion.bin exists (correct directory)
            //If not valid or not entered yet
            if (!Directory.Exists(path) || !File.Exists(aion_exe))
            {
                // try to read aion path from registry setting
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\NCSoft\\Aion");

                if (key == null)
                {
                    return false;
                }
                else
                {
                    object install_path = key.GetValue("InstallPath");
                    //If we found a valid install path for Aion
                    if (install_path != null)
                    {
                        set_game_path(install_path.ToString());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {   
                //check Chat.log, if it's not there create it
                if (!File.Exists(chat_log))
                {
                    StreamWriter w = new StreamWriter(chat_log);
                    w.Write("");
                    w.Close();
                }
                //TODO wipe log file on each startup
            }
            return true;
        }

        /// <summary>
        /// Create the system.ovr file in the Aion directory
        /// This file is what tells the game to log stuff
        /// </summary>
        public static void create_aion_config_file()
        {
            string ovr_file = get_game_path() + "/system.ovr";

            StreamWriter sw = new StreamWriter(ovr_file, true);

            sw.WriteLine("g_chatlog = \"1\"");
            sw.WriteLine("log_IncludeTime = \"1\"");
            sw.WriteLine("log_Verbosity = \"1\"");
            sw.WriteLine("log_FileVerbosity = \"1\"");
            sw.Close();
        }
    }
}
