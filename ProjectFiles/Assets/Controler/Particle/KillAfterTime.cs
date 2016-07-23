using UnityEngine;
using System.Collections;

public class KillAfterTime : MonoBehaviour {

    public float m_Limit = 0.5f;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, m_Limit);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
