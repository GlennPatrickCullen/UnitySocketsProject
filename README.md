# UnitySocketsProject
A small scale multiplayer space shooter in Unity. It uses the Winsock API to create a TCP connection between two players. 

To play please download the builds from this link: https://www.dropbox.com/sh/5qicwiv7yj7452k/AAA1C9-yurQ2tGpkiTz-rYn9a?dl=0.
The IP adresses are set to the local machine by default but can be changed in the relevant text file.

Be sure to launch the Server application before launching the client application.

Use WASD to move and the mouse to shoot!

#Project Details
This project implements a multiplayer game using Winsock for C#
It creates an Asyncronous TCP connection between the Server application and the Client Application. Network messages are serialized using Unity's inbuilt JSON serializer before being sent. The majority of the network code takes place inside the "Server.cs" and "Client.cs" found in the "NetworkCode" folder.

Both server and client .exe can be found at the link above

Copies of the source code can be found in the "SourceCode" folder

All Unity project files and assets can be found in the "ProjectFiles" folder
