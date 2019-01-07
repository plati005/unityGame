using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySocketIO.SocketIO;
using UnitySocketIO.Events;
using UnitySocketIO;

public class NetMove : MonoBehaviour {
	
	//Link this script to the Network
	//TODO: Make this private and getcomponent
	public SocketIOController socket;
	
	//Move the Character for other people
	public void SendMove(float x, float y) {
		//Debug.Log("sending position to node: " + Network.DirectionsToJson(x,y));
		//Send position to server
		socket.Emit("move", Network.DirectionsToJson(x,y));
	}
	
	//Face the Character for other people
	public void SendDirectionMod(int directionMod) {
		//Debug.Log("sending face to node: " + directionMod);
		//Send face to server
		socket.Emit("face", Network.IntToJson(directionMod));
	}
	
	//Send attack to other people
	public void SendAttackTrigger(int spellNumber, Vector2 mouseLocation){
		//Debug.Log("sending attack to node: " + Network.AttackToJson(spellNumber, mouseLocation));
		socket.Emit("attack", Network.AttackToJson(spellNumber, mouseLocation));
	}
	
}
