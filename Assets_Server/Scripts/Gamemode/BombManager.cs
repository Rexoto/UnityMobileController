using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib;
using LiteNetLib.Utils;

public class BombManager : MonoBehaviour {

    LiteNetServer server;

    public Text PlayerText;

    int currPlayer = 0;

    public float bombTimer = 0;

    public int timeMin = 10;

    public int timeMax = 60;

    private void OnEnable()
    {
        //LiteNetServer.OnReceiveEvent += OnNetworkReceive;
        //LiteNetServer.OnPeerConnectedEvent += OnPeerConnected;
    }

    // Use this for initialization
    void Start () {
        server = LiteNetServer.instance;
        server.PacketProcessor.SubscribeReusable<ButtonPacket, NetPeer>(OnButtonPacketReceived);
        currPlayer = Random.Range(0, server.Players.Count);
        bombTimer = Random.Range(timeMin, timeMax);
        server.SendPacketToPeer(new UIPacket { name = "bombreceive", enabled = true }, server.Players[currPlayer].peer);
        PlayerText.text = server.Players[currPlayer].name.ToUpper() + " HAS THE BOMB!";
    }
	
	// Update is called once per frame
	void Update () {
        bombTimer -= Time.deltaTime;
        if (bombTimer <= 0)
        {
            server.SendPacketToPeer(new UIPacket { name = "bombexplode", enabled = true }, server.Players[currPlayer].peer);
        }
	}

    public void OnButtonPacketReceived(ButtonPacket buttonPacket, NetPeer peer)
    {
        if (buttonPacket.name == "bombtap")
        {
            Debug.Log(currPlayer);
            server.SendPacketToPeer(new UIPacket { name = "bombreceive", enabled = false }, server.Players[currPlayer].peer);
            List<int> choices = new List<int>();
            for (int i = 0; i < server.Players.Count; i++)
            {
                if (i != currPlayer)
                    choices.Add(i);
            }
            currPlayer = choices[Random.Range(0, choices.Count)];
            server.SendPacketToPeer(new UIPacket { name = "bombreceive", enabled = true }, server.Players[currPlayer].peer);
            PlayerText.text = server.Players[currPlayer].name + " HAS THE BOMB!";
        }
    }
}
