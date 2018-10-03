using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    Rigidbody2D rb;
    public float speed = 100f;

	// Use this for initialization
	void Start () { 

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0.0f);
        GetComponent<Rigidbody2D>().velocity = movement * speed * Time.deltaTime;
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "EnterTown")
        {
            CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();
            GameManager.instance.nextHeroPosition = col.spawnpoint.transform.position;
            GameManager.instance.sceneToLoad = col.sceneToload;
            GameManager.instance.LoadNextScene();
        }
        if (other.tag == "ExitTown")
        {
            CollisionHandler col = other.gameObject.GetComponent<CollisionHandler>();
            GameManager.instance.nextHeroPosition = col.spawnpoint.transform.position;
            GameManager.instance.sceneToLoad = col.sceneToload;
            GameManager.instance.LoadNextScene();
        }
    }
}
