using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Text;
using System;

public class LiteNetClient : MonoBehaviour, INetEventListener
{
    public static LiteNetClient instance;

    public string IP = "127.0.0.1";
    public int sendPort = 8888;
    public int receivePort = 8889;
    public int UpdateTime = 15;
    public bool Discovery = true;
    public string Key = "sample_app";

    public Text text;

    private NetManager Client;
    NetDataWriter DataWriter;
    private readonly NetPacketProcessor PacketProcessor = new NetPacketProcessor();

    public delegate void ReceiveAction(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod);
    public static event ReceiveAction OnReceiveEvent;

    public delegate void PeerConnectedAction(NetPeer peer);
    public static event PeerConnectedAction OnPeerConnectedEvent;

    public delegate void PeerDisconnectedAction(NetPeer peer);
    public static event PeerDisconnectedAction OnPeerDisconnectedEvent;

    // Use this for initialization
    void Start () {
        instance = this;
        Client = new NetManager(this);
        DataWriter = new NetDataWriter();
        Client.Start();
        Client.UpdateTime = UpdateTime;
    }
	
	// Update is called once per frame
	void Update () {
        Client.PollEvents();

        var peer = Client.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {

        }
        else if(Discovery)
        {
            Client.SendDiscoveryRequest(new byte[] { 1 }, sendPort);
        }
    }

    void OnDestroy()
    {
        if (Client != null)
            Client.Stop();
    }

    public void SendPacketToServer<T>(T p) where T : class, new()
    {
        var peer = Client.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            peer.Send(PacketProcessor.Write(p), DeliveryMethod.Sequenced);
        }
    }

    public void SendToServer(string message)
    {
        var peer = Client.FirstPeer;
        if (peer != null && peer.ConnectionState == ConnectionState.Connected)
        {
            DataWriter.Reset();
            DataWriter.Put(message);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
        text.text = "[CLIENT] We connected to " + peer.EndPoint;
        if (OnPeerConnectedEvent != null)
            OnPeerConnectedEvent(peer);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
    {
        Debug.Log("[CLIENT] We received error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        if (OnReceiveEvent != null)
            OnReceiveEvent(peer, reader, deliveryMethod);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        if (messageType == UnconnectedMessageType.DiscoveryResponse && Client.PeersCount == 0)
        {
            Debug.Log("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            Client.Connect(remoteEndPoint, Key);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {

    }

    public void OnConnectionRequest(ConnectionRequest request)
    {

    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
        if (OnPeerDisconnectedEvent != null)
            OnPeerDisconnectedEvent(peer);
    }
}
