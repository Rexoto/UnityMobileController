using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotator : MonoBehaviour {

    [SerializeField]
    private GameObject _controllableGameObject;
    private Quaternion _rot;
    LiteNetServer server;

    // Use this for initialization
    void Start () {
        server = LiteNetServer.instance;
        server.PacketProcessor.SubscribeReusable<GyroscopePacket>(OnGyroPacketReceived);
    }
	
	// Update is called once per frame
	void Update () {
        _controllableGameObject.transform.localRotation = _rot;
    }

    void OnGyroPacketReceived(GyroscopePacket gp)
    {
        _rot = new Quaternion(gp.x, gp.y, gp.z, gp.w);
    }
}
