using UnityEngine;
using System.Collections;
using UnitySocketIO;
using UnitySocketIO.Events;

public class Network : MonoBehaviour {
	
	//Create static socket connection
	static SocketIOController socket;
	
	//Prefab to spawn other players
	public GameObject playerPrefab;
	
	//Network Initialization
	private void Start() {
		socket = GetComponent<SocketIOController>();
		socket.Connect();

		socket.On("register", OnRegister);
		socket.On("spawn", OnSpawned);
		
	}
	
	//Method to confirm connection
	private void OnRegister (SocketIOEvent e)
	{
		Debug.Log ("Succesfully registered player " + e.data);
		//socket.Emit("move");
	}
		
	//Method to spawn another player
	private void OnSpawned (SocketIOEvent e)
	{
		Debug.Log ("Another player spawned " + e.data);
		Instantiate (playerPrefab);
	}
	
	//Method to convert directions to Json format
	public static string DirectionsToJson(float x, float y){
		return string.Format(@"{{""x"":""{0}"", ""y"":""{1}""}}", x, y);
	}
	
}
