using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour {

    public GameObject EnemyPrefab;

    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().EnemySelection(EnemyPrefab);

    }
    public void ShowToggle()
    {
        EnemyPrefab.transform.Find("Select").gameObject.SetActive(true);
    }
    public void HideToggle()
    {
        EnemyPrefab.transform.Find("Select").gameObject.SetActive(false);
    }
}
