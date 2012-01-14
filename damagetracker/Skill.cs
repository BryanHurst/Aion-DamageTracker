using System;
using System.Collections.Generic;
using System.Text;

namespace DamageTracker
{
    public enum TYPES
    {
        PHYSICAL,
        MAGICAL
    }

    public enum SUB_TYPES
    {
        NONE,
        ATTACK,
        BUFF,
        DEBUFF,
        HEAL,
        SUMMONTRAP,
        SUMMON,
        CHANT,
        SUMMONHOMING
    }

    public enum EFFECT_TYPES
    {
        NONE,
        WPN_MASTERY,
        AMR_MASTERY,
        SHIELDMASTERY,
        WPN_DUAL,
        EXTENDAURARANGE,
        BOOSTHATE,
        STATBOOST,
        CONDSKILLLAUNCHER,
        BOOSTHEALEFFECT,
        SUBTYPEEXTENDDURATION,
        WEAPONSTATBOOST,
        SUBTYPEBOOSTRESIST,
        SKILLATK_INSTANT,
        STATDOWN,
        STUN,
        HOSTILEUP,
        STATUP,
        SHIELD,
        SKILLATKDRAIN_INSTANT,
        ALWAYSPARRY,
        CLOSEAERIAL,
        BIND,
        HEAL_INSTANT,
        HEAL,
        SNARE,
        PROVOKER,
        DASHATK,
        SPELLATK_INSTANT,
        SPELLATK,
        ROOT,
        ALWAYSBLOCK,
        PROTECT,
        REFLECTOR,
        DISPELDEBUFF,
        WEAPONSTATUP,
        SLOW,
        BLEED,
        DISPEL,
        INVULNERABLEWING,
        FEAR,
        HIDE,
        ALWAYSDODGE,
        ALWAYSRESIST,
        SUMMONTRAP,
        ONETIMEBOOSTSKILLCRITICAL,
        BACKDASHATK,
        SILENCE,
        ONETIMEBOOSTSKILLATTACK,
        SHAPECHANGE,
        POISON,
        SEARCH,
        SLEEP,
        FPHEAL_INSTANT,
        DISPELDEBUFFPHYSICAL,
        DEBOOSTHEALAMOUNT,
        BOOSTSKILLCASTINGTIME,
        MPATTACK_INSTANT,
        BLIND,
        MOVEBEHINDATK,
        CARVESIGNET,
        SIGNETBURST,
        RESURRECT,
        REBIRTH,
        SUMMONSERVANT,
        CHANGEHATEONATTACKED,
        ONETIMEBOOSTHEALEFFECT,
        RESURRECTPOSITIONAL,
        DISPELDEBUFFMENTAL,
        MPHEAL,
        SWITCHHPMP_INSTANT,
        HEALCASTORONTARGETDEAD,
        SPELLATKDRAIN_INSTANT,
        MPHEAL_INSTANT,
        BOOSTSPELLATTACKEFFECT,
        BOOSTSKILLCOST,
        FPHEAL,
        AURA,
        HEALCASTORONATTACKED,
        SPELLATKDRAIN,
        DELAYEDSPELLATK_INSTANT,
        DEFORM,
        SUMMONSKILLAREA,
        SUMMONGROUPGATE,
        RANDOMMOVELOC,
        SUMMONBINDINGGROUPGATE,
        MAGICCOUNTERATK,
        PETORDERUSEULTRASKILL,
        RECALL_INSTANT,
        SUMMONHOMING,
        SUMMON,
        DISPELBUFF,
        DISPELBUFFCOUNTERATK,
        SWITCHHOSTILE,
        FPATK_INSTANT,
        FPATK,
        RETURNHOME,
        DEATHBLOW,
        DPTRANSFER,
        POLYMORPH,
        NOFLY,
        FALL,
        CANNON,
        DPHEAL_INSTANT,
        DPHEAL,
        DELAYEDFPATK_INSTANT,
        MPATTACK,
        RESURRECTBASE,
        ACTIVATE_ENSLAVE,
        RETURNPOINT,
        STAGGER,
        STUMBLE,
        SIMPLE_ROOT,
        SPIN,
        OPENAERIAL,
        PROCATK_INSTANT,
        CONVERTHEAL,
        PROCFPHEAL_INSTANT,
        PARALYZE,
        PROCMPHEAL_INSTANT,
        PROCHEAL_INSTANT,
        SIGNET,
        PROCDPHEAL_INSTANT,
        PULLED,
        BOOSTDROPRATE,
        DUMMY,
        CURSE,
        XPBOOST,
        SKILLLAUNCHER,
        PETRIFICATION,
        DISEASE,
        CONFUSE,
        DISPELNPCBUFF,
        DISPELNPCDEBUFF,
        SANCTUARY
    }

    /// <summary>
    /// Performable skill
    /// </summary>
    public class Skill
    {
        public int id;
        public string name;
        public TYPES type;
        public SUB_TYPES sub_type;
        public EFFECT_TYPES effect_type;
        public bool is_overtime_effect = false;
        public int effect_period = 0;
        public int effect_tick = 0;
        public int maximum_ticks = 0;

        public Skill(int _id, string _name, TYPES _type, SUB_TYPES _sub_type, EFFECT_TYPES _effect_type, int _effect_period, int _effect_tick)
        {
            id = _id;
            name = _name;
            type = _type;
            sub_type = _sub_type;
            effect_type = _effect_type;
            switch (effect_type)
            {
                case DamageTracker.EFFECT_TYPES.SPELLATK:
                case DamageTracker.EFFECT_TYPES.BLEED:
                case DamageTracker.EFFECT_TYPES.POISON:
                case DamageTracker.EFFECT_TYPES.HEAL:
                    is_overtime_effect = true;
                    break;
            }
            effect_period = _effect_period;
            effect_tick = _effect_tick;
            if (effect_tick != 0)
            {
                maximum_ticks = effect_period / effect_tick;
            }
        }

        public Skill()
        { 
        }
    }
}
