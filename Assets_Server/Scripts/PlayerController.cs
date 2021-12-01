using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour {

    CharacterController controller;

    public float Speed = 4;

    public int peerID;
    public Material material;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();		
    }

    public void Move(float x, float y)
    {
        controller.Move(new Vector3(x, 0, y) * Time.deltaTime * Speed);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
