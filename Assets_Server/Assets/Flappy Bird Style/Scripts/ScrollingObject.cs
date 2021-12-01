using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingObject : MonoBehaviour 
{
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () 
	{
		//Get and store a reference to the Rigidbody2D attached to this GameObject.
		rb2d = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
        // If the game is over, stop scrolling.
        if (FlappyGameControl.instance.gameOver == true)
        {
            rb2d.velocity = Vector2.zero;
        }
        else
        {
            rb2d.velocity = new Vector2(FlappyGameControl.instance.currScrollSpeed, 0);
        }
	}
}
