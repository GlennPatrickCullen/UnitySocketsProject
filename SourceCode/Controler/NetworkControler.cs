using UnityEngine;
using System.Collections;

// Character controler for character controlled by the network.
public class NetworkControler : MonoBehaviour {

	public float m_Scale = 10;
	float velocity;
	OutgoingStack m_OutputMessage;
	IncomingStack m_InputMessage;
	public float m_MessageResolution;
	float m_timer = 0;
    bool m_NetworkMovement = false;
    Quaternion m_OldUpdate;
    public GameObject m_Bullet;

    public AudioSource m_ShootBang;

    public Spawner m_Spawner;

    float m_RotationVelocity;
	// Use this for initialization
	void Start () {
		m_OutputMessage = GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent<OutgoingStack> ();
		m_InputMessage = GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent<IncomingStack> ();
        m_ShootBang = GetComponent<AudioSource>();
        m_RotationVelocity = 0;
	}
	
	// Update is called once per frame
	void Update () {
        m_NetworkMovement = false;
	}
	
    // If no network message has been recieved continue to move as instructed by the last network message
    void LateUpdate()
    {
        if (!m_NetworkMovement)
        {
            transform.position += transform.up * velocity;
            transform.Rotate(0f, 0f, (m_RotationVelocity/m_MessageResolution)*Time.deltaTime);
        }
    }
	

   public void ParseNetworkMessage(NetworkMessage msg)
    {
        switch(msg.m_MesssageType)
        {
            case MessageType.Move:
                transform.position = msg.m_Position;
                m_RotationVelocity = Quaternion.Angle( m_OldUpdate , msg.m_Rotation)*msg.m_RoationDirection;
                transform.rotation =  m_OldUpdate = msg.m_Rotation;
                velocity = msg.m_Velocity;
                m_NetworkMovement = true;
               // Debug.Log("Parse move");
                break;
            case MessageType.Spawn:
                if (msg.m_ObjectID.Contains("Bullet"))
                {
                    GameObject bul = (GameObject)Instantiate(m_Bullet, msg.m_Position, msg.m_Rotation);
                    bul.transform.name = msg.m_ObjectID;
                    m_ShootBang.Play();
                    //Debug.Log("Fire ");
                }
                if(msg.m_ObjectID.Contains("Disk"))
                {
                    m_Spawner.NetworkSpawn(msg.m_Target, msg.m_Position, msg.m_ObjectID);
                }
                break;
            case MessageType.Destroy:
                break;
        }
    }
}
