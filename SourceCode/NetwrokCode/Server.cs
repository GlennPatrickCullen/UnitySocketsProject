using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


//Server network manager
//Implements an asynchronous TCP connection that sends and recieves JSON messages
//These messages are read from the OutgoingStack object and serialized before being sent over the network.
//All recieved messages are deserialized and placed on the IncomingStack object. They are then communicated to the application in the Update loop using the cached network managers.
//NOTE: Currently port 5500 and 5501 need to be available for this project to work
public class Server : MonoBehaviour {

	AsynchronousSocketListener m_Server;
    IncomingStack m_Incoming;
	OutgoingStack m_OutgoingMessages;

    // cached network managers
    NetworkDestroyer m_NetworkDestroyer;
    NetworkControler m_NetworkControler;
	// Use this for initialization
	void Start () {
		m_Server = new AsynchronousSocketListener ();
		m_Server.StartListening ();
		m_OutgoingMessages = gameObject.GetComponent<OutgoingStack> ();
        m_Incoming = gameObject.GetComponent<IncomingStack>();

      
        m_NetworkDestroyer = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkDestroyer>();
        m_NetworkControler = GameObject.FindGameObjectWithTag("Player").GetComponent<NetworkControler>();
	}


    // Update is called once per frame
    // Cycles through all messages generated this frame and sends them
    //Reads all messages ecieved this frame and sends them to relevant game objects
	void Update () {
		m_Server.ListenTick ();
		if (m_Server.client != null) {
            m_Server.SendStack (m_OutgoingMessages.m_Outgoing);
		}

        m_Server.ReceiveStack(m_Server.client, m_Incoming.m_Incoming);
      
       while (m_Server.m_MessagesRecieved.Count > 0)
       {
           if (m_Server.m_MessagesRecieved[0].m_MesssageType == MessageType.Destroy)
           {
               m_NetworkDestroyer.DestroyObject(m_Server.m_MessagesRecieved[0]);
   
           }
           else if (m_Server.m_MessagesRecieved[0].m_MesssageType == MessageType.ScoreUpdate)
           {
               BroadcastMessage("SetTheirScore", m_Server.m_MessagesRecieved[0].m_Score);
              
           }
           else
           {
               m_NetworkControler.ParseNetworkMessage(m_Server.m_MessagesRecieved[0]);
            }

           m_Server.m_MessagesRecieved.RemoveAt(0);
       }
      

	}

	void OnApplicationQuit()
	{
		m_Server.ShutDown();
	}
}


public class AsynchronousSocketListener {

	// Thread signal.
	public static ManualResetEvent allDone = new ManualResetEvent(false);
	Socket listener;
	public Socket client  = null ;
    // The response from the remote device.
	private static String response = String.Empty;
    private static ManualResetEvent receiveDone =
    new ManualResetEvent(false);

    public List<NetworkMessage> m_MessagesRecieved = new List<NetworkMessage>();

	public AsynchronousSocketListener() {
	}
	
	public  void StartListening() {
		// Data buffer for incoming data.
		byte[] bytes = new Byte[1024];
		
		// Establish the local endpoint for the socket.

        string ip = System.IO.File.ReadAllText("./ServerIP.txt");
        IPAddress ipAddress = IPAddress.Parse(ip);
		IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5500);
		
		// Create a TCP/IP socket.
		listener = new Socket(AddressFamily.InterNetwork,
		                             SocketType.Stream, ProtocolType.Tcp );
		
