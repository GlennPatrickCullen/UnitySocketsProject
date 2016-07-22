using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Class for recording both players' scores and sending updates on loacal score over the network
public class ScoreManager : MonoBehaviour {

    public int m_MyScore = 0;
    public int m_TheirScore = 0;

    public Text m_MyDisplay;
    public Text m_TheirDisplay;

    int m_OldScore = 0;

    OutgoingStack m_OutputMessage;
  
       
    
	// Use this for initialization
	void Start () {
        m_OutputMessage = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<OutgoingStack>();
	}
	
	// Update is called once per frame
	void Update () {
        m_MyDisplay.text = m_MyScore.ToString();
        m_TheirDisplay.text = m_TheirScore.ToString();

        if(m_OldScore != m_MyScore)
        {
            SendScoreUpdate();
            m_OldScore = m_MyScore;
        }
	}

    public void IncreaseScore()
    {
        m_MyScore++;
    }
    public void DecreaseScore()
    {
        m_MyScore -= 2;

        if (m_MyScore < 0)
            m_MyScore = 0;
    }

    public void SetTheirScore(int TheirScore)
    {
        m_TheirScore = TheirScore;
    }

    void SendScoreUpdate()
    {
        // SIGNAL THE NETWORK
        NetworkMessage msg = new NetworkMessage();
        msg.InitialiseScoreUpdate(m_MyScore);
        string s_Msg = JsonUtility.ToJson(msg);
        s_Msg = s_Msg + "|";
        m_OutputMessage.AddMessage(s_Msg);
    }
}
