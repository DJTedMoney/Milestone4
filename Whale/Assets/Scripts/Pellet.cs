// Andrew Franowicz 29297832
// Jason Heckard  84851006
// Nathan Stengel 28874701

using UnityEngine;
using System.Collections;

public class Pellet : MonoBehaviour {
	public Transform pelletMesh;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void setPos(int newX, int newY)
	{
		transform.position = new Vector2(newX, newY);
	}
}
