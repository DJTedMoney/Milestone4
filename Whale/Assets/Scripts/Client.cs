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
	
	static TcpClient client;
	NetworkStream stream;
	const String serverIP = "127.0.0.1";
	//const String serverIP = "128.195.11.143";
	
	
	
	private Thread clientThread;
	private int numPlayers;
	bool isConnect;
	bool sendData;
	bool getData;
	
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
	}
	
	//if anything needs the IP address
	public String GetIP()
	{
		return serverIP;
	}
	
	
	// Update is called once per frame
	void Update () 
	{
	
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
			message = inputMove;
			sendData = true;
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
			manager.serverCommand.Enqueue(newMove);
			sendData = true;
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
		print ("ServerIO: in first If");
		// stream = client.GetStream();
		print ("1");
		message = "1$" + use + "$" + Encryptor.encryptString("elephant") + "$" + pass + "$";
		print ("message " + message);
		sendMessage(message);
		print ("ServerIO: message sent");
		
		getMessage();
		
		//THIS IS A TEST BLOCK
			print ("ServerIO: in first If");
			// stream = client.GetStream();
			print ("1");
			message = "2$" + "TestMessage$";
			print ("message " + message);
			sendMessage(message);
			print ("ServerIO: message sent");
		//END THE TEST BLOCK
		
		getMessage();
		
		/*
		while(isConnect)
		{
			print ("ServerIO: in whileLoop");		
			if(sendData)
			{	
    			sendMessage();
				sendData = false;
			}
			
			//reads a message
			getMessage();
		}
		*/
		print ("end of server IO");
	}
	
	public void sendMessage(string theMessage)
	{
		print ("in SendMessage " + theMessage);
		if(theMessage.Length >0)
		{
			Byte[] data = System.Text.Encoding.ASCII.GetBytes(theMessage);
   			// Send the message to the connected TcpServer. 
		   	stream.Write(data, 0, data.Length);
			stream.Flush();
   			print("Sent: " + theMessage);
		}
	}
	
	public void getMessage()
	{
		print("in GetMessage");
		Byte[] data = new Byte[256];
    	// String to store the response ASCII representation.
   		String responseData = String.Empty;
		// Read the first batch of the TcpServer response bytes.
    	Int32 bytes = stream.Read(data, 0, data.Length);
    	responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    	print("Received: " + responseData);
		manager.serverCommand.Enqueue(responseData);
	}
}
