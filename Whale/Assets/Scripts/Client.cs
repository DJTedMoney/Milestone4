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
			isConnect = true;
		    clientThread = new Thread(new ThreadStart(serverIO));
			clientThread.Start();
			print ("clientThread should have started by now");
			
			use = "";
			pass = "";
		
			//stream = client.GetStream();
			
			try
			{ // start try to connect
				client.Connect(server, port);
				
				if(! client.Connected)
				{
					print ("Connection failed!");	
				}
				
				stream = client.GetStream();
				
			} // end try to connect
		
			catch(System.Exception e)
			{
				print("Exception e" + e.ToString() );
			}
			
			//here, add all the "connect" stuff, and "manager.start"
			
			
			//after connecting to server:	
			manager.start = true;
  		} 
  		catch (ArgumentNullException e) 
  		{
    		Console.WriteLine("ArgumentNullException: {0}", e);
  		} 
  		catch (SocketException e) 
  		{
    		Console.WriteLine("SocketException: {0}", e);
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
		

	}
	
	public void serverIO()
	{
		print ("in ServerIO");
		if(manager.start && isConnect)
		{
			stream = client.GetStream();
			message = "1$" + use + "$" + Encryptor.encryptString("elephant") + "$" + pass + "$";
			sendMessage(stream);
			isConnect = false;
		}
		while(manager.start)
		{			
			if(sendData)
			{	
    			sendMessage(stream);
				sendData = false;
			}
			
			//reads a message
			getMessage(stream);
		}
	}
	
	public void sendMessage(NetworkStream theStream)
	{
		print ("in SendMessage");
		if(message.Length >0)
		{
			Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
   			// Send the message to the connected TcpServer. 
		   	theStream.Write(data, 0, data.Length);
			theStream.Flush();
   			Console.WriteLine("Sent: " + message);
		}
	}
	
	public void getMessage(NetworkStream theStream)
	{
		print("in GetMessage");
		Byte[] data = new Byte[256];
    	// String to store the response ASCII representation.
   		String responseData = String.Empty;
		// Read the first batch of the TcpServer response bytes.
    	Int32 bytes = theStream.Read(data, 0, data.Length);
    	responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
    	Console.WriteLine("Received: ", responseData);
		manager.serverCommand.Enqueue(responseData);
	}
}
