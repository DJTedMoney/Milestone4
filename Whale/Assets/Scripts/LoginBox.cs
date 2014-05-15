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
	public GameManager manager;
	
	public bool showLogin;
	private const string ipAdress = "128.195.11.143";
	
	// Use this for initialization
	void Start () 
	{
		manager = GameObject.Find("GameManager").GetComponent<GameManager>();
		
		userName = "";
		passWord = "";
		
	}
	
	// Update is called once per frame
	
	void OnGUI()
	{	
		if(!showLogin)
		{
			GUI.Label(new Rect(130, 200, 100, 20), "UserName : ");
			
			userName = GUI.TextField(new Rect(200, 200, 200, 20), userName );
			
			GUI.Label(new Rect(130, 220, 100, 20), "PassWord : ");
			
			passWord = GUI.PasswordField(new Rect(200, 220, 200, 20), passWord, "%"[0], 25);
			
			if(GUI.Button (new Rect (250, 170, 100, 20), "Connect") )
			{
				manager.start = true;
				showLogin = !showLogin;
				print ("<From LoginBox>UserName: " +Encryptor.encryptString(userName));
			print ("<From LoginBox>Password: " +Encryptor.encryptString(passWord));;
				//grafxText.text = "Connect";
				manager.activeClient.Connect(ipAdress, Encryptor.encryptString(userName), 
											 Encryptor.encryptString(passWord));
				
			}
		}
		
		else
		{
			GUI.Label(new Rect(10, 40, 100, 20), userName);
			
			if(GUI.Button (new Rect(10, 10, 100, 20), "Disconnect") )
			{
				//grafxText.text = "Hello";
				manager.activeClient.Disconnect();
				showLogin = !showLogin;
				manager.start = false;
			}
		}
	}
}
