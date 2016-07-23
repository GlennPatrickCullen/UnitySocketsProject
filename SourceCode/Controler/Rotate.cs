using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

    float m_RotateSpeed = 90;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, m_RotateSpeed * Time.deltaTime);
    }
}
