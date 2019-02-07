using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Stats are representing as instances of this class, eg STR is an instance of Stat that has an
 * additive mixin, int display, etc. Enums aren't powerful enough to do what we want in C#. Instead
 * there's a StatTag that hooks into this class.
 */
public class Stat {

    public CombinationStrategy combinator { get; private set; }
    public StatTag tag { get; private set; }
    public string nameShort { get; private set; }
    public bool useBinaryEditor { get; private set; }

    private static Dictionary<StatTag, Stat> stats;

    private Stat(StatTag tag, CombinationStrategy combinator, string nameShort, bool useBinaryEditor) {
        this.combinator = combinator;
        this.tag = tag;
        this.nameShort = nameShort;
        this.useBinaryEditor = useBinaryEditor;
    }

    public static Stat Get(StatTag tag) {
        if (stats == null || !stats.ContainsKey(tag)) {
            InitializeStats();
        }
        return stats[tag];
    }

    public static Stat Get(int enumIndex) {
        return Get((StatTag)enumIndex);
    }

    private static void InitializeStats() {
        stats = new Dictionary<StatTag, Stat>();
        AddStat(StatTag.MHP,            CombinationAdditive.Instance(), "Max HP");
        AddStat(StatTag.HP,             CombinationAdditive.Instance(), "HP");
        AddStat(StatTag.MAP,            CombinationAdditive.Instance(), "Max AP");
        AddStat(StatTag.AP,             CombinationAdditive.Instance(), "AP");
        AddStat(StatTag.MAG,            CombinationAdditive.Instance(), "MAG");
        AddStat(StatTag.DEF,            CombinationAdditive.Instance(), "DEF");
        AddStat(StatTag.RES,            CombinationAdditive.Instance(), "RES");
        AddStat(StatTag.PEN,            CombinationAdditive.Instance(), "PEN");
        AddStat(StatTag.HEAL,           CombinationAdditive.Instance(), "Heal %");
        AddStat(StatTag.EVADE,          CombinationAdditive.Instance(), "Evade %");
        AddStat(StatTag.STAGGER,        CombinationAdditive.Instance(), "Stagger");
        AddStat(StatTag.STAGGER_ATK,    CombinationAdditive.Instance(), "StaggerAtk");
        AddStat(StatTag.STAGGER_DEF,    CombinationAdditive.Instance(), "StaggerDef");
        AddStat(StatTag.STAGGER_PEN,    CombinationAdditive.Instance(), "StaggerPen");
        AddStat(StatTag.POISON_DEF,     CombinationAdditive.Instance(), "PoisonResist");
    }

    private static void AddStat(StatTag tag, CombinationStrategy combinator, string nameShort, bool useBinaryEditor=false) {
        if (!stats.ContainsKey(tag)) {
            stats[tag] = new Stat(tag, combinator, nameShort, useBinaryEditor);
        }
    }
}
