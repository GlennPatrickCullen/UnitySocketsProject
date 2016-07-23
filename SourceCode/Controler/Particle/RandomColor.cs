using UnityEngine;
using System.Collections;

public class RandomColor : MonoBehaviour {

    public bool  m_IsSet = false;
	// Use this for initialization
	void Awake () {
        if (!m_IsSet)
        {
            gameObject.GetComponent<ParticleSystem>().startColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
