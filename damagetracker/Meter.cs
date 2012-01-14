using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DamageTracker
{
    /// <summary>
    /// Damage Tracker
    /// </summary>
    public class Meter : IDisposable
    {       
        //Current rendering mode for meter
        public enum RENDER_MODE 
        {
            render_damage,
            render_healing
        }

        //The active meter
        public static Meter active_meter = null; 

        public RENDER_MODE render_mode; 
        public Thread thread; 
        public Parser parser; 
        public Group group; 
        public LogReader log_tracker; 
        public OverTimeEffectTracker effect_tracker; 
        public PetTracker pet_tracker; 
        private bool disposed = false;

        #region General Functions

        public Meter()
        {
            //If skill db not loaded yet, do so
            if (!Skills.loaded)
            {
                Skills.init();
            }

            //If there is already a meter when creating a new one,
            //get rid of it
            if (Meter.active_meter != null)
            { 
                Meter.active_meter.Dispose();
            }

            //Make this new meter the active meter
            Meter.active_meter = this;           

            //Run the meter on a seperate thread than the GUI
            thread = new Thread(run);

            //Start new thread for this meter
            thread.Start();
        }

        /// <summary>
        /// Make the meter go
        /// </summary>
        /// <param name="_mode"></param>
        private void run()
        {
            parser = new Parser(this);
            //The players group
            group = new Group(); 
            //The DOT and HOT tracker
            effect_tracker = new OverTimeEffectTracker(); 
            //The Pet tracker
            pet_tracker = new PetTracker();            

            //Init the log reader and file watcher
            log_tracker = new LogReader(); 
        }

        /// <summary>
        /// Generic meter
        /// </summary>
        ~Meter()
        {
            Dispose(false);           
        }

        /// <summary>
        /// Public dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //The GC shouldn't try to finalize the object as we are disposing of it
            GC.SuppressFinalize(this); 
        }

        /// <summary>
        /// Internal dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            //If system hasn't disposed already
            if (!disposed)
            {
                //And we are disposing
                if (disposing)
                {
                    //Kill running thread, parser, group, effect tracker, pet tracker, and log tracker
                    if (thread != null)
                    {
                        thread.Abort();
                        thread = null;
                    }
                    if (parser != null)
                    {
                        parser.Dispose();
                        parser = null;
                    }
                    if (group != null)
                    {
                        group.Dispose();
                        group = null;
                    }
                    if (effect_tracker != null)
                    {
                        effect_tracker.Dispose();
                        effect_tracker = null;
                    }
                    if (pet_tracker != null)
                    {
                        pet_tracker.Dispose();
                        pet_tracker = null;
                    }
                    if (log_tracker != null)
                    {
                        log_tracker.Dispose();
                        log_tracker = null;
                    }
                    Meter.active_meter = null;
                    GC.Collect();
                }
            }
            disposed = true;
        }

        #endregion

        #region Meter Functions
        /// <summary>
        /// Submit action to meter
        /// </summary>
        /// <param name="time">time of the action</param>
        /// <param name="who">Who did the action</param>
        /// <param name="amount">How much the action was for (damage)</param>
        /// <param name="target">Who the action was to</param>
        /// <param name="skill">What skill the action was</param>
        /// <param name="critical">If the in-game action was critted</param>
        private void commit_action(string time, string who, int amount, string target, string skill,bool critical)
        {
            //Check if doer is in party
            if (group[who] != null)
            {
                //Make a new combat action for the group member
                Action a = new Action(time, group[who], amount, target, skill, critical);
                //Add this action to the player that did it
                group[who].details.Add(a);
                Console.WriteLine(a.ToString());
            }
            else
            { 
                // a pet may also inflict the damage, so let PetTracker to check it
                pet_tracker.commit_pet_action(time, who, amount, target, skill);
            }
        }

        /// <summary>
        /// Reset the meter stats and the group
        /// </summary>
        public void reset_meter() 
        {
            //Kill the group
            group.reset();

            try
            {
                //Clear the stats for all players
                group[Properties.Resources.You].Clear(); 
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// Calculate the statistics for damage and shit
        /// </summary>
        public void calculate_statistics()
        {
            //Total party damage/healing for percent calculations
            int party_total = 0; 

            if (group != null && group.members.Count > 0)
            {
                foreach (Player player in group.members.Values)
                {
                    switch (render_mode)
                    {
                        case RENDER_MODE.render_damage:
                            party_total += player.damage;
                            break;
                        case RENDER_MODE.render_healing:
                            party_total += player.healing;
                            break;
                    }
                }
                try
                {
                    //Time to calculate stats for everyone
                    foreach (Player player in group.members.Values)
                    {                        
                        TimeSpan t = new TimeSpan(0);

                        if (player.details.Count > 0)
                        {
                            t = DateTime.Now - player.details[0].time;
                        }

                        // DPS & HPS
                        player.DPS = (double)(player.damage / t.TotalSeconds);
                        if (double.IsNaN(player.DPS))
                        {
                            player.DPS = 0;
                        }
                        player.HPS = (double)(player.healing / t.TotalSeconds);
                        if (double.IsNaN(player.HPS))
                        {
                            player.HPS = 0;
                        }

                        // Burst values
                        int i = 0;
                        int burst_dmg_total=0;
                        int burst_healing_total=0;
                        for (i = player.details.Count-1; i >= 0; i--)
                        {
                            double span = (DateTime.Now - player.details[i].time).TotalSeconds;
                            if (span > 0 && span < 5)
                            {
                                if (player.details[i].skill.sub_type == SUB_TYPES.HEAL)
                                {
                                    burst_healing_total += player.details[i].healing;
                                }
                                else
                                {
                                    burst_dmg_total += player.details[i].damage;
                                }
                            }
                            else if (span > 5)
                            {
                                break;
                            }
                        }
                        player.burst_DPS = burst_dmg_total / 5;
                        player.burst_HPS = burst_healing_total / 5;

                        // percentage
                        switch (render_mode)
                        {
                            case RENDER_MODE.render_damage:
                                player.percent = (double)(player.damage * 100) / party_total;
                                break;
                            case RENDER_MODE.render_healing:
                                player.percent = (double)(player.healing * 100) / party_total;
                                break;
                        }
                        if (double.IsNaN(player.percent))
                        {
                            player.percent = 0; 
                        }
                    }
                }
                //Ignore division by zero
                catch (Exception e) 
                { 
                } 
            }
        }
        #endregion

        #region Parser Engine Callbacks
        /// <summary>
        /// Method to get the sorted list of players vs their damage output
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, double>> get_sorted_list()
        {
            List<KeyValuePair<string, double>> sorted_list = new List<KeyValuePair<string, double>>();

            //Go through all the players in the unsorted list and transfer to "sorted list"
            foreach (Player player in group.members.Values)
            {
                sorted_list.Add(new KeyValuePair<string, double>(player.name, player.percent));
            }

            //Actually sort the "sorted list"
            sorted_list.Sort(
                delegate(KeyValuePair<string, double> v1, KeyValuePair<string, double> v2)
                {
                    return -Comparer<double>.Default.Compare(v1.Value, v2.Value);
                }
            );

            return sorted_list;
        }

        /// <summary>
        /// Setup direct damage action to commit
        /// </summary>
        /// <param name="matches"></param>
        public void commit_damage(GroupCollection matches) 
        {
            string time = matches["time"].Value.Trim();
            string who = matches["who"].Value.Trim();
            // AION puts speration character between each 3 digits, but it's sometimes not only a '.' or ','. So we're replacing any characters thats not a digit. 
            int amount = int.Parse(Regex.Replace(matches["amount"].Value,@"[\D]",""));                                         
            bool critical = false;
            if (matches["critical"].Value.ToString() == "Critical Hit!")
            {
                critical = true;
            }
            string target = matches["target"].Value.Trim();
            string skill = matches["skill"].Value.Trim();
            //Fix the ending of the string...
            skill = skill.Replace('.', ' ').Trim();
            if (skill.Trim().Length == 0)
            {
                //If no spell/skill is given, then it is an auto-attack "swing"
                skill = "Swing";
            }

            //Have player commit the created direct damage action
            commit_action(time, who, amount, target, skill,critical);
        }

        /// <summary>
        /// Setup damage over time action to commit
        /// </summary>
        /// <param name="matches"></param>
        public void commit_dot_effect(GroupCollection matches) 
        {
            string time = matches["time"].Value.Trim();
            string target = matches["target"].Value.Trim();
            int amount = int.Parse(Regex.Replace(matches["amount"].Value, @"[\D]", ""));
            string skill = matches["skill"].Value.Trim();
            //Fix ending of the string...
            skill = skill.Replace('.', ' ').Trim();

            //Commit this dot tick
            Meter.active_meter.effect_tracker.apply_effect(time, target, amount, skill);
        }

        /// <summary>
        /// Setup direct healing action to commit
        /// </summary>
        /// <param name="matches"></param>
        public void commit_healing(GroupCollection matches) 
        {
            string time = matches["time"].Value.Trim();
            string who = matches["who"].Value.Trim();
            int amount = int.Parse(Regex.Replace(matches["amount"].Value, @"[\D]", ""));
            bool critical = false;
            string target;
            if (matches["target"].Success)
            {
                // check if there was a healing target
                target = matches["target"].Value.Trim();
            }
            else
            { 
                // if not it's a self-healing
                target = who;
            }
            string skill = matches["skill"].Value.Trim();
            //Fix the end of the string...
            skill = skill.Replace('.', ' ').Trim(); 
            commit_action(time, who, amount, target, skill,critical);
        }

        /// <summary>
        /// Setup heal over time action to commit
        /// </summary>
        /// <param name="matches"></param>
        public void commit_hot_effect(GroupCollection matches) 
        {
            string time = matches["time"].Value.Trim();
            string target = matches["target"].Value.Trim();
            bool critical = false;
            string who = "";
            if (matches["who"].Success)
            {
                who = matches["who"].Value.Trim();
            }
            int amount;
            if (matches["amount"].Success)
            {
                amount = int.Parse(Regex.Replace(matches["amount"].Value, @"[\D]", ""));
            }
            else
            {
                amount = 0;
            }
            string skill = matches["skill"].Value.Trim();
            //Fix end of the string...
            skill = skill.Replace('.', ' ').Trim();

            if (who != "")
            { 
                // if who is missing that means with this line a HOT effect starts
                commit_action(time, who, amount, target, skill, critical);
            }
            else
            {
                //Commit this HOT tick
                Meter.active_meter.effect_tracker.apply_effect(time, target, amount, skill);
            }
        }

        /// <summary>
        /// Set a player's pet when summoned
        /// </summary>
        /// <param name="matches"></param>
        public void summon_pet(GroupCollection matches) 
        {
            string who = matches["who"].Value.Trim();
            if (group[who] != null)
            {
                string time = matches["time"].Value.Trim();
                string target = matches["target"].Value.Trim();
                string skill = matches["skill"].Value.Trim();
                skill = skill.Replace('.', ' ').Trim();            
                group[who].summon_pet(time, target, skill);
            }
        }

        /// <summary>
        /// Disband the group
        /// </summary>
        /// <param name="matches"></param>
        public void group_disbanded(GroupCollection matches)
        {
            group.reset();
        }

        /// <summary>
        /// Group member joined the group
        /// </summary>
        /// <param name="matches"></param>
        public void group_message(GroupCollection matches)
        {
            string who = matches["who"].Value.Trim();
            string message = matches["message"].Value.Trim();
            group.join(matches["who"].Value.Trim());
        }

        /// <summary>
        /// You joined the group
        /// </summary>
        /// <param name="matches"></param>
        public void self_message(GroupCollection matches)
        {
            string self = matches["who"].Value.Replace(":","").Trim();
            string message = matches["message"].Value.Trim();
        }

        /// <summary>
        /// Somebody left the group
        /// </summary>
        /// <param name="matches"></param>
        public void member_left(GroupCollection matches)
        {
            group.leave(matches["who"].Value.Trim());
        }
        #endregion
    }
}
