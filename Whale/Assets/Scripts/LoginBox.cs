// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701
using UnityEngine;
using System.Collections;

public class LoginBox : MonoBehaviour 
{
	public string userName;
	private string passWord;
	
	public GUIText grafxText;
	public Client client;
	//public GameManager manager;
	
	public bool showLogin;
	// Use this for initialization
	void Start () 
	{
		//manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		client  = GameObject.Find("GameClient").GetComponent<Client>();
		
		userName = "";
		passWord = "";
		
		
	}
	
	// Update is called once per frame
	
	 void OnGUI()
	{	
		if(!showLogin)
		{
			grafxText.text = "Enter Username and password";
			
			GUI.Label(new Rect(130, 200, 100, 20), "UserName : ");
			
			userName = GUI.TextField(new Rect(200, 200, 200, 20), userName );
			
			GUI.Label(new Rect(130, 220, 100, 20), "PassWord : ");
			
			passWord = GUI.PasswordField(new Rect(200, 220, 200, 20), passWord, "%"[0], 25);
			
			if(GUI.Button (new Rect (250, 170, 100, 20), "Connect") )
			{

				showLogin = !showLogin;
				print ("<From LoginBox>UserName: " +Encryptor.encryptString(userName));
				print ("<From LoginBox>Password: " +Encryptor.encryptString(passWord));
				//manager.start = true;
				//grafxText.text = "Connect";
				//manager.activeClient.Connect( manager.activeClient.GetIP(), Encryptor.encryptString(userName), 
											// Encryptor.encryptString(passWord));
				client.Connect(client.GetIP(),Encryptor.encryptString(userName), Encryptor.encryptString(passWord));
				
				grafxText.text = "";
			}
		}
		
		else
		{
			GUI.Label(new Rect(10, 40, 100, 20), userName);
			
			if(GUI.Button (new Rect(10, 10, 100, 20), "Disconnect") )
			{
				//grafxText.text = "Hello";
				//manager.activeClient.Disconnect();
				
				client.message = "0$"+client.manager.clientNumber+"$";
				client.sendMessage(client.message);
				client.Disconnect();
				showLogin = !showLogin;
				//manager.start = false;
			}
		}
	}
}
