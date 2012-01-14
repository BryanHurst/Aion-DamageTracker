using System;
using System.Collections.Generic;
using System.Text;

namespace DamageTracker
{
    /// <summary>
    /// Combat action performed by the player
    /// </summary>
    public class Action : IDisposable
    {
        public DateTime time;
        public Player who;
        public string target;
        public Skill skill;
        public int damage;
        public int healing;
        public List<int> ticks;
        public DateTime last_tick;
        public bool critical = false;
        private bool disposed = false;

        /// <summary>
        /// Create the action performed by the player
        /// </summary>
        /// <param name="_time">Time action occured</param>
        /// <param name="_who">Who did the action</param>
        /// <param name="_amount">Amount the action was for</param>
        /// <param name="_target">Who the action was done to</param>
        /// <param name="_skill">What skill did the action</param>
        /// <param name="_critical">If the in-game action was critted</param>
        public Action(string _time, Player _who, int _amount, string _target, string _skill, bool _critical)
        {
            try
            {
                time = DateTime.Parse(_time);
                who = _who;
                target = _target;
                critical = _critical;

                skill = (Skill)Skills.list[_skill];

                switch (skill.sub_type)
                {
                    case SUB_TYPES.ATTACK:
                    case SUB_TYPES.DEBUFF:
                        damage = _amount;
                        who.damage += _amount;
                        if (_amount > who.peak_damage)
                        {
                            who.peak_damage = _amount;
                        }
                        break;
                    case SUB_TYPES.HEAL:
                    case SUB_TYPES.BUFF:
                        healing = _amount;
                        who.healing += _amount;
                        if (_amount > who.peak_healing)
                        {
                            who.peak_healing = _amount;
                        }
                        break;
                }

                //If it's an overtime effect
                if (skill.is_overtime_effect)
                {
                    //Set the last tick value to initial damage time
                    last_tick = time;
                    Meter.active_meter.effect_tracker.track(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(">> Action Exception: time:" + _time + " who:" + who.name + " amount:" + _amount + " target:" + _target + " skill:" + _skill);
            }
        }

        /// <summary>
        /// Convert all log data of action to readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string tmp = "";
            try
            {
                tmp += ">> ";
                tmp += "[who: " + who.name + "] ";
                tmp += "[target: " + target + "] ";
                tmp += "[skill: " + skill.name;

                switch (skill.sub_type)
                {
                    case SUB_TYPES.ATTACK:
                    case SUB_TYPES.DEBUFF:
                        tmp += " (ATTACK)] [amount: " + damage + "]";
                        break;
                    case SUB_TYPES.HEAL:
                        tmp += " (HEAL)] [amount: " + healing + "]";
                        break;
                }
                if (critical) tmp += " [CRITICAL]";
                return tmp;
            }
            catch (Exception e) 
            {
                return ">> Exception: " + e.Message;
            }            
        }

        /// <summary>
        /// Take DOT or HOT tick and convert the last one 
        /// done to a readable string
        /// </summary>
        /// <returns></returns>
        public string last_tick_ToString()
        {
            string tmp = "";
            try
            {
                tmp += ">> ";
                tmp += "[who: " + who.name + "] ";
                tmp += "[target: " + target + "] ";
                tmp += "[skill: " + skill.name;

                if (skill.is_overtime_effect)
                {
                    switch (skill.sub_type)
                    {
                        case SUB_TYPES.ATTACK:
                        case SUB_TYPES.DEBUFF:
                            tmp += " (ATTACK)] [amount: " + damage + "]";
                            break;
                        case SUB_TYPES.HEAL:
                            tmp += " (HEAL)] [amount: " + healing + "]";
                            break;
                    }
                }
                return tmp;
            }
            catch (Exception e)
            {
                return ">> Exception: " + e.Message;
            }
        }

        /// <summary>
        /// Basic action
        /// </summary>
        ~Action()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose call
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //The GC shouldn't try to finalize the object as we are disposing it
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Internal dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            //If the action wasn't already disposed by the system
            if (!disposed)
            {
                //And we are disposing it
                if (disposing)
                {
                    //If DOT or HOT still active
                    if (ticks != null)
                    {
                        //Kill the DOT or HOT
                        ticks.Clear();
                        ticks = null;
                    }
                }
            }
            disposed = true;
        }
    }
}
