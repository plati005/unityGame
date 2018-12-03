using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;

public class NetMove : MonoBehaviour {
	/*
	public GameObject network;
	private Network netScript;
	
	private void Start () {
		netScript = network.GetComponent<Network>();
	}
	
	//TODO: This script can be moved into Network
	//Move the Character for other people
	public void OnMove(float x, float y) {
		Debug.Log("sending position to node" + DirectionsToJson(x,y));
		//Send position to server
		//netScript.socket.Emit("move", new JSONObject(DirectionsToJson(x,y)));
	}
	
	//method for sending position to sockets
	public string DirectionsToJson(float x, float y){
		return string.Format(@"{{""x"":""{0}"", ""y"":""{1}""}}", x, y);
	}
	*/
}
