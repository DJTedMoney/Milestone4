// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public Client activeClient;
	public string command;
	public Queue serverCommand;
	public Player[] players;
	public Pellet[] pellets;
	public bool[] isActive;
	char delim = '$';
	public bool move;
	public bool send;
	public bool start;
	LoginBox guiBox;
	public int clientNumber;
	
	// Use this for initialization
	void Start () 
	{
		activeClient = GameObject.Find("GameClient").GetComponent<Client>();
		players = new Player[4];
		isActive = new bool[4];
		for(int i = 0; i<4; i++)
		{
			print ("test " + i);
			players[i] = GameObject.Find ("Player" + i.ToString()).GetComponent<Player>();
			isActive[i]=false;
		}
		guiBox = GameObject.Find("GUI").GetComponent<LoginBox>();
		serverCommand = new Queue();
		command = "";
		start = false;
		move = false;
		send = false;
		pellets = new Pellet[4];
		for(int i = 0; i <4; i++)
		{
			pellets[i] = GameObject.Find ("Pellet" + (i+1).ToString()).GetComponent<Pellet>();
		}
	}
	
	IEnumerator Wait(float x) {
    yield return new WaitForSeconds(x);
    }
	
	// Update is called once per frame
	void Update () 
	{
			sendMove();
			//Wait (0.5f);
			// applyMove();
	}
	
	public void sendMove()
	{
		//print ("in SendMove");
		
		//starts command with new direction (U = up, D = down
		//L = left, and R = right)
		send = true;
		//print (start);
		if(Input.GetKeyDown(KeyCode.UpArrow) )
		{
			command = "U$"; 
			print ("Pressed UP");
		}
		
		else if(Input.GetKeyDown(KeyCode.DownArrow) )
		{
			command = "D$";
			print ("Pressed DOWN");
		}
		
		else if(Input.GetKeyDown(KeyCode.LeftArrow) )
		{
			command = "L$";
			print ("Pressed LEFT");
		}
		
		else if(Input.GetKeyDown(KeyCode.RightArrow) )
		{
			command = "R$";
	     	print ("Pressed RIGHT");
		}
		//default command, means no change
		else
		{
			send = false;
		}
		//print (command);
		
		//finishes the command with player data (Position x and y, speed, and size)
		if(send == true)
		{
			command = "2$"+ command + clientNumber.ToString()+"$";
			//print (command);
			activeClient.requestMove(command);
			send = false;
		}
	}
	
	void applyMove()
	{	//print ("in applyMove");
		//loads the next server command and reads the first command
		//The first command is the command type (0 = disconect, 1 = connect, 2 = move)
		lock(serverCommand){
			while(serverCommand.Count != 0)
			{
				//print ("serverCommand is not = 0");
				string tempCommand = serverCommand.Dequeue().ToString();
				string comType = tempCommand.Substring(0,tempCommand.IndexOf(delim));
				tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				
				//Server Disconectd Client
				if(comType.Equals("0"))
				{
					print ("comtype is 0");
					guiBox.grafxText.text = "error, wrong pasword\ndisconected from server";
					activeClient.Disconnect();
					start = false;
					guiBox.showLogin = !guiBox.showLogin;
				}
				//Server connected client
				else if(comType.Equals("1"))
				{
					print ("comtype is 1");
					guiBox.grafxText.text = "Connected\nWelcome back " + guiBox.userName;
					
					// second element of command is the client number
					clientNumber = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					print ("ClientNumber: " + clientNumber);
					isActive[clientNumber] = true;
					
					// third element of command is starting x pos
					int startX = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					
					// fourth element of command is starting y 
					int startY = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
				}
				//Server sent Move commands to client
				if(comType.Equals("2") && move == true)
				{
					for(int i = 0; i <4; i++)
					{
						if(isActive[i])
						{
							//print ("comtype is 2");
							//sets player position to match server
							int tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
							tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
							int tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
							players[i].transform.position = new Vector2(tempX, tempY);
							tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					
							//sets player direction to match server
							tempX = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
							tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
							tempY = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
							tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
							players[i].setDirection(tempX, tempY);
							//sets player speed
							players[i].setSpeed(int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim))));
							tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					
							//sets player size
							players[i].size = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
							tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
						}
					}
					//sets pellet position
					for(int j = 0; j < 4; j++)
					{
						int tempX = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
						tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
						int tempY = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
						tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
						pellets[j].setPos(tempX, tempY);
					}
					move = false;
				}
				//writes the server command to the gui
				else if(comType.Equals("3"))
				{
					print ("comtype is 3");
					guiBox.grafxText.text = "Connected\nWelcome " + guiBox.userName;
					clientNumber = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					isActive[clientNumber] = true;
				}
				else if(comType.Equals("4"))
				{
					print("comType is 4");
					int playerNum  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					isActive[playerNum] = true;
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					int tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					int tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					players[clientNumber].transform.position = new Vector2(tempX, tempY);
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				}
				else if(comType.Equals("5"))
				{
					int temp = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					isActive[temp] = false;
				}
				else if(comType.Equals("hello"))
				{
					print("Client got the Hello!");
				}
			}//ends whileLoop
		}//ends lock
	}
}
