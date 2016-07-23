using UnityEngine;
using System.Collections;


//Code attache dto the spawners that instantiate the enemies.
//If the application is the server side application then it activly instatiates enemy prefabs
//Instantiated enemies choose a player at random to target. When spawned a spawn message is sent to the network manager
//If the application is client side this object waits to recieve a spawn message from the network and then instantiates it using the parameters given by the message
public class Spawner : MonoBehaviour {
    public GameObject m_LocalPlayer;
    public GameObject m_RemotePlayer;
    public GameObject m_BadGuyPrefab;

    public float m_BadGuysPerSecond = 1;
    public float m_IncreasePerBadGuy = 0.01f;
    float m_Timer = 0;
    public int m_DiskNum = 0;
    public bool m_Server = true;

    public Transform[] m_SpawnPoints; // must be provided in editor
    int currentSpawn = 0;
    OutgoingStack m_OutputMessage;

	void Start () {
        m_OutputMessage = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<OutgoingStack>();
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Server)
        {
            if (currentSpawn < m_SpawnPoints.Length - 1)
            {
                currentSpawn++;
            }
            else
            {
                currentSpawn = 0;
            }

            m_Timer += Time.deltaTime;

            if (m_Timer > 1 / m_BadGuysPerSecond)
            {
                m_Timer = 0;
                if (Random.Range(0, 2) == 0)
                {
                    Spawn(m_LocalPlayer.transform.position, m_SpawnPoints[currentSpawn].position);
                }
                else
                {
                    Spawn(m_RemotePlayer.transform.position, m_SpawnPoints[currentSpawn].position);
                }
                m_BadGuysPerSecond += m_IncreasePerBadGuy;

            }
        }
        
	}

    public void Spawn(Vector3 target, Vector3 location)
    {
        m_DiskNum++;
        GameObject obj = (GameObject)Instantiate(m_BadGuyPrefab, location, Quaternion.identity);
        obj.GetComponent<BadGuyControler>().Initialise(target);
        obj.transform.name = "Disk" + m_DiskNum;
        if (m_Server)
        {
            NetworkMessage msg = new NetworkMessage();
            msg.InitialiseSpawn(obj, location, Quaternion.identity, target);
            string s_Msg = JsonUtility.ToJson(msg);
            s_Msg = s_Msg + "|";
            m_OutputMessage.AddMessage(s_Msg);
        }
        
    }

    public void NetworkSpawn(Vector3 target, Vector3 location, string name)
    {
        GameObject obj = (GameObject)Instantiate(m_BadGuyPrefab, location, Quaternion.identity);
        obj.GetComponent<BadGuyControler>().Initialise(target);
        obj.transform.name = name;
    }
}
