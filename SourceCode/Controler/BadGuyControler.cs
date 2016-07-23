using UnityEngine;
using System.Collections;

//CSode for enemy controller.
//The enemey is spawned, looks at its target and then moves towards them at a constaant speed

public class BadGuyControler : MonoBehaviour {

    public float speed = 5.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * (speed * Time.deltaTime);
	}

    public void Initialise(Vector3 target)
    {
        transform.LookAt(target);
    }
}
