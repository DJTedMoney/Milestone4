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
	bool active;
	
	public Client commsClient;
	
	// Use this for initialization
	void Start () 
	{
		commsClient = GameObject.Find("GameClient").GetComponent<Client>();
		size = 40;
		speed = 0;
		score = 0;
		direction = new Vector2(0,1);
		active = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(active)
		{
			transform.position = new Vector2(transform.position.x + direction.x*speed, transform.position.y + direction.y*speed);
			transform.localScale = new Vector3(size, size, 1);
			if(!transform.renderer.enabled)
			{
				transform.renderer.enabled = true;
			}
		}
		else
		{
			if( transform.renderer.enabled)
			{
				transform.renderer.enabled = false;
			}
		}
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
	public void setSize(int newSize)
	{
		size = newSize;
	}
	
	public void setActive(bool newActive)
	{
		active = newActive;
	}
	
	void OnGUI()
	{
		if(active)
		{
			int x = 0;
			int y = 0;
			if(number == 0)
			{	
				x = 10;
				y = 40;
			}
			else if(number == 1)
			{	
				x = 10;
				y = 115;
			}
			else if(number == 2)
			{	
				x = 400;
				y = 40;
			}
			else if(number == 3)
			{	
				x = 400;
				y = 115;
			}
			GUI.Label(new Rect(x, y, 40, 20), myName + " : ");
			GUI.Label(new Rect(x+45, y, 40, 20), score.ToString() );
			GUI.Label(new Rect(x, y+25, 40, 20), "Size : ");
			GUI.Label(new Rect(x+45, y+25, 40, 20), size.ToString() );
			GUI.Label(new Rect(x, y+50, 40, 20), "Speed : ");
			GUI.Label(new Rect(x+45, y+50, 40, 20), speed.ToString() );
		}
	}
}
