using UnityEngine;
using System.Collections;
using System;


// Network message class.
//Previously this was handeled through inheritence however this caused problems with Unity's JSON serialiser.

public enum MessageType {Move,Spawn,Destroy,ScoreUpdate};

[Serializable]

public class NetworkMessage {

    // Type of message
    public MessageType m_MesssageType;
    //ID of object
    public string m_ObjectID;
    //Variables for Movement
    public Vector3 m_Position;
    public Quaternion m_Rotation;
    public float m_RoationDirection;
    public float m_Velocity;

    //Variables for Spawn
    public Vector3 m_Target;

    //Variables for Destroy
    public Color32 m_Color1;
    public Color32 m_Color2;

    //variables for score update
    public int m_Score;

    public void InitialiseMove( Vector3 position, Quaternion rotation, float velocity,float rotationDirection)
    {
        m_MesssageType = MessageType.Move;
        m_Rotation = rotation;
        m_Position = position;
        m_Velocity = velocity;
        m_RoationDirection = rotationDirection;
    }

    public void InitialiseSpawn(GameObject Item, Vector3 position, Quaternion rotation)
    {
        m_MesssageType = MessageType.Spawn;
        m_ObjectID = Item.transform.name;
        m_Position = position;
        m_Rotation = rotation;
        m_Velocity = 0;
        
    }

    public void InitialiseSpawn(GameObject Item, Vector3 position, Quaternion rotation, Vector3 target)
    {
        m_MesssageType = MessageType.Spawn;
        m_ObjectID = Item.transform.name;
        m_Position = position;
        m_Rotation = rotation;
        m_Velocity = 0;
        m_Target = target;
    }

    public void InitialiseDestroy(GameObject Item, Color32 PrimaryColor, Color32 SecondaryColor)
    {
        m_MesssageType = MessageType.Destroy;
        m_ObjectID = Item.transform.name;
        m_Color1 = PrimaryColor;
        m_Color2 = SecondaryColor;
    }

    public void InitialiseScoreUpdate(int score)
    {
        m_MesssageType = MessageType.ScoreUpdate;
        m_Score = score;
    }

 
}

