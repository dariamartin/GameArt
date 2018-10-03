using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour {

    private BattleStateMachine BSM;
    public BaseEnemy Enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState CurState;
    //for the ProgressBar
    //cd == cooldown
    private float cur_cd = 0f;
    private float max_cd = 10f;
    public Image ProgressBar;
    //this gameoject
    private Vector3 startPosition;
    //Waiting Time
    private bool actionStarted = false;
    public GameObject HeroToAttack;
    public GameObject selector;
    private float animSpeed = 10f;
    //Dead or Not
    private bool Alive = true;
    //Enemy Panel


    
    void Start () {
     

        CurState = TurnState.PROCESSING;
        selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        switch (CurState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.CHOOSEACTION):
                ChooseAction();
                CurState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                //idle
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());

                break;
            case (TurnState.DEAD):
                if (!Alive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadEnemy";
                    //not attackable
                    BSM.EnemyInCombat.Remove(this.gameObject);
                    //deactive selector
                    selector.SetActive(false);
                    //remove from performlist
                    if (BSM.EnemyInCombat.Count > 0)
                    {
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (BSM.PerformList[i].AttackGameObject == this.gameObject)
                            {
                                BSM.PerformList.Remove(BSM.PerformList[i]);
                            }
                            if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                            {
                                BSM.PerformList[i].AttackersTarget = BSM.EnemyInCombat[Random.Range(0, BSM.EnemyInCombat.Count)];
                            }

                        }
                    }
                    //change color and play dead animation
                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //Alive is false
                    Alive = false;
                    //reset Enemybutton
                    BSM.EnemyButtons();
                    //CheckAlive
                    BSM.battleState = BattleStateMachine.PerformanceAction.CHECKALIVE;
                }
                break;
        }
    }
    void UpgradeProgressBar()
    {
        cur_cd = cur_cd + Time.deltaTime;
        if (cur_cd >= max_cd)
        {
            CurState = TurnState.CHOOSEACTION;
        }
    }
    void ChooseAction ()
    {
        HandleTurn myAttack = new HandleTurn();
        //get name
        myAttack.Attacker = Enemy.theName;
        myAttack.Type = "Enemy";
        //which one is attacking
        myAttack.AttackGameObject = this.gameObject;
        //random choose a target 
        myAttack.AttackersTarget = BSM.HerosInCombat[Random.Range(0, BSM.HerosInCombat.Count)];

        int num = Random.Range(0, Enemy.Atk.Count);
        myAttack.AttackSelect = Enemy.Atk[num];
        //Debug.Log(this.gameObject.name + "has Choosen" + myAttack.AttackSelect.AtkName);
        BSM.CollectAction(myAttack);
    }
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack
        Vector3 HeroPosition = new Vector3(HeroToAttack.transform.position.x+1.5f, HeroToAttack.transform.position.y, HeroToAttack.transform.position.z);
        while(MoveTowardsEnemy(HeroPosition)) {yield return null;}

        //wait
        yield return new WaitForSeconds(0.5f);
        //damage
        DoDamage();
        //animate return to start position
        Vector3 firstPosition = startPosition;
        while(MoveTowardsStart(firstPosition)) { yield return null; }

        //remove this perform for the list
        BSM.PerformList.RemoveAt(0);

        //reset the BSM > wait
        BSM.battleState = BattleStateMachine.PerformanceAction.WAIT;

        actionStarted = false;
        cur_cd = 0f;
        CurState = TurnState.PROCESSING;

    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void DoDamage()
    {
        float dmg_calc = Enemy.curAtk + BSM.PerformList[0].AttackSelect.AtkDmg;
        HeroToAttack.GetComponent<HeroStateMachine>().TakeDamage(dmg_calc);
    }

    public void TakeDamage(float getDamage)
    {
        Enemy.curHp -= getDamage;
        if(Enemy.curHp <= 0)
        {
            Enemy.curHp = 0;
            CurState = TurnState.DEAD;
        }
    } 

  
}
