using UnityEngine;
using System.Collections;

// Bullet spawned by network event
// Does not kill enemeies or increase score, these effects should be boradcast over the network when they happen on the remote machine
public class DummyBullet : MonoBehaviour
{

    public float m_Speed = 10;
    public float m_Range = 50;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, m_Range / m_Speed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * (m_Speed * Time.deltaTime);
    }


    public void NetworkDie(Color32[] Colors)
    {
        Destroy(gameObject);
    }

}