		// Bind the socket to the local endpoint and listen for incoming connections.
		try {
			listener.Bind(localEndPoint);
			listener.Listen(100);
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}

		
	}
	public void  ListenTick()
	{
		try {
		
				// Set the event to nonsignaled state.
				allDone.Reset();
				
				// Start an asynchronous socket to listen for connections.
				//Debug.Log("Waiting for a connection...");
				listener.BeginAccept( 
				                     new AsyncCallback(AcceptCallback),
				                     listener );
				
				// Wait until a connection is made before continuing.
				allDone.WaitOne(0);
			}
		 catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}
	public  void AcceptCallback(IAsyncResult ar) {
		try{
		// Signal the main thread to continue.
		allDone.Set();
		
		// Get the socket that handles the client request.
		//Socket listener = (Socket) ar.AsyncState;
		//Socket handler = listener.EndAccept(ar);
			client = listener.EndAccept(ar);
			//client = handler;
		// Create the state object.
		StateObject state = new StateObject();
		state.workSocket = client;
		client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                     new AsyncCallback(ReadCallback), state);
		}
		catch(Exception e)
		{
			Debug.Log (e.Message);
		}
	}
	
	public  void ReadCallback(IAsyncResult ar) {
		String content = String.Empty;
		
		// Retrieve the state object and the handler socket
		// from the asynchronous state object.
		StateObject state = (StateObject) ar.AsyncState;
		Socket handler = state.workSocket;
		
		// Read data from the client socket. 
		int bytesRead = handler.EndReceive(ar);
		
		if (bytesRead > 0) {
			// There  might be more data, so store the data received so far.
			state.sb.Append(Encoding.ASCII.GetString(
				state.buffer,0,bytesRead));
			
			// Check for end-of-file tag. If it is not there, read 
			// more data.
			content = state.sb.ToString();
			if (content.IndexOf("<EOF>") > -1) {
				Send(handler, content);
			} else {
				// Not all data received. Get more.
				handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
				                     new AsyncCallback(ReadCallback), state);
			}
		}
	}

	public void SendStack(List<String> messages)
	{
		while (messages.Count>0) {
			Send(client,messages[0]);
			messages.RemoveAt(0);
		}
	}
	
	private  void Send(Socket handler, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
       // Debug.Log("Sent" + data);
		// Begin sending the data to the remote device.
		//while
		handler.BeginSend(byteData, 0, byteData.Length, 0,
		                  new AsyncCallback(SendCallback), handler);
	}
	
	private  void SendCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket handler = (Socket) ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = handler.EndSend(ar);
			//Debug.Log("Sent {0} bytes to client."+ bytesSent);
			
			//handler.Shutdown(SocketShutdown.Both);
			//handler.Close();
			
		} catch (Exception e) {
			Debug.Log(e.ToString());
		}
	}

	public void ShutDown()
	{
		listener.Close ();
		client.Close ();
	}


    public void ReceiveStack(Socket client, List<string> income)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;
            state.incomingStack = income;
            // Begin receiving the data from the remote device.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                                new AsyncCallback(ReceiveStackCallback), state);
        }
        catch (Exception e)
        {
           // Debug.LogError(e.ToString());
        }
    }

    private void ReceiveStackCallback(IAsyncResult ar)
    {
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
                //Debug.Log("I recieved" + response);

                //Debug.Log(response);
                String[] splitResponses = response.Split('|');
                foreach (string msg in splitResponses)
                {
                    msg.Replace("|", "z");
                   // Debug.Log("Trimmed:" + msg);
                    if (!String.IsNullOrEmpty(msg))
                    {

                        NetworkMessage deparse = new NetworkMessage();
                        try
                        {
                            deparse = JsonUtility.FromJson<NetworkMessage>(msg);
                        }
                        catch (Exception e)
                        {
                           // Debug.LogError("JSON MISHAP!!!!! Message was: " + msg);

                            // Write the string to a file.
                            System.IO.StreamWriter file = new System.IO.StreamWriter("./ErrorDump/Error.txt");
                            file.WriteLine(response);
                            file.WriteLine("/n");
                            file.WriteLine(msg);
                            file.WriteLine("/n");
                            file.WriteLine(splitResponses.Length);
                            file.Close();
                           // Debug.LogError("TEXT DUMP CREATED");
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
    }
}
