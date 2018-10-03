using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
public class HandleTurn{
    //attack name
    public string Attacker;
    public string Type;
    public GameObject AttackGameObject; // who attack
    public GameObject AttackersTarget; // attack who

    //Attack Type
    public BaseAttack AttackSelect;
}
