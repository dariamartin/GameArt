using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Serializable]
public class BaseEnemy: Base
{

    public enum Type
    {
        Meat,
        Veg,
        Fruit,
        Sweet
    }
    public enum Rarity
    {
        COMMON,
        UNCOMMON,
        RARE
    }

    public Type EnemyType;
    public Rarity EnemyRarity;

    

}
