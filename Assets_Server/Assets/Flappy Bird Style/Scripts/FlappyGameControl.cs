using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;

public class FlappyGameControl : MonoBehaviour 
{
    LiteNetServer server;

    public static FlappyGameControl instance;			//A reference to our game control script so we can access it statically.
	public Text scoreText;						//A reference to the UI text component that displays the player's score.
	public GameObject gameOvertext;				//A reference to the object that displays the text which appears when the player dies.

	private int score = 0;						//The player's score.
	public bool gameOver = false;				//Is the game over?
	public float startScrollSpeed = -1.5f;
    public float currScrollSpeed = 0;

    public GameObject Bird;

    public List<Bird> birds = new List<Bird>();

    private int deaths = 0;

    void Start()
    {
        server = LiteNetServer.instance;
        LiteNetServer.OnReceiveEvent += OnNetworkReceive;
        LiteNetServer.OnPeerConnectedEvent += OnPeerConnected;
        server.PacketProcessor.SubscribeReusable<ButtonPacket, NetPeer>(OnButtonPacketPacketReceived);
        server.SendPacketToAllPeers(new UIPacket { name = "flappy", enabled = true });
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        //PacketProcessor.ReadAllPackets(reader, peer);
    }

    public void OnPeerConnected(NetPeer peer)
    {

    }

    public void Begin()
    {
        Debug.Log(server);
        for (int i =  0; i < server.Players.Count; i++)
        {
            birds.Add(Instantiate(Bird).GetComponent<Bird>());
            birds[i].GetComponent<SpriteRenderer>().color = server.Players[i].color;
            birds[i].transform.Translate(-Vector3.right * i);
        }
        currScrollSpeed = startScrollSpeed;
    }

    public void OnButtonPacketPacketReceived(ButtonPacket buttonPacket, NetPeer peer)
    {

        if (buttonPacket.name == "flap")
        {
            birds[peer.Id].Flap();
        }

        if (buttonPacket.name == "begin")
        {
            server.SendPacketToAllPeers(new UIPacket { name = "flappybegin", enabled = true });
            Begin();
        }
        if (buttonPacket.name == "retry")
        {
            //deaths = 0;
            Restart();
        }
    }

    public void Restart()
    {
        LiteNetServer.instance.SendPacketToPeer(new UIPacket { name = "retry", enabled = false }, LiteNetServer.instance.Host);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Awake()
    {
        //If we don't currently have a game control...
        if (instance == null)
            //...set this one to be it...
            instance = this;
        //...otherwise...
        else if (instance != this)
            //...destroy this one because it is a duplicate.
            Destroy(gameObject);
    }

	void Update()
	{
		//If the game is over and the player has pressed some input...
		//if (gameOver && Input.GetMouseButtonDown(0)) 
		//{
		//	//...reload the current scene.
		//	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		//}
	}

	public void BirdScored()
	{
		//The bird can't score if the game is over.
		if (gameOver)	
			return;
		//If the game is not over, increase the score...
		score++;
		//...and adjust the score text.
		scoreText.text = "Score: " + score.ToString();
	}

	public void BirdDied(Bird bird)
	{
        deaths++;
        if (deaths >= birds.Count)
        {
            gameOvertext.SetActive(true);
            LiteNetServer.instance.SendPacketToPeer(new UIPacket { name = "retry", enabled = true }, LiteNetServer.instance.Host);
        }
        //Activate the game over text.
        //gameOvertext.SetActive(true);
        ////Set the game to be over.
        //gameOver = true;
        //SceneManager.LoadScene("menu");
    }
}
