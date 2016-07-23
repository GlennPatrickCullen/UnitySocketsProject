using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A list to store all outgoin gmessages created by the application
public class OutgoingStack : MonoBehaviour {

	public List<string> m_Outgoing;
	// Use this for initialization
	void Start () {
		m_Outgoing = new List<string> ();
	}

	public void AddMessage(string message)
	{
		m_Outgoing.Add(message);
	}
}
