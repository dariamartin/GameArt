using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : BaseAttack {

	public Poison()
    {
        AtkName = "Poison";
        AtkDescription = "Poison deal damage over time";
        AtkDmg = 5f;
        AtkCost = 5f;
    }
}
