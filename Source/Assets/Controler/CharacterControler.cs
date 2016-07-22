using UnityEngine;
using System.Collections;

// A netowrked character controller for the spaceship
// should be added to the ship that the player will be controlling (different for server/client applications)

public class CharacterControler : MonoBehaviour {
	
    public float m_SpeedScale = 12;
    public float m_RotScale = 1;
    float m_velocity = 0;
    float m_rotation = 0;

    //cached output stack
	OutgoingStack m_OutputMessage;
    //freqency of messages
	public float m_MessageResolution;
	float m_timer = 0;

    //Bullet variables
    float m_BulletFire = 0;
    public float m_SecondPerBullet = 0.01f; 
    public GameObject m_Bullet;
    public AudioSource m_ShootBang;

    public int m_NetworkID = 1;
    public int m_BulletCount = 0;

	// Use this for initialization
    void Start()
    {
        Random.seed = 42;
        m_OutputMessage = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<OutgoingStack>();
        m_ShootBang = GetComponent<AudioSource>();
    }
	// Update is called once per frame
	void Update () {
		
        // movement input
        ParseInput ();
        transform.Rotate(0, 0,-m_rotation);
        transform.position += transform.up * m_velocity;

        //Send movement message over the network
        NetworkMessage msg = new NetworkMessage();
		msg.m_ObjectID= "1";
        msg.InitialiseMove(transform.position, transform.rotation, m_velocity, Mathf.Sign(-m_rotation));
		m_timer += Time.deltaTime;
		if (m_timer > m_MessageResolution) 
        {
           string s_Msg = JsonUtility.ToJson(msg);
           s_Msg = s_Msg + "|";
           m_OutputMessage.AddMessage(s_Msg);
			m_timer = 0;
		}

        Fire();
	}

 
	void ParseInput()
	{
        m_velocity = Input.GetAxis("Vertical") / m_SpeedScale;
        m_rotation = Input.GetAxis("Horizontal") / m_RotScale;
	}

    // read if the player wishes to fire a bullet
    // if they do, instantiate one and send a spawn message over the network
    void Fire()
    {
        m_BulletFire += Time.deltaTime;
        if (Input.GetButton("Fire1") && m_BulletFire >= m_SecondPerBullet)
        {
            m_BulletCount++;
            GameObject bul = (GameObject)Instantiate(m_Bullet, transform.position, transform.rotation);
            bul.transform.name = "Bullet-" + m_NetworkID + "-" + m_BulletCount;

            // SIGNAL THE NETWORK
            NetworkMessage msg = new NetworkMessage();
            msg.InitialiseSpawn(bul, transform.position, transform.rotation);
            string s_Msg = JsonUtility.ToJson(msg);
            s_Msg = s_Msg + "|";
            m_OutputMessage.AddMessage(s_Msg);

            m_BulletFire = 0;
            m_ShootBang.Play();
        }
    }
}
