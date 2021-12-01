using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    LiteNetServer server;
    //NetPacketProcessor PacketProcessor = new NetPacketProcessor();

    public Transform PlayerContainer;
    public GameObject PlayerPrefab;

    private void Start()
    {
        server = LiteNetServer.instance;
        LiteNetServer.OnPlayerConnectedEvent += OnPlayerConnected;
        if (server.Players.Count > 0)
        {
            foreach (LiteNetServer.Player p in server.Players)
            {
                AddPlayer(p);
            }
        }
    }

    private void OnPlayerConnected(LiteNetServer.Player player)
    {
        AddPlayer(player);
    }

    public void AddPlayer(LiteNetServer.Player player)
    {
        GameObject playerobj = Instantiate(PlayerPrefab, PlayerContainer);
        playerobj.GetComponent<Image>().color = player.color;
        playerobj.GetComponentInChildren<Text>().text = player.name;
    }

	// Update is called once per frame
	void Update () {
		
	}
}
