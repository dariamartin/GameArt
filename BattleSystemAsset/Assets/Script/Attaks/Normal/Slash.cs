using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : BaseAttack {

	public Slash()
    {
        AtkName = "Slash";
        AtkDescription = "Quick slash with weapon";
        AtkDmg = 10f;
        AtkCost = 0;
    }
}
