using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateMachine : MonoBehaviour {



    public enum PerformanceAction
    {
        WAIT,
        TAKEACTION,
        PERFORMANCE,
        CHECKALIVE,
        WIN,
        LOSE
    }
    public PerformanceAction battleState;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> HerosInCombat = new List<GameObject>();
    public List<GameObject> EnemyInCombat = new List<GameObject>();

    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        ATTACK,
        SELECT,
        DONE
    }
    public HeroGUI HeroInput;
    public List<GameObject> HeroToControl = new List<GameObject>();
    private HandleTurn HeroChoice;

    public GameObject enemyButton;
    public Transform Spacer;

    public GameObject ActionPanel;
    public GameObject MagicPanel;
    public GameObject EnemySelectPanel;

    //Hero Action
    public Transform actionSpacer;
    public Transform magicSpacer;
    public GameObject actionButton;
    public GameObject magicButton;
    private List<GameObject> AtkButtons = new List<GameObject>();

    //Enemy Action
    private List<GameObject> EnemyBtns = new List<GameObject>();

    void Start() {
        battleState = PerformanceAction.WAIT;
        EnemyInCombat.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HerosInCombat.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        HeroInput = HeroGUI.ACTIVATE;

        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        MagicPanel.SetActive(false);
        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            case (PerformanceAction.WAIT):
                if (PerformList.Count > 0)
                {
                    battleState = PerformanceAction.TAKEACTION;
                }
                break;
            case (PerformanceAction.TAKEACTION):
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                if (PerformList[0].Type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                        for(int i = 0; i < HerosInCombat.Count; i++)
                        {
                            if(PerformList[0].AttackersTarget == HerosInCombat[i])
                            {
                                ESM.HeroToAttack = PerformList[0].AttackersTarget;
                                ESM.CurState = EnemyStateMachine.TurnState.ACTION;
                                break;
                            }
                            else
                            {
                                PerformList[0].AttackersTarget = HerosInCombat[Random.Range(0, HerosInCombat.Count)];
                                ESM.HeroToAttack = PerformList[0].AttackersTarget;
                                ESM.CurState = EnemyStateMachine.TurnState.ACTION;
                            }
                        }
                }
                if (PerformList[0].Type == "Hero")
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.EnemyToAttack = PerformList[0].AttackersTarget;
                    HSM.CurState = HeroStateMachine.TurnState.ACTION;
                }
                battleState = PerformanceAction.PERFORMANCE;
                break;
            case (PerformanceAction.PERFORMANCE):
                break;
            case (PerformanceAction.CHECKALIVE):
                if (HerosInCombat.Count < 1) // LOSE
                {
                    battleState = PerformanceAction.LOSE;
                }
                else if (EnemyInCombat.Count < 1) // WIN
                {
                    battleState = PerformanceAction.WIN;
                }
                else
                {
                    //Call Function
                    ClearAttackPanel();
                    HeroInput = HeroGUI.ACTIVATE;
                }
                break;
            case (PerformanceAction.LOSE):
                {
                    Debug.Log("LOSE :(");
                }
                break;
            case (PerformanceAction.WIN):
                {
                    Debug.Log("WIN!");
                    for (int i = 0; i < HerosInCombat.Count; i++)
                    {
                        HerosInCombat[i].GetComponent<HeroStateMachine>().CurState = HeroStateMachine.TurnState.WAITING;
                    }
                }
                break;
        }
        switch (HeroInput)
        {
            case (HeroGUI.ACTIVATE):
                if (HeroToControl.Count > 0)
                {
                    HeroToControl[0].transform.Find("Select").gameObject.SetActive(true);
                    HeroChoice = new HandleTurn();
                    ActionPanel.SetActive(true);
                    //create attack panel
                    CreateAtkButton();

                    HeroInput = HeroGUI.WAITING;
                }
                break;
            case (HeroGUI.WAITING):
                //idle
                break;
            case (HeroGUI.DONE):
                HeroInputDone();
                break;
        }
    }

    public void CollectAction(HandleTurn input)
    {
        PerformList.Add(input);
    }

    public void EnemyButtons()
    {
        //clean up
        foreach (GameObject enemyBtn in EnemyBtns)
        {
            Destroy(enemyBtn);
        }
        EnemyBtns.Clear();
        //Create
        foreach (GameObject enemy in EnemyInCombat)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();

            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.GetComponentInChildren<Text>();
            //Text buttonText = newButton.transform.Find ("Text").gameObject.GetComponent<Text>();
            buttonText.text = cur_enemy.Enemy.theName;

            button.EnemyPrefab = enemy;
            //all local setup will hold up in place
            newButton.transform.SetParent(Spacer, false);
            EnemyBtns.Add(newButton);
        }
    }
    //input 1
    public void Attack()
    {
        HeroChoice.Attacker = HeroToControl[0].name;
        HeroChoice.AttackGameObject = HeroToControl[0];
        HeroChoice.Type = "Hero";
        HeroChoice.AttackSelect = HeroToControl[0].GetComponent<HeroStateMachine>().Hero.Atk[0];
        ActionPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);
    }
    // input 2
    public void EnemySelection(GameObject chooseEnemy)
    {
        HeroChoice.AttackersTarget = chooseEnemy;
        HeroInput = HeroGUI.DONE;
    }
    void HeroInputDone()
    {
        PerformList.Add(HeroChoice);
        //EnemySelectPanel.SetActive(false);

        //Clean Attack panel
        ClearAttackPanel();

        HeroToControl[0].transform.Find("Select").gameObject.SetActive(false);
        HeroToControl.RemoveAt(0);
        HeroInput = HeroGUI.ACTIVATE;
    }

    void ClearAttackPanel()
    {
        EnemySelectPanel.SetActive(false);
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(false);
        foreach (GameObject AtkButton in AtkButtons)
        {
            Destroy(AtkButton);
        }
        AtkButtons.Clear();

    }
    //create actionButton
    void CreateAtkButton()
    {
        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.Find("Action").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Attack());
        AttackButton.transform.SetParent(actionSpacer, false);
        AtkButtons.Add(AttackButton);

        GameObject MagicAtkButton = Instantiate(actionButton) as GameObject;
        Text MagicAtkText = MagicAtkButton.transform.Find("Action").gameObject.GetComponent<Text>();
        MagicAtkText.text = "Magic";
        MagicAtkButton.GetComponent<Button>().onClick.AddListener(() => switching());
        MagicAtkButton.transform.SetParent(actionSpacer, false);
        AtkButtons.Add(MagicAtkButton);

        if(HeroToControl[0].GetComponent<HeroStateMachine>().Hero.MagicAttack.Count > 0)
        {
            foreach(BaseAttack magicAtk in HeroToControl[0].GetComponent<HeroStateMachine>().Hero.MagicAttack)
            {
                GameObject MagicButton = Instantiate(magicButton) as GameObject;
                Text MagicButtonText = MagicButton.transform.Find("Action").gameObject.GetComponent<Text>();
                MagicButtonText.text = magicAtk.AtkName;

                AttackButton ATB = MagicButton.GetComponent<AttackButton>();
                ATB.MagicToPerform = magicAtk;
                MagicButton.transform.SetParent(magicSpacer, false);
                AtkButtons.Add(MagicButton);
            }
        }
        else
        {
            MagicAtkButton.GetComponent<Button>().interactable = false;
        }
    }
    //input 4
    public void Magic(BaseAttack choosenMagic)
    {
        HeroChoice.Attacker = HeroToControl[0].name;
        HeroChoice.AttackGameObject = HeroToControl[0];
        HeroChoice.Type = "Hero";

        HeroChoice.AttackSelect = choosenMagic;
        MagicPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);

    }
    //input 3
    public void switching()
    {
        ActionPanel.SetActive(false);
        MagicPanel.SetActive(true);
    }

}
