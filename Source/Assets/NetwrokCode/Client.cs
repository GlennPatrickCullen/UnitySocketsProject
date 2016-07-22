using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


//Client network manager
//Implements an asynchronous TCP connection that sends and recieves JSON messages
//These messages are read from the OutgoingStack object and serialized before being sent over the network.
//All recieved messages are deserialized and placed on the IncomingStack object. They are then communicated to the application in the Update loop using the cached network managers.
//NOTE: Currently port 5500 and 5501 need to be available for this project to work

public class Client : MonoBehaviour {

	AsynchronousClient m_Client;
	IncomingStack m_Incoming;
    OutgoingStack m_Outgoing;

    // cached network managers
    NetworkDestroyer m_NetworkDestroyer;
    NetworkControler m_NetworkControler;

    // Creats new AsynchronousClient object and finds required network variables to cache
	void Start () {
		m_Client = new AsynchronousClient ();
		m_Client.StartClient ();
		//AsynchronousClient.StartClient ();
		m_Incoming = gameObject.GetComponent<IncomingStack> ();
        m_Outgoing = gameObject.GetComponent<OutgoingStack>();

        m_NetworkDestroyer = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkDestroyer>();
        m_NetworkControler = GameObject.FindGameObjectWithTag("Player").GetComponent<NetworkControler>();
	}
	
	// Update is called once per frame
    //Cycles through all messages generated this frame and sends them
    //Reads all messages ecieved this frame and sends them to relevant game objects
	void Update () {
		

        //send messages to server
        
        if (m_Outgoing.m_Outgoing.Count > 0)
        {
            m_Client.SendStack(m_Outgoing.m_Outgoing);
        }

        m_Client.ReceiveStack(m_Client.client, m_Incoming.m_Incoming);
        while (m_Client.m_MessagesRecieved.Count > 0)
        {
            if (m_Client.m_MessagesRecieved[0].m_MesssageType == MessageType.Destroy)
            {
                m_NetworkDestroyer.DestroyObject(m_Client.m_MessagesRecieved[0]);
            }
            else  if (m_Client.m_MessagesRecieved[0].m_MesssageType == MessageType.ScoreUpdate)
            {
                BroadcastMessage("SetTheirScore", m_Client.m_MessagesRecieved[0].m_Score);
            }
            else
            {
                m_NetworkControler.ParseNetworkMessage(m_Client.m_MessagesRecieved[0]);
            }
            m_Client.m_MessagesRecieved.RemoveAt(0);
        }
	}

	void OnApplicationQuit()
	{
		m_Client.ShutDown ();
	}
}



// State object for receiving data from remote device.
public class StateObject {
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
    public const int BufferSize = 5000;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
 
	public List<string> incomingStack; 
}

public class AsynchronousClient {
	// The port number for the remote device.
	private const int port = 5500;
				public Socket client;
	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent sendDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent receiveDone = 
		new ManualResetEvent(false);

    public List<NetworkMessage> m_MessagesRecieved;
	// The response from the remote device.
	private static String response = String.Empty;

