// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour 
{
	public GameManager manager;
	//public FakeServerInputs server;
	public string message;
	string use;
	string pass;
	//Queue sendQueue;
	
	static TcpClient client;
	NetworkStream stream;
	//const String serverIP = "127.0.0.1";
	
	const String serverIP = "128.195.11.143";
	
	
	
	private Thread clientThread;
	private int numPlayers;
	bool isConnect;
	bool sendData;
	bool getData;
	
	void OnApplicationQuit()
	{
		clientThread.Abort();
		client.Close();
		isConnect = false;
	}
	
	// Use this for initialization
	void Start () 
	{
		manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		//server = GameObject.Find ("FakeServer").GetComponent<FakeServerInputs>();
		message = "";
		isConnect = false;
		sendData = false;
		getData = false;
		use = "";
		pass = "";
		numPlayers = 0;
		//sendQueue = new Queue();
	}
	
	//if anything needs the IP address
	public String GetIP()
	{
		return serverIP;
	}
	
	//sends move request to "server" from gameManager
	public void requestMove(string inputMove)
	{
		//server.getMessage(inputMove);
		
		if(manager.start)
		{
			//sends the movement change command to server
		 	// Translate the passed message into ASCII and store it as a Byte array.
			//print ("sending message to server");
			//print ("inputMove = " + inputMove);
			message = inputMove;
			//print ("message = " + message);	
    		sendMessage(message);
		}
	}
	
	//gets move data from server and sends it to gameManager
	public void doMove(string newMove)
	{
		//print ("doing move");
		//sends velocity change comand to gameManager
		if(manager.start)
		{
			manager.move = true;
			lock(manager.serverCommand)
			{
				manager.serverCommand.Enqueue(newMove);
			}
		}
	}
	
	public void Connect(String server, string userName, string password) 
	{
		print ("in Connect");
		bool testing = false;
  		try 
  		{
			testing = true;
			
    		// Create a TcpClient.
    		Int32 port = 4300;
    		client = new TcpClient();
			
			
			//sends pre-encrypted username and password to server.
			//if the Username and password do not match, the server will send a disconect command back
			print ("UserName: " + userName);
			print ("Password: " + password);
			use = userName;
			pass = password;
			
			// use = "";
			// pass = "";
		
			//stream = client.GetStream();
			
			try
			{ // start try to connect
				client.Connect(server, port);
				
				if(! client.Connected)
				{
					print ("Connection failed!");	
				}
				
				stream = client.GetStream();
				
				isConnect = true;
		    	clientThread = new Thread(new ThreadStart(serverIO));
				clientThread.Start();
				print ("clientThread should have started by now");
			} // end try to connect
		
			catch(System.Exception e)
			{
				print("Exception e" + e.Message );
			}
			
			
			
			//here, add all the "connect" stuff, and "manager.start"
			
			
			//after connecting to server:	
			manager.start = true;
			
			print (" manager start " + manager.start);
  		} 
  		catch (ArgumentNullException e) 
  		{
    		print("ArgumentNullException: {0}"+ e.Message);
  		} 
  		catch (SocketException e) 
  		{
    		print("SocketException: {0}" + e.Message);
  		}
		
		if (testing == false){
			print ("try never gets entered");
		}
	}
	
	public void Disconnect()
	{
		print("Running Disconnect");
		//moved here from LoginBox
		manager.start = false;
		//stream.Close();
		client.Close ();
		clientThread.Abort();
		isConnect = false;
	}
	
	public void serverIO()
	{
		print ("in ServerIO");
		
		//should get hello$ from server
		//
		getMessage ();
		
		//sends login info
		message = "1$" + use + "$" + Encryptor.encryptString("elephant") + "$" + pass + "$";
		//print ("message " + message);
		sendMessage(message);

		
		//should get 0$, 1$, or 3$
		getMessage();
		
		//if the message was 1$ or 3$, a series of 4$ messages will follow
		
		
		while(isConnect)
		{
			//print ("ServerIO: in whileLoop");		
			//reads a message
			getMessage();
		}
		
		print ("end of server IO");
	}
	
	public void sendMessage(string theMessage)
	{

		if(theMessage.Length >0)
		{

			Byte[] data = System.Text.Encoding.ASCII.GetBytes(theMessage);
   			// Send the message to the connected TcpServer. 
		   	stream.Write(data, 0, data.Length);
			stream.Flush();
			print ("in SendMessage, sent " + theMessage);
		}
	}
	
	public void getMessage()
	{
		//locks serverCommand queue for thread safety		
		//print("in GetMessage");
		Byte[] data = new Byte[256];
    	// String to store the response ASCII representation.
   		String responseData = String.Empty;
		// Read the first batch of the TcpServer response bytes.
    	Int32 bytes = stream.Read(data, 0, data.Length);
    	responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
		if(responseData != String.Empty){
    			print("Received: " + responseData);	
				lock(manager.serverCommand)
				{
					manager.serverCommand.Enqueue(responseData);
				}
		}
	}
}
