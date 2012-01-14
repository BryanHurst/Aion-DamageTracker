using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DamageTracker
{
    /// <summary>
    /// Regex base line parser for the game log file
    /// </summary>
    public class Parser : IDisposable
    {
        public enum MODE
        {
            NORMAL,
            IGNORE_COMBAT
        }
        // the list of regex patterns
        public List<Filter> filters; 
        public MODE mode = MODE.NORMAL;
        private bool disposed = false;

        public Parser(Meter meter)
        {
            // Add the regex patterns and the callback functions for a succesfull match
            filters = new List<Filter>();

            // damage filters
            filters.Add(new Filter(Properties.Resources.FILTER_DIRECT_DAMAGE_SKILL, new Filter.delegate_callback(meter.commit_damage), true));
            filters.Add(new Filter(Properties.Resources.FILTER_DIRECT_DAMAGE, new Filter.delegate_callback(meter.commit_damage), true));
            // damage overtime-effects filters
            filters.Add(new Filter(Properties.Resources.FILTER_DOT, new Filter.delegate_callback(meter.commit_dot_effect), true));
            // healing filters
            filters.Add(new Filter(Properties.Resources.FILTER_DIRECT_HEALING_SKILL, new Filter.delegate_callback(meter.commit_healing), true));
            filters.Add(new Filter(Properties.Resources.FILTER_DIRECT_HEALING, new Filter.delegate_callback(meter.commit_healing), true));
            // healing over-time effects
            filters.Add(new Filter(Properties.Resources.FILTER_HOT_1, new Filter.delegate_callback(meter.commit_hot_effect), true));
            filters.Add(new Filter(Properties.Resources.FILTER_HOT_2, new Filter.delegate_callback(meter.commit_hot_effect), true));
            // pet summons
            filters.Add(new Filter(Properties.Resources.FILTER_SUMMON_1, new Filter.delegate_callback(meter.summon_pet), true));
            filters.Add(new Filter(Properties.Resources.FILTER_SUMMON_2, new Filter.delegate_callback(meter.summon_pet), true));

            // Group-awareness filters
            filters.Add(new Filter(Properties.Resources.FILTER_PLAYER_JOIN_GROUP, new Filter.delegate_callback(meter.group_message)));
            filters.Add(new Filter(Properties.Resources.FILTER_PLAYER_LEFT_GROUP, new Filter.delegate_callback(meter.member_left)));
            filters.Add(new Filter(Properties.Resources.FILTER_PLAYER_KICKED, new Filter.delegate_callback(meter.member_left)));
            filters.Add(new Filter(Properties.Resources.FILTER_GROUP_DISBAND, new Filter.delegate_callback(meter.group_disbanded)));
            filters.Add(new Filter(Properties.Resources.FILTER_GROUP_MESSAGE, new Filter.delegate_callback(meter.group_message)));
            filters.Add(new Filter(Properties.Resources.FILTER_SELF_MESSAGE, new Filter.delegate_callback(meter.self_message)));
        }

        /// <summary>
        /// Take next line in log and search for a match
        /// </summary>
        /// <param name="line"></param>
        public void parse_line(string line)
        {
            // filter out any empty lines
            if (line.Trim().Length > 0) 
            {
                bool ignore_combat = false;
                if (mode == MODE.IGNORE_COMBAT)
                {
                    ignore_combat = true;
                }

                // try filters on the line
                foreach (Filter f in filters) 
                {
                    // ingore_combat may be state dropping of combat message lines.
                    if (!ignore_combat || (ignore_combat && !f.combat_filter)) 
                    {
                        // until a match
                        if (f.Run(line))
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generic parser
        /// </summary>
        ~Parser()
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
                    //Kill all the filters!
                    if (filters != null)
                    {
                        foreach (Filter f in filters)
                        {
                            f.Dispose();
                        }
                        filters.Clear();
                        filters = null;
                    }
                }
            }
            disposed = true;
        }
    }
}
