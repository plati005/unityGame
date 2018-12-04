using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;
using UnitySocketIO;

public class NetMove : MonoBehaviour {
	
	//Link this script to the Network
	public SocketIOController socket;
	
	//Move the Character for other people
	public void SendMove(float x, float y) {
		Debug.Log("sending position to node" + Network.DirectionsToJson(x,y));
		//Send position to server
		socket.Emit("move", Network.DirectionsToJson(x,y));
	}
	

}
