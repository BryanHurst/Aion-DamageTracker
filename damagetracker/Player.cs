using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DamageTracker
{
    /// <summary>
    /// Player object
    /// </summary>
    public class Player : IDisposable
    {
        public string name;
        public int damage = 0;
        public int healing = 0;
        public int peak_damage = 0;
        public int peak_healing = 0;
        public double DPS = 0;
        public double burst_DPS = 0;
        public double HPS = 0;
        public double burst_HPS = 0;
        public List<Action> details;
        public List<Pet> pets;
        public double percent = 0;
        public System.Drawing.Color color;
        private bool disposed = false;

        /// <summary>
        /// Setup player in display and setup their actions to track
        /// </summary>
        /// <param name="name"></param>
        public Player(string name)
        {
            this.name = name;
            details = new List<Action>();
            if (name == Properties.Resources.You)
            {
                color = System.Drawing.ColorTranslator.FromHtml("#8F1327");
            }
            else
            {
                color = System.Drawing.ColorTranslator.FromHtml("#425B8C");
            }
        }

        /// <summary>
        /// Add a pet to this player
        /// </summary>
        /// <param name="time"></param>
        /// <param name="target"></param>
        /// <param name="skill"></param>
        public void summon_pet(string time, string target, string skill)
        {
            // if the player summons first pet, prepare the structure
            if (pets == null)
            {
                pets = new List<Pet>();
            }
            //if players has already summoned pet, first remove the last pet from Pet Tracker
            if (pets.Count > 0)
            {
                Meter.active_meter.pet_tracker.remove((Pet)pets[pets.Count - 1]); 
            }

            // new pet
            Pet p = new Pet(target, this);
            // add to players pet list
            pets.Add(p);
            // track the pet
            Meter.active_meter.pet_tracker.track(p); 

            //Log that player summoned a pet
            Action a = new Action(time, this, 0, target, skill,false); 
            details.Add(a);
            Console.WriteLine(a.ToString());
        }

        /// <summary>
        /// Reset player stats
        /// </summary>
        public void Clear()
        {
            details.Clear();
            damage = 0;
            percent = 0;
        }

        /// <summary>
        /// Generic player
        /// </summary>
        ~Player()
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
                    if (details != null)
                    {
                        foreach (Action a in details)
                        {
                            a.Dispose();
                        }
                        details.Clear();
                        details = null;
                    }
                    if (pets != null)
                    {
                        foreach (Pet p in pets)
                        {
                            p.Dispose();
                        }
                        pets.Clear();
                        pets = null;
                    }
                }
            }
            disposed = true;
        }
    }
}
