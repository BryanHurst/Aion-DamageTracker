using System;
using System.Collections.Generic;
using System.Text;

namespace DamageTracker
{
    /// <summary>
    /// PetTracker to track all pet actions
    /// </summary>
    public class PetTracker : IDisposable
    {
        public List<Pet> active_pets;
        private bool disposed = false;

        public PetTracker()
        {
            if (active_pets == null)
            {
                // init the structure
                active_pets = new List<Pet>();
            }
            else
            {
                // reset if already inited so we can start anew
                active_pets.Clear(); 
            }
        }

        /// <summary>
        /// Add a new pet to be tracked
        /// </summary>
        /// <param name="p"></param>
        public void track(Pet p)
        {
            active_pets.Add(p);
        }

        /// <summary>
        /// Remove a pet from the tracker
        /// Players may summon new pets, so they must be removed quickly
        /// </summary>
        /// <param name="p"></param>
        public void remove(Pet p)
        {
            active_pets.Remove(p);
        }

        /// <summary>
        /// Setup action for the pet to commit on behalf of the owner
        /// </summary>
        /// <param name="time">Time of action</param>
        /// <param name="who">Owner of the pet</param>
        /// <param name="amount">Amount of damage pet did</param>
        /// <param name="target">Pet's target</param>
        /// <param name="skill">Skill pet used</param>
        public void commit_pet_action(string time, string who, int amount, string target, string skill)
        {
            if (active_pets != null && active_pets.Count > 0)
            {
                foreach (Pet pet in active_pets)
                {
                    if (pet.name == who)
                    {
                        pet.commit_action(time, who, amount, target, skill);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Generic pet
        /// </summary>
        ~PetTracker()
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
                    if (active_pets != null)
                    {
                        foreach (Pet p in active_pets)
                        {
                            p.Dispose();
                        }
                        active_pets.Clear();
                        active_pets = null;
                    }
                }
            }
            disposed = true;
        }
    }
}
