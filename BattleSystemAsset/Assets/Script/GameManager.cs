using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    //hero
    public GameObject Player;

    //postion
    public Vector3 nextHeroPosition;
    public Vector3 lastHeroPosition; // battle

    //SCENCE
    public string sceneToLoad;
    public string lastScene; //battle


    void Awake()
    {
        //check instance exist
        if(instance == null)
        {
            //set instance to this if not occur
            instance = this;
        }
        //if it exist but is not this instance
        else if (instance != this)
        {
            //destroy
            Destroy(gameObject);

        }
        //set this to be not destroyable
        DontDestroyOnLoad(gameObject);
        if(!GameObject.Find("Hero"))
        {
            GameObject Hero = Instantiate(Player, Vector3.zero, Quaternion.identity) as GameObject;
            Hero.name = "Hero";
        }
    }
   
    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
