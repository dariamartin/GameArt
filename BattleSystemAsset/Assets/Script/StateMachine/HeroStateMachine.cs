using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HeroStateMachine : MonoBehaviour {

    public BattleStateMachine BSM;
    public BaseHero Hero;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    public TurnState CurState;
    //for the ProgressBar
    //cd == cooldown
    private float cur_cd = 0f;
    private float max_cd = 5f;
    public Image ProgressBar;
    public GameObject selector;
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startPosition;
    private float animSpeed = 10f;
    //Dead or Not
    private bool Alive = true;
    //Hero Panel
    private HeroStat stats;
    public GameObject HeroPanel;
    private Transform HeroSpacer;

    void Start () {
        //find spacer
        HeroSpacer = GameObject.Find("BattleCanvas").transform.Find("HeroPanel").Find("HeroSpacer");
        //create panel, fill with info
        CreateHeroPanel();
       
        startPosition = transform.position;
        cur_cd = Random.Range(0, 2.5f);
        selector.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        CurState = TurnState.PROCESSING;
	}
	
	// Update is called once per frame
	void Update () {
        //test State
        //Debug.Log(CurState);

		switch (CurState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;
            case (TurnState.ADDTOLIST):
                //add to the bottom of heroToControl List
                BSM.HeroToControl.Add(this.gameObject);
                CurState = TurnState.WAITING;
                break;
            case (TurnState.WAITING):
                //idle
                break;
            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (TurnState.DEAD):
                if(!Alive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadHero";
                    //not attackable
                    BSM.HerosInCombat.Remove(this.gameObject);
                    //not managable
                    BSM.HeroToControl.Remove(this.gameObject);
                    //deactive selector 
                    selector.SetActive(false);
                    //reset GUI
                    BSM.ActionPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);
                    //remove from performlist
                    if (BSM.HerosInCombat.Count > 0)
                    {
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (BSM.PerformList[i].AttackGameObject == this.gameObject)
                            {
                                BSM.PerformList.Remove(BSM.PerformList[i]);
                            }
                            if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                            {
                                BSM.PerformList[i].AttackersTarget = BSM.HerosInCombat[Random.Range(0, BSM.HerosInCombat.Count)];
                            }
                        }
                    }
                    //change color and play dead animation
                    this.gameObject.GetComponent<SpriteRenderer>().material.color = new Color32(105, 105, 105, 255);
                    //reset heroInput
                    BSM.battleState = BattleStateMachine.PerformanceAction.CHECKALIVE;               
 
                    Alive = false;
                }
                break;
        }
	}

    void UpgradeProgressBar()
    {
        cur_cd = cur_cd + Time.deltaTime;
        float calc_cd = cur_cd / max_cd;
        //clamp between min and max
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cd, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (cur_cd >= max_cd)
        {
            CurState = TurnState.ADDTOLIST;
        }
    }
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        //animate the enemy near the hero to attack
        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x - 1.5f, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition)) { yield return null; }

        //wait
        yield return new WaitForSeconds(0.5f);
        //damage
        DoDamge();

        //animate return to start position
        Vector3 firstPosition = startPosition;
        while (MoveTowardsStart(firstPosition)) { yield return null; }

        //remove this perform for the list
        BSM.PerformList.RemoveAt(0);

        //reset the BSM > wait
        if (BSM.battleState != BattleStateMachine.PerformanceAction.WIN && BSM.battleState != BattleStateMachine.PerformanceAction.LOSE)
        {
            BSM.battleState = BattleStateMachine.PerformanceAction.WAIT;
            //reset Enemy State
            
            cur_cd = 0f;
            CurState = TurnState.PROCESSING;
        }
        else
        {
            CurState = TurnState.WAITING;
        }
        //end coroutine
        actionStarted = false;



    }

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    public void TakeDamage(float getDmgTaken) 
    {
        Hero.curHp -= getDmgTaken;
        if(Hero.curHp <= 0)
        {
            Hero.curHp = 0;
            CurState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }
    //do dmg
    void DoDamge()
    {
        float dmg_Calc = Hero.curAtk + BSM.PerformList[0].AttackSelect.AtkDmg;
        EnemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(dmg_Calc);
    }


    //Create hero panel
    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroStat>();
        stats.HeroName.text = Hero.theName;
        stats.HeroHP.text = "HP: " + Hero.curHp + "/" + Hero.baseHp;
        stats.HeroMP.text = "MP: " + Hero.curMp + "/" + Hero.baseMp;

        ProgressBar = stats.ProgressBar;
        HeroPanel.transform.SetParent(HeroSpacer, false);
    }
    void UpdateHeroPanel()
    {
        stats.HeroHP.text = "HP: " + Hero.curHp + "/" + Hero.baseHp;
        stats.HeroMP.text = "MP: " + Hero.curMp + "/" + Hero.baseMp;
    }
}
