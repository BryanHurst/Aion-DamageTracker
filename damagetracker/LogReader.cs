using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DamageTracker
{
    /// <summary>
    /// Class to read in each new line of the Aion chat log
    /// </summary>
    public class LogReader : IDisposable 
    {
        // the file reader
        private StreamReader reader;
        // the file change watcher
        private FileWatcher watcher; 
        private bool disposed = false;

        public LogReader()
        {
            string file = Config.get_game_path() + "\\Chat.log";

            try
            {
                //Open the file
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); 
                reader = new StreamReader(fs, System.Text.Encoding.Default);
                // set position to end
                reader.ReadToEnd();
                // start the filewatcher
                watcher = new FileWatcher(file_changed); 
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("The log file " + file + " can not be found! Please set your AION path", "Log file error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// When notified of change in file, read the changes in
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void file_changed(object source, FileSystemEventArgs e) 
        {
            // read the latest changes
            read_changes(); 
        }

        /// <summary>
        /// Read the latest changes in the chat log
        /// </summary>
        private void read_changes() 
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                // temprorary fix for character \u2019 which rendered as \uFFFD (non-printable character)
                line = line.Replace('\uFFFD', '\'');
                // let the parse engine check the line
                Meter.active_meter.parser.parse_line(line); 
            }
        }

        /// <summary>
        /// Generic log reader
        /// </summary>
        ~LogReader()
        {
            Dispose(false);
        }

        /// <summary>
        /// Public dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        /// <summary>
        /// Internal dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
                    watcher.Dispose();
                }
            }
            disposed = true;
        }
    }
}
