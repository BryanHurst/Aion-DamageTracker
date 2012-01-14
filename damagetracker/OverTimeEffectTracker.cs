using System;
using System.Collections.Generic;
using System.Text;

namespace DamageTracker
{
    /// <summary>
    /// Damage over time tracker
    /// </summary>
    public class OverTimeEffectTracker : IDisposable
    {
        public List<Action> list;
        private bool disposed = false;

        public OverTimeEffectTracker()
        {
            if (list == null)
            {
                // init the structure
                list = new List<Action>();
            }
            else
            {
                // reset if already inited
                list.Clear(); 
            }
        }

        /// <summary>
        /// Add a DOT to the list of DOT's to track
        /// </summary>
        /// <param name="a"></param>
        public void track(Action a)
        {
            list.Add(a);
        }

        /// <summary>
        /// Stop tracking all DOT's 
        /// </summary>
        public void stop()
        {
            //nuke the list of effects
            list.Clear();
        }

        /// <summary>
        /// Initial application of a DOT that we need to track
        /// </summary>
        /// <param name="_time">time of initiation</param>
        /// <param name="_target">target of the dot</param>
        /// <param name="_amount">amount it hurt for</param>
        /// <param name="_skill">what skill did the dot</param>
        public void apply_effect(string _time, string _target, int _amount, string _skill)
        {
            DateTime time = DateTime.Parse(_time);
            Skill skill = (Skill)Skills.list[_skill];

            // the list for completed effects after applied tick
            List<Action> completed_effects = new List<Action>(); 

            foreach (Action action in list)
            {
                // timespan between action's last tick and current effect's time
                TimeSpan ts = time - action.last_tick;

                // if it's in the time range of skills effect_period and effect_tick intervals
                // if skill's match
                // if target's match
                if (ts.Seconds + 1 >= skill.effect_tick && action.skill == skill && action.target == _target) 
                {
                    // if no ticks happened yet
                    if (action.ticks == null)
                    {
                        // prepare the list
                        action.ticks = new List<int>(); 
                    }

                    // let the tick happen
                    action.ticks.Add(_amount); 
                    action.last_tick = time;

                    if (skill.is_overtime_effect)
                    {
                        if (skill.effect_type == EFFECT_TYPES.HEAL)
                        {
                            action.who.healing += _amount;
                        }
                        else
                        {
                            action.who.damage += _amount;
                        }
                    }

                    // check if effect ticks is complete
                    if (action.ticks.Count >= skill.maximum_ticks)
                    { 
                        completed_effects.Add(action);
                    }

                    Console.WriteLine(action.last_tick_ToString());

                    break;
                }
            }

            // remove the completed effects
            foreach (Action action in completed_effects)
            {
                list.Remove(action);
            }
        }

        /// <summary>
        /// Generic DOT
        /// </summary>
        ~OverTimeEffectTracker()
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
            //If system hasn't disposed already
            if (!disposed)
            {
                //And we are disposing it
                if (disposing)
                {
                    //Nuke the list of DOTs
                    if (list != null)
                    {
                        foreach (Action a in list)
                        {
                            a.Dispose();
                        }
                        list.Clear();
                        list = null;
                    }
                }
            }
            disposed = true;
        }

    }
}
