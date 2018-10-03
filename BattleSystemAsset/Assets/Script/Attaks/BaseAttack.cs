using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string AtkName;
    public string AtkDescription;
    public float AtkDmg; // Base dmg15 , melee sta 35 = basedmg + (lvl / stamina)
    public float AtkCost; // Mana

    
}
