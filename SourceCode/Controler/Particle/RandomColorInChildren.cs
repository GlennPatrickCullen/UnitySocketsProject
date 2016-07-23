using UnityEngine;
using System.Collections;

public class RandomColorInChildren : MonoBehaviour {

    public bool m_IsSet = false;
    public ParticleSystem m_Boom1;
    public ParticleSystem m_Boom2;

    public Color32 m_PrimaryColor;
    public Color32 m_SecondaryColor;

    //score to double size of explosion
    public float m_Scale = 25;
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //remost explosion
    public void Initialise(Color32 primary, Color32 secondary, bool useScore)
    {
        m_Boom1.startColor = m_PrimaryColor = primary;
        m_Boom2.startColor = m_SecondaryColor = secondary;

        m_IsSet = true;
        if (useScore)
        {
            SetRandius(m_Boom1, GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ScoreManager>().m_TheirScore);
            SetRandius(m_Boom2, GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ScoreManager>().m_TheirScore);
        }   
    }

    //local explosion
    public void Initialise()
    {

        m_Boom1.startColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        m_PrimaryColor = m_Boom1.startColor;
        m_Boom2.startColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        m_SecondaryColor = m_Boom2.startColor;

        SetRandius(m_Boom1, GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ScoreManager>().m_MyScore);
        SetRandius(m_Boom2, GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<ScoreManager>().m_MyScore);
    }

    void SetRandius(ParticleSystem system, int score)
    {
        system.gameObject.transform.localScale *= (1 + score / m_Scale);
    }
}
