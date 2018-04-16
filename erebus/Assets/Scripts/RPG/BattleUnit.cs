﻿using System.Collections;
using System.Collections.Generic;

// representation of a unit in battle
public class BattleUnit {

    public Unit Unit { get; private set; }
    public Battle Battle { get; private set; }
    public Doll doll { get; private set; }
    public Alignment Align { get; private set; }

    public BattleUnit(Unit unit, Battle battle, Alignment align) {
        this.Unit = unit;
        this.Battle = battle;
        this.Align = align;
    }
}