    private bool m_Locked = false;
	public  void StartClient() {
		// Connect to a remote device.
		try {

            string ip = System.IO.File.ReadAllText("./ServerIP.txt");
            IPAddress ipAddress = IPAddress.Parse(ip);
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
            ip = System.IO.File.ReadAllText("./ClientIP.txt");
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(ip), 5501);

			// Create a TCP/IP socket.
			client = new Socket(AddressFamily.InterNetwork,
			                           SocketType.Stream, ProtocolType.Tcp);
			
			// Connect to the remote endpoint.
			client.Bind(localEP);
			client.BeginConnect( remoteEP, 
			                    new AsyncCallback(ConnectCallback), client);
			connectDone.WaitOne(0);

			while(!client.Connected)
			{
			}
			// Send test data to the remote device.
			Send(client,"This is a test<EOF>");
			sendDone.WaitOne(0);
			
			// Receive the response from the remote device.
			Receive(client);
			receiveDone.WaitOne(0);

            // initialise message list
            m_MessagesRecieved = new List<NetworkMessage>();
		} catch (Exception e) {
			Debug.LogError(e.ToString());
		}
	}
	
	private  void ConnectCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete the connection.
			client.EndConnect(ar);
			
			//Debug.LogError("Socket connected to "+
			//                  client.RemoteEndPoint.ToString());
			
			// Signal that the connection has been made.
			connectDone.Set();
		} catch (Exception e) {
			Debug.LogError(e.ToString());
		}
	}
	
	private  void Receive(Socket client) {
		try {
			// Create the state object.
			StateObject state = new StateObject();
			state.workSocket = client;
			
			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallback), state);
		} catch (Exception e) {
			Debug.LogError(e.ToString());
		}
	}
	
	private  void ReceiveCallback( IAsyncResult ar ) {
		try {

			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			
			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar);
			
			// All the data has arrived; put it in response.
			state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));

			if (state.sb.Length > 1) {
                // PUT MESSAGE ON STACK
				response = state.sb.ToString();
				//Debug.LogError("START I recieved" + response);
			}
				// Signal that all bytes have been received.
				receiveDone.Set();
	
		} catch (Exception e) {
			Debug.LogError(e.ToString());
		}
	}

	public  void ReceiveStack(Socket client, List<string> income) {
		try {
			// Create the state object.
			StateObject state = new StateObject();
			state.workSocket = client;
			state.incomingStack = income;
			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveStackCallback), state);
		} catch (Exception e) {
			Debug.LogError(e.ToString());
		}
	}
	
    // Waits for all data to be read from stream
    // When it has been read the string is passed into Unity's JSON deserializer. The resulting object is then added to the recieved stack
	private void ReceiveStackCallback(IAsyncResult ar)
    {
        if (!m_Locked)
        {
            m_Locked = true;
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // All the data has arrived; put it in response.
                if (state.sb.Length > 1)
                {
                    // PUT MESSAGE ON STACK
                    response = state.sb.ToString();
                    state.incomingStack.Add(response);
                   // Debug.Log("I recieved" + response);

                    //Debug.Log(response);
                    String[] splitResponses = response.Split('|');
                    foreach (string msg in splitResponses)
                    {
                        msg.Replace("|", "");
                        //Debug.Log("Trimmed:" + msg);
                        if (!String.IsNullOrEmpty(msg))
                        {

                            NetworkMessage deparse = new NetworkMessage();
                            try
                            {
                                deparse = JsonUtility.FromJson<NetworkMessage>(msg);
                            }
                            catch (Exception e)
                            {
                               
                                //Bad JSON message, dump string to file for review                               
                                // Write the string to a file.
                                System.IO.StreamWriter file = new System.IO.StreamWriter("./ErrorDump/Error.txt");
                                file.WriteLine(response);
                                file.WriteLine("/n");
                                file.WriteLine(msg);
                                file.WriteLine("/n");
                               file.WriteLine( splitResponses.Length);
                               foreach (string msg2 in splitResponses)
                               {
                                   file.WriteLine("/n");
                                   file.WriteLine(msg2);
                               }
                               file.Close();
                                Debug.LogError("TEXT DUMP CREATED");
                            }

                            m_MessagesRecieved.Add(deparse);
                        }
                    }

                }
                // Signal that all bytes have been received.
                receiveDone.Set();
                //}
            }
            catch (Exception e)
            {
               Debug.LogError(e.ToString());
            }
            m_Locked = false;
        }
	}
	
	private  void Send(Socket client, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallback), client);
	}
	
	private  void SendCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			//Debug.LogError("Sent {0} bytes to server."
			         // + bytesSent);
			
			// Signal that all bytes have been sent.
			sendDone.Set();
		} catch (Exception e) {
			Debug.LogError(e.ToString());
		}
	}

    public void SendStack(List<String> messages)
    {
        while (messages.Count > 0)
        {
            Send(client, messages[0]);
            messages.RemoveAt(0);
        }
    }
	
	public void ShutDown()
	{
		client.Close ();
	}

}
