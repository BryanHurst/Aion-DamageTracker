using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DamageTracker
{
    /// <summary>
    /// Continuously watch specified file (chatlog)
    /// </summary>
    class FileWatcher : IDisposable
    {
        //Watcher for changes in file
        private FileSystemWatcher watcher;

        private bool disposed = false;

        /// <summary>
        /// Create watcher for the specified file and monitor changes
        /// </summary>
        /// <param name="file_changed_func"></param>
        public FileWatcher(FileSystemEventHandler file_changed_func)
        {
            watcher = new FileSystemWatcher(); 
            watcher.Path = Config.get_game_path(); 
            //Always check the last write to the file
            watcher.NotifyFilter = NotifyFilters.LastWrite; 
            watcher.Filter = "Chat.log";
            //Callback
            watcher.Changed += new FileSystemEventHandler(file_changed_func);
            //Start watching
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 
        /// </summary>
        ~FileWatcher()
        {
            Dispose(false);
        }

        /// <summary>
        /// Public dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //The GC shouldn't try to finalize the object as we are disposing it
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            //If system hasn't already disposed
            if (!disposed)
            {
                //And we are disposing
                if (disposing)
                {
                    //Kill the watcher
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    watcher = null;
                }
            }
            disposed = true;
        }
    }
}
