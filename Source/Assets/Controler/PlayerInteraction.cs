using UnityEngine;
using System.Collections;

//Script to tell enemeies how to interact with the player.
//If a player is hit the network manager is told to decrease the player's score and the enemey is told to destroy itself.
public class PlayerInteraction : MonoBehaviour {


	// Use this for initialization
	void Start () {
        Destroy(gameObject.transform.parent.gameObject, 20);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
      
    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "LocalPlayer")
        {
            GameObject.FindGameObjectWithTag("NetworkManager").BroadcastMessage("DecreaseScore");
            gameObject.BroadcastMessage("Suicide");
        }
    }
}
