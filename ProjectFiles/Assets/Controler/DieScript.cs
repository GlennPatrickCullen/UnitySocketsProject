using UnityEngine;
using System.Collections;

//Script called to destory an enemey
//Eneies are destroyed in three ways:
//          1. They are hit by a local bullet
//          2. They are destroyed by a network event
//          3. They hit the local player and suicide

public class DieScript : MonoBehaviour {

    public GameObject m_Explosion;

    //Called when hit by a local bullet
    //Instantiates a death effect and creates a network message that this enemey is destroyed
    public void Die()
    {
        GameObject anim = (GameObject)Instantiate(m_Explosion, transform.position, Quaternion.identity);
        anim.GetComponent<RandomColorInChildren>().Initialise();

        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkDestroyer>().SendDestroyMessage(transform.parent.gameObject, 
                                                                                                               anim.GetComponent<RandomColorInChildren>().m_PrimaryColor,
                                                                                                               anim.GetComponent<RandomColorInChildren>().m_SecondaryColor);
        Destroy(gameObject);
    }

    //Called when destroyed by a network event
    //Instantiates a death effect of specified colors and destroys gameobject
    public void NetworkDie(Color32[] Colors)
    {
        GameObject anim =(GameObject)Instantiate(m_Explosion, transform.position, Quaternion.identity);
        anim.GetComponent<RandomColorInChildren>().Initialise(Colors[0], Colors[1], true);
        Destroy(gameObject);
    }

    //Called when an enemey hits a local player
    //Instantiates a death effect and creates a network message that this enemey is destroyed
    public void Suicide()
    {
        GameObject anim = (GameObject)Instantiate(m_Explosion, transform.position, Quaternion.identity);
        anim.GetComponent<RandomColorInChildren>().Initialise(Color.red, Color.red, false);

        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkDestroyer>().SendDestroyMessage(transform.parent.gameObject,
                                                                                                               anim.GetComponent<RandomColorInChildren>().m_PrimaryColor,
                                                                                                               anim.GetComponent<RandomColorInChildren>().m_SecondaryColor);
        Destroy(gameObject);
     }
}
