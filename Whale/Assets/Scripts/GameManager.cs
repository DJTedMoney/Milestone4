// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	int MAX_PLAYERS = 2;
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
		players = new Player[MAX_PLAYERS];
		isActive = new bool[MAX_PLAYERS];
		for(int i = 0; i<MAX_PLAYERS; i++)
		{
			//print ("test " + i);
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
		for(int i = 0; i<MAX_PLAYERS; i++)
		{
			players[i].setActive(isActive[i]);
		}
			sendMove();
			applyMove();
	}
	
	public void sendMove()
	{
		//print ("in SendMove");
		
		//starts command with new direction (U = up, D = down
		//L = left, and R = right)
		send = false;
		//print (start);
		if(start && isActive[clientNumber]){
			if(Input.GetKeyDown(KeyCode.UpArrow) )
			{
				command = "U$"; 
				send = true;
				print ("Pressed UP");
			}
			
			else if(Input.GetKeyDown(KeyCode.DownArrow) )
			{
				command = "D$";
				send = true;
				print ("Pressed DOWN");
			}
			
			else if(Input.GetKeyDown(KeyCode.LeftArrow) )
			{
				command = "L$";
				send = true;
				print ("Pressed LEFT");
			}
			
			else if(Input.GetKeyDown(KeyCode.RightArrow) )
			{
				command = "R$";
				send = true;
	     		print ("Pressed RIGHT");
			}
	
			
			//finishes the command with player data (Position x and y, speed, and size)
			if(send == true)
			{
				command = "2$"+ command + clientNumber.ToString()+"$";
				//print (command);
				activeClient.requestMove(command);
				send = false;
			}
		}
	}		
	void applyMove()
	{	//print ("in applyMove");
		//loads the next server command and reads the first command
		//The first command is the command type (0 = disconect, 1 = connect, 2 = move)
		int serverSize;
		lock(serverCommand){
			serverSize = serverCommand.Count;
		}
		if(serverSize != 0)
		{
				//print ("serverCommand is not = 0");
			string tempCommand;
			lock(serverCommand){
				tempCommand = serverCommand.Dequeue().ToString();
				serverSize--;
			}
			string comType = tempCommand.Substring(0,tempCommand.IndexOf(delim));
			tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				
			//Server Disconectd Client due to wrong username/Passowrd
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
				tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				// fourth element of command is starting y 
				int startY = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
				tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				
				players[clientNumber].transform.position = new Vector3(startX, startY, 0);
				players[clientNumber].setNumber(clientNumber);
				players[clientNumber].setSpeed(1);
			}
			//Server sent Move commands to client
			if(comType.Equals("2")) //&& move == true)
			{
				int tempNum = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
				tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				int tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
				tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				int tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
				players[tempNum].setDirection(tempX,tempY);
				tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				
				tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
				tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
				players[tempNum].transform.position = new Vector3(tempX, tempY, 0);
				tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					
						//sets player speed
						//players[i].setSpeed(int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim))));
						//tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);					
						//sets player size
						//players[i].size = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
						//tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					//}
				//}
				//sets pellet position
				/*for(int j = 0; j < 4; j++)
				{
					int tempX = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					int tempY = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					pellets[j].setPos(tempX, tempY);
				}*/
				//move = false;
			}
				//writes the server command to the gui
				//new player connetion success
				else if(comType.Equals("3"))
				{
					print ("comtype is 3");
					guiBox.grafxText.text = "Connected\nWelcome " + guiBox.userName;
					clientNumber = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);// third element of command is starting x pos
					int startX = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					// fourth element of command is starting y 
					int startY = int.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand = tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				
					players[clientNumber].transform.position = new Vector3(startX, startY, 0);
					isActive[clientNumber] = true;
					players[clientNumber].setSpeed(1);
					players[clientNumber].setNumber(clientNumber);
					print ("got to end of comtype 3");
				}
				//player[temp] connected
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
				
					string tempName = tempCommand.Substring(0,tempCommand.IndexOf (delim));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[playerNum].setName(tempName);
				}
				//player[temp] disconnected
				else if(comType.Equals("5"))
				{
					int temp = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					isActive[temp] = false;
				}
				//wall collision or new player info
				else if(comType.Equals("6"))
				{
					int tempNum = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					
					int tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					int tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					players[tempNum].transform.position = new Vector3(tempX, tempY, 0);
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				
					tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setDirection(tempX, tempY);
				
					int tempSize  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setSize(tempSize);
				
					int tempScore  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setScore(tempScore);
				
					int tempSpeed = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setSpeed(tempSpeed);
				
					if(tempNum != clientNumber)
					{
						if(players[tempNum].size > players[clientNumber].size)
						{
							players[tempNum].renderer.material.color = Color.red;
						}//end if
						else
						{
							players[tempNum].renderer.material.color = Color.blue;
						}//end else
					}//end if
				}//end if
			
			else if(comType.Equals("7"))
				{
					int tempNum = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					
					int tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					int tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					players[tempNum].transform.position = new Vector3(tempX, tempY, 0);
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
				
					tempX  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					tempY  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setDirection(tempX, tempY);
				
					int tempSize  = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setSize(tempSize);
				
					int tempScore  =  (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setScore(tempScore);
				
					int tempSpeed = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setSpeed(tempSpeed);
				
					string tempName = tempCommand.Substring(0,tempCommand.IndexOf (delim));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					players[tempNum].setName(tempName);
				}
				//pellets set to new position
				else if(comType.Equals("8"))
				{
					int tempPellet = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					int tempX = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					int tempY = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					pellets[tempPellet].transform.position = new Vector3(tempX, tempY, 0);
				}
				else if(comType.Equals("9"))
				{
					int tempNum = (int)float.Parse(tempCommand.Substring(0,tempCommand.IndexOf(delim)));
					tempCommand= tempCommand.Substring(tempCommand.IndexOf(delim)+1);
					guiBox.grafxText.text = players[tempNum] + " has won/nwith a Score of: " + players[tempNum].score.ToString();
					for(int i = 0; i < MAX_PLAYERS; i++)
					{
						isActive[i] = false;
					}
				}
				else if(comType.Equals("hello"))
				{
					print("Client got the Hello!");
				}
		}//ends whileLoop
	}
}
