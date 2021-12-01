# UnityMobileController
Control unity games remotely (e.g. Phone as controller) over Wi-Fi.
Uses [LiteNetLib](https://github.com/RevenantX/LiteNetLib) with UDP to send inputs with minimal delay without missing inputs.
NAT Punchthrough means no messing arount with port forwarding and IP settings so its easy for end users to jump straight into the game.
A few basic example games are included to show it in action.

Client_Assets contains all the code for the controller. It is a basic android app that automatically connects when it detects a server.
Client_Server contains all the code for the server and has examples for a lobby and 2 basic multiplayer minigames.

Both are required to work. Inputs, users etc. are all defined in the server so the client can easily be kept seperate and requires minimal editing beyond the UI.

On the server project make sure the menu, flappy and bomb scenes are added to the build scenes. Make sure the IP, Ports and Key are set correctly on the LitNetServer and LiteNetClient respectively.
![Output sample](https://github.com/Rexoto/UnityMobileController/blob/main/Media/ServerSettings.png)

(Example using another Unity Editor and my phone as controllers)
![Output sample](https://github.com/Rexoto/UnityMobileController/blob/main/Media/output.gif)
