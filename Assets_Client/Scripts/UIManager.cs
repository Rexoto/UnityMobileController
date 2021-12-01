using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    LiteNetClient client;
    NetPacketProcessor PacketProcessor = new NetPacketProcessor();

    public GameObject WaitForServer;
    public GameObject StartScreen;
    public GameObject flappy;
    public GameObject racing;
    public GameObject bomb;
    public Button bombBackground;
    public GameObject GameSelection;
    public GameObject Retry;

    public Text playerName;
    public Color color;
    public BetterToggleGroup colors;

    private void OnEnable()
    {
        client = GetComponent<LiteNetClient>();
        LiteNetClient.OnReceiveEvent += OnNetworkReceive;
        LiteNetClient.OnPeerConnectedEvent += OnPeerConnected;
        LiteNetClient.OnPeerDisconnectedEvent += OnPeerDisconnected;
        PacketProcessor.SubscribeReusable<UIPacket, NetPeer>(OnUIPacketPacketReceived);
        PacketProcessor.SubscribeReusable<OrientationPacket, NetPeer>(OnOrientationPacketReceived);
        colors.OnChange += ColorOnChange;
    }

    private void OnOrientationPacketReceived(OrientationPacket packet, NetPeer peer)
    {
        if (packet.name == "Landscape")
        {
            LockOrientation(ScreenOrientation.Landscape);
        }
        else if (packet.name == "Portrait")
        {
            LockOrientation(ScreenOrientation.Portrait);
        }
    }

    public void LockOrientation(ScreenOrientation orientation)
    {
        Screen.orientation = orientation;
    }

    private void ColorOnChange(Toggle newActive)
    {
        color = colors.ActiveToggles().First().GetComponent<Image>().color;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void OnDestroy()
    {
        LiteNetClient.OnReceiveEvent -= OnNetworkReceive;
        LiteNetClient.OnPeerConnectedEvent -= OnPeerConnected;
        LiteNetClient.OnPeerDisconnectedEvent -= OnPeerDisconnected;
    }

    public void OnPeerConnected(NetPeer peer)
    {
        WaitForServer.SetActive(false);
    }

    public void OnPeerDisconnected(NetPeer peer)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        PacketProcessor.ReadAllPackets(reader, peer);
    }

    public void OnUIPacketPacketReceived(UIPacket uiPacket, NetPeer peer)
    {
        if (uiPacket.name == "selection")
        {
            GameSelection.SetActive(uiPacket.enabled);
        }
        if (uiPacket.name == "retry")
        {
            Retry.SetActive(uiPacket.enabled);
        }
        if (uiPacket.name == "flappy")
        {
            flappy.transform.GetChild(0).gameObject.SetActive(false);
            flappy.transform.GetChild(1).gameObject.SetActive(true);
            flappy.SetActive(uiPacket.enabled);
        }
        if (uiPacket.name == "racing")
        {
            racing.SetActive(uiPacket.enabled);
            LockOrientation(ScreenOrientation.Landscape);
        }
        if (uiPacket.name == "flappybegin")
        {
            flappy.transform.GetChild(0).gameObject.SetActive(true);
            flappy.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (uiPacket.name == "bombreceive")
        {
            bomb.SetActive(uiPacket.enabled);
            Handheld.Vibrate();
        }
        if (uiPacket.name == "bombexplode")
        {
            bombBackground.interactable = false;
            ColorBlock red = bombBackground.colors;
            red.disabledColor = Color.red;
            bombBackground.colors = red;
        }
    }

    public void SendButton(string Name)
    {
        ButtonPacket bp = new ButtonPacket
        {
            name = Name
        };
        client.SendPacketToServer(bp);
    }

    public void SendPlayer()
    {
        PlayerPacket pp = new PlayerPacket
        {
            name = playerName.text,
            r = color.r,
            g = color.g,
            b = color.b
        };
        client.SendPacketToServer(pp);
        StartScreen.SetActive(false);
    }

    public void DisableGameObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    public void SendScene(string scene)
    {
        ScenePacket sp = new ScenePacket
        {
            name = scene
        };
        client.SendPacketToServer(sp);
    }

    public void LockLandscape()
    {
        OrientationPacket op = new OrientationPacket
        {
            name = "Landscape"
        };
        client.SendPacketToServer(op);
    }

    public void LockPortait()
    {
        OrientationPacket op = new OrientationPacket
        {
            name = "Portrait"
        };
        client.SendPacketToServer(op);
    }
}
