using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DamageTracker
{
    /// <summary>
    /// Pet object
    /// </summary>
    public class Pet : IDisposable
    {
        public string name;
        public int damage;
        public Player owner;
        private bool disposed = false;

        public Pet(string _name, Player _owner)
        {
            name = _name;
            damage = 0;
            owner = _owner;
        }

        /// <summary>
        /// Commit an action that this pet does on behalf of it's owner
        /// </summary>
        /// <param name="time">Time action occured</param>
        /// <param name="who">The owner of the pet</param>
        /// <param name="amount">Amount of damage done by pet</param>
        /// <param name="target">Pet's target</param>
        /// <param name="skill">Skill the pet may have used</param>
        public void commit_action(string time, string who, int amount, string target, string skill)
        {
            Action a = new Action(time, owner, amount, target, skill,false);
            owner.details.Add(a);
            Console.WriteLine(a.ToString());
        }

        /// <summary>
        /// Generic pet
        /// </summary>
        ~Pet()
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
            //If the system hasn't already disposed
            if (!disposed)
            {
                //And we are disposing
                if (disposing)
                {
                    name = null;
                    owner = null;
                }
            }
            disposed = true;
        }
    }
}
