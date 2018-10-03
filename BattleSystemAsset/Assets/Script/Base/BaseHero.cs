using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//To make visible in inspector
[System.Serializable]

public class BaseHero: Base
{
    
    public int stam;
    public int inte;
    public int dex;
    public int agi;

    public List<BaseAttack> MagicAttack = new List<BaseAttack>();
}
