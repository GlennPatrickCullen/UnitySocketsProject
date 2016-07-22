using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Class that was previously used to store incoming messages before they were passed to the application
 * this remains included for future expansion as if the messages needed to be sorted further this would be where the logic woul dbe placed
 */
public class IncomingStack : MonoBehaviour {

	public List<string> m_Incoming;
	// Use this for initialization
	void Start () {
		m_Incoming = new List<string> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.LogError (m_Incoming.Count);
		while (m_Incoming.Count>0) {
			//Debug.LogError(m_Incoming[0]);
			m_Incoming.RemoveAt(0);
		}
	}
	
	public void AddMessage(string message)
	{
		m_Incoming.Add(message);
	}
}
