// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	public Transform playerMesh;
	
	public Vector2 direction;
	public int speed;
	public int size;
	public int score;
	int number;
	string myName;
	
	public Client commsClient;
	
	// Use this for initialization
	void Start () 
	{
		commsClient = GameObject.Find("GameClient").GetComponent<Client>();
		size = 40;
		speed = 0;
		score = 0;
		direction = new Vector2(0,1);
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = new Vector2(transform.position.x + direction.x*speed, transform.position.y + direction.y*speed);
		transform.lossyScale.Set(size, size, 1);
	}
	
	public void setPosition(int newX, int newY)
	{
		transform.position = new Vector2(newX, newY);
	}
	
	public void setDirection(int newX, int newY)
	{
		direction = new Vector2(newX, newY);
		//rigidbody.velocity = direction * speed;
	}
	
	public void setSpeed(int newSpeed)
	{
		speed = newSpeed;
		//rigidbody.velocity = direction*speed;
	}
	
	public void setScore(int newScore)
	{
		score = newScore;
	}
	
	public void setName(string newName)
	{
		myName = newName;
	}
	
	public void setNumber(int newNum)
	{
		number = newNum;
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 40+number*75, 40, 20), myName + " : ");
		GUI.Label(new Rect(55, 40+number*75, 40, 20), size.ToString() );
		GUI.Label(new Rect(10, 65+number*75, 40, 20), "Size : ");
		GUI.Label(new Rect(55, 65+number*75, 40, 20), size.ToString() );
		GUI.Label(new Rect(10, 90+number*75, 40, 20), "Speed : ");
		GUI.Label(new Rect(55, 90+number*75, 40, 20), speed.ToString() );
	}
}
