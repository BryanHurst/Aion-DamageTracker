using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DamageTracker
{
    /// <summary>
    /// Regex filter for the chat log entries
    /// </summary>
    public class Filter : IDisposable
    {
        public Regex regex;

        public delegate void delegate_callback(GroupCollection matches);
        //The callback function for a match
        public delegate_callback callback; 
        //Is it a combat entry?
        public Boolean combat_filter;
 
        private bool disposed = false;

        /// <summary>
        /// Sift through all general entries
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="_callback"></param>
        public Filter(string pattern, delegate_callback _callback)
        {
            //Compile the regex for faster matches
            regex = new Regex(pattern, RegexOptions.Compiled);
            callback = _callback;
            combat_filter = false;
        }

        /// <summary>
        /// Mark as combat entry and continue sifting
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="_callback"></param>
        /// <param name="_combat_filter"></param>
        public Filter(string pattern, delegate_callback _callback, Boolean _combat_filter)
        {
            //Compile the regex for faster matches
            regex = new Regex(pattern, RegexOptions.Compiled);
            callback = _callback;
            combat_filter = _combat_filter;
        }

        /// <summary>
        /// Decompile the current log entry
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Boolean Run(string line)
        {
            Match m;

            //If we have a match
            if ((m = regex.Match(line)).Success)
            {
                //Call the callback function with match groups
                callback(m.Groups);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Basic filter
        /// </summary>
        ~Filter()
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
                    //Kill callback delegates and regex
                    callback = null;
                    regex = null;
                }
            }
            disposed = true;
        }
    }
}
