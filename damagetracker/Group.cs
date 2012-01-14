using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace DamageTracker
{
    /// <summary>
    /// Current group of players
    /// </summary>
    public class Group : IDisposable
    {
        public Hashtable members;
        private bool disposed = false;

        /// <summary>
        /// Create the group
        /// </summary>
        public Group()
        {
            members = new Hashtable();
            //Add yourself to the group
            members.Add(Properties.Resources.You, new Player(Properties.Resources.You));
        }

        /// <summary>
        /// Get player from the group if exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Player this[string name] 
        {
            get
            {
                if ((Player)members[name] != null)
                {
                    return (Player)members[name];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Wipe all data for all players in the group
        /// </summary>
        public void clear()
        {
            foreach (Player player in members.Values)
            {
                player.damage = 0;
                player.healing = 0;
                if (player.details != null)
                {
                    foreach (Action a in player.details)
                    {
                        a.Dispose();
                    }
                    player.details.Clear();
                }
            }
        }

        /// <summary>
        /// Reset the group
        /// </summary>
        public void reset()
        {
            List<Player> members_to_remove = new List<Player>();

            // we have to keep a second list as members_to_deleted list, as deleting items while traversing collection is not possible
            foreach (Player player in members.Values) 
            {
                if (player.name != Properties.Resources.You) members_to_remove.Add(player);
            }
            foreach (Player player in members_to_remove)
            {
                player.Dispose();
                members.Remove(player.name);
            }
        }

        /// <summary>
        /// Create new player when somebody joins
        /// </summary>
        /// <param name="name"></param>
        public void join(String name)
        {
            try
            {
                Player p = new Player(name);
                members.Add(name, p);
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// Remove player when kicked or leaves
        /// </summary>
        /// <param name="name"></param>
        public void leave(String name)
        {
            if (name != Properties.Resources.You)
            {
                try
                {
                    members.Remove(name);
                }
                catch (Exception e) { }
            }
            else
                reset();
        }

        /// <summary>
        /// Generic group
        /// </summary>
        ~Group()
        {
            Dispose(false);
        }

        /// <summary>
        /// Public dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //The GC Shouldn't try to finalize the object as we are disposing it
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
                    //If there are members still in the group
                    if (members != null)
                    {
                        //Dispose all members
                        foreach (Player p in members.Values)
                        {
                            p.Dispose();
                        }
                        members.Clear();
                        members = null;
                    }
                }
            }
            disposed = true;
        }
    }
}
