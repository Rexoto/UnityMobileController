using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System;

public class LiteNetServer : MonoBehaviour, INetEventListener, INetLogger {

    public string IP = "127.0.0.1";
    public int sendPort = 8889;
    public int receivePort = 8888;
    public int UpdateTime = 15;
    public bool Discovery = true;
    public string Key = "sample_app";

    NetManager Server;
    [System.NonSerialized]
    public NetPeer ServerPeer;
    NetDataWriter DataWriter;
    [System.NonSerialized]
    public NetPacketProcessor PacketProcessor = new NetPacketProcessor();

    public static LiteNetServer instance;

    public delegate void ReceiveAction(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
    public static event ReceiveAction OnReceiveEvent;

    public delegate void PeerConnectedAction(NetPeer peer);
    public static event PeerConnectedAction OnPeerConnectedEvent;

    public delegate void PlayerConnectedAction(Player player);
    public static event PlayerConnectedAction OnPlayerConnectedEvent;

    public List<Player> Players = new List<Player>();

    public NetPeer Host;

    private void Awake()
    {
        if (LiteNetServer.instance)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        NetDebug.Logger = this;
        DataWriter = new NetDataWriter();
        Server = new NetManager(this);
        Server.Start(receivePort);
        Server.DiscoveryEnabled = Discovery;
        Server.UpdateTime = UpdateTime;
        PacketProcessor.SubscribeReusable<PlayerPacket, NetPeer>(OnPlayerPacketReceived);
        PacketProcessor.SubscribeReusable<ScenePacket, NetPeer>(OnScenePacketReceived);
        PacketProcessor.SubscribeReusable<OrientationPacket, NetPeer>(OnOrientationPacketReceived);
    }

    private void OnOrientationPacketReceived(OrientationPacket packet, NetPeer peer)
    {
        SetOrientation(packet);
    }

    public void SetOrientation(OrientationPacket packet)
    {
        SendPacketToAllPeers(packet);
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        Server.PollEvents();
	}

    void OnDestroy()
    {
        NetDebug.Logger = null;
        if (Server != null)
            Server.Stop();
    }

    public virtual void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
        ServerPeer = peer;
        if (OnPeerConnectedEvent != null)
            OnPeerConnectedEvent(peer);
        if (peer.Id == 0)
        {
            Host = peer;
        }
        //Players.Add(peer);
    }

    public void SendPacketToAllPeers<T>(T p, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
    {
        foreach (Player player in Players)
        {
            if (player.peer != null && player.peer.ConnectionState == ConnectionState.Connected)
            {
                player.peer.Send(PacketProcessor.Write(p), method);
            }
            else
            {
                Debug.Log("[SERVER] error Packet not sent. Peer not connected");
            }
        }
    }

    public void SendPacketToPeer<T>(T p, NetPeer peer, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
    {
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            peer.Send(PacketProcessor.Write(p), method);
        }
        else
        {
            Debug.Log("[SERVER] error Packet not sent. Peer not connected");
        }
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[SERVER] error " + socketErrorCode);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryRequest)
        {
            Debug.Log("[SERVER] Received discovery request. Send discovery response");
            Server.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnConnectionRequest(ConnectionRequest request)
    {
        request.AcceptIfKey(Key);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        if (peer == ServerPeer)
            ServerPeer = null;
        Players.RemoveAt(peer.Id);
        if (peer.Id == 0)
        {
            if (Players.Count > 0)
                Host = Players[0].peer;
            else
                Host = null;
        }
    }

    public virtual void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        try
        {
            PacketProcessor.ReadAllPackets(reader, peer);
        }
        catch
        { }
        if (OnReceiveEvent != null)
            OnReceiveEvent(peer, reader, deliveryMethod);
        //PacketProcessor.ReadAllPackets(reader, peer);
        //string[] messages = reader.GetString().Split('|');
        //int x = messages.ex;
        //int y;
        //Debug.Log(reader.GetString());
    }

    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Debug.LogFormat(str, args);
    }

    public void OnPlayerPacketReceived(PlayerPacket playerPacket, NetPeer peer)
    {
        Players.Add(new Player(peer, playerPacket.name, new Color(playerPacket.r, playerPacket.g, playerPacket.b)));
        if(peer == Host)
            SendPacketToPeer(new UIPacket { name = "selection", enabled = true }, Host);
        if (OnPlayerConnectedEvent != null)
            OnPlayerConnectedEvent(Players[peer.Id]);
    }

    public void OnScenePacketReceived(ScenePacket senePacket, NetPeer peer)
    {
        SceneManager.LoadScene(senePacket.name);
    }

    [System.Serializable]
    public class Player
    {
        public Player(NetPeer peer, string name, Color color)
        {
            this.peer = peer;
            this.name = name;
            this.color = color;
        }

        public NetPeer peer;
        public string name;
        public Color color;
    }
}
