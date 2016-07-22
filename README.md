# UnitySocketsProject
A small scale multi-player space shooter in Unity. It uses the Winsock API to create a TCP connection between two players. 

To play please open the "build" folder.
The IP addresses are set to the local machine by default but can be changed in the relevant text file.

Be sure to launch the Server application before launching the client application.

Use WASD to move and the mouse to shoot!

#Project Details
This project implements a multi-player game using Winsock for C#
It creates an Asynchronous TCP connection between the Server application and the Client Application. Network messages are serialized into
JSON before being sent using Unity's inbuilt JSON serializer.
