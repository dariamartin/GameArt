using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base
{
    public string theName;

    public float baseHp;
    public float curHp;

    public float baseMp;
    public float curMp;

    public float baseAtk;
    public float curAtk;

    public float baseDef;
    public float curDef;

    public List<BaseAttack> Atk = new List<BaseAttack>();
}
