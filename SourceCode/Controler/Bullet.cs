using UnityEngine;
using System.Collections;

//Code for controlling a bullet.
//Bullets are spawned either by the character controller (if fired by lockal palyer) or by the network manager (if fired by remote player)
//Bullets travel in a line at a set speed untill they reach their max range, at which point they are destroyed.
public class Bullet : MonoBehaviour {

    public float m_Speed = 10;
    public float m_Range = 50;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, m_Range / m_Speed);
	}
	
	// Update is called once per frame
    void Update()
    {
        transform.position += transform.up * (m_Speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            coll.gameObject.SendMessage("Die");

            GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkDestroyer>().SendDestroyMessage(gameObject,
                                                                                                                   new Color32((byte)0,(byte)0,(byte)0,(byte)0),
                                                                                                                   new Color32((byte)0,(byte)0,(byte)0,(byte)0));

            Destroy(gameObject);
            GameObject.FindGameObjectWithTag("NetworkManager").BroadcastMessage("IncreaseScore");
        }
    }

   
}
