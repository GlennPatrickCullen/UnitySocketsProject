using UnityEngine;
using System.Collections;

// Class that handels destroying objects based on network messages and sending destroy messages over the network
// Room for improvement: Currently the class calls GameObject.Find() to find the target to destroy.

public class NetworkDestroyer : MonoBehaviour {

    OutgoingStack m_OutputMessage;
	// Use this for initialization
	void Start () {
        m_OutputMessage = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<OutgoingStack>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DestroyObject(NetworkMessage msg)
    {
        if(msg.m_MesssageType == MessageType.Destroy)
        {
            GameObject obj = GameObject.Find(msg.m_ObjectID);

            if(obj)
            {
                Color32[] array = new Color32[2];
                array[0] = msg.m_Color1;
                array[1] = msg.m_Color2;
                obj.BroadcastMessage("NetworkDie", array, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void SendDestroyMessage(GameObject obj, Color32 color1, Color32 color2)
    {
        // SIGNAL THE NETWORK
        NetworkMessage msg = new NetworkMessage();
        msg.InitialiseDestroy(obj, color1, color2);
        string s_Msg = JsonUtility.ToJson(msg);
        s_Msg = s_Msg + "|";
        m_OutputMessage.AddMessage(s_Msg);
    }
}

