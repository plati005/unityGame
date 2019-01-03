using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnitySocketIO;
using UnitySocketIO.Events;
using SimpleJSON;

//Intent of class is to receive from server, not send to server
public class Network : MonoBehaviour {
	
	//TODO: Delete this, it's for testing only
	public GameObject block;
	
	//Json object
	//TODO: I can use one for spawn, move, and disconnect, but for other actions I will need more, I can instantiate locally?
	private JSONObject playerJson;
	
	//Create static socket connection
	static SocketIOController socket;
	
	//Prefab to spawn other players
	public GameObject playerPrefab;
	
	//Reference to the user's player
	public GameObject myPlayer;
	
	//Player list
	private Dictionary<string, GameObject> otherPlayers;
	
	//Network Initialization
	private void Start() {
		socket = GetComponent<SocketIOController>();
		socket.Connect();
		
		//Begin socket listening
		socket.On("register", OnRegister);
		socket.On("spawn", OnSpawned);
		socket.On("moved", OnMoved);
		socket.On("disconnected", OnDisconnected);
		socket.On("requestPosition", OnRequestPosition);
		socket.On("updatingPosition", OnUpdatingPosition);
		socket.On("attack", OnAttack);
		socket.On("face", OnFace);
		
		//Instantiate player list
		otherPlayers = new Dictionary<string, GameObject>();
		
		//TODO: Delete this, it's for testing only
		socket.On("test-event", (SocketIOEvent e) => {
			var purp = block.GetComponent<SpriteRenderer>();
			purp.color = Color.blue;
        });
	}
	
	//Method to confirm connection
	private void OnRegister (SocketIOEvent e)
	{
		Debug.Log ("Succesfully registered player " + e.data);
		//TODO: Delete this, it's for testing only
		var purp = block.GetComponent<SpriteRenderer>();
		purp.color = Color.blue;
	}
	
	//Method to send back current position from this player (and all others) to currently spawning player
	private void OnRequestPosition (SocketIOEvent e)
	{
		//Debug.Log ("Another player requested position " + e.data);
		//Debug.Log ("My current position " + LocationToJson(myPlayer.transform.position));
		//Will update my position on spawning player's screen
		//TODO: Add direction to this
		socket.Emit("updatePosition", LocationToJson(myPlayer.transform.position));
	}
	
	//Method to update position, if player is not spawning, nothing should happen
	private void OnUpdatingPosition (SocketIOEvent e)
	{
		//Convert string to Json (x, y, id)
		playerJson = (JSONObject)JSON.Parse(e.data);
		Debug.Log ("Updating position of another player " + playerJson["x"] + ", " + playerJson["y"] + ", " + playerJson["id"]);

		//Get other player by Id and update their position
		var teleport = otherPlayers[playerJson["id"]].GetComponent<NetworkPlayer>();
		teleport.Teleport(float.Parse(playerJson["x"]), float.Parse(playerJson["y"]));
		//player.transform.position.x = float.Parse(playerJson["x"]);
		//player.transform.position.y = float.Parse(playerJson["y"]);
	}
		
	//Method to spawn another player
	private void OnSpawned (SocketIOEvent e)
	{
		//Convert string to Json
		playerJson = (JSONObject)JSON.Parse(e.data);
		Debug.Log ("Another player spawned " + playerJson["id"]);

		var player = Instantiate (playerPrefab);
		//Add another player to player list
		otherPlayers.Add(playerJson["id"], player);
		Debug.Log("count: " + otherPlayers.Count);
	}
	
	//Method to move NetworkPlayer
	private void OnMoved (SocketIOEvent e)
	{
		//Convert string to Json (x, y, id)
		playerJson = (JSONObject)JSON.Parse(e.data);
		Debug.Log ("Another player moved " + playerJson["x"] + ", " + playerJson["y"] + ", " + playerJson["id"]);

		//Get other player by Id and move them
		var networkMove = otherPlayers[playerJson["id"]].GetComponent<NetworkPlayer>();
		networkMove.SetDirection(float.Parse(playerJson["x"]), float.Parse(playerJson["y"]));
	}
	
	//Method to trigger attack from NetworkPlayer
	private void OnAttack (SocketIOEvent e)
	{
		//Convert string to Json (x, y, id)
		playerJson = (JSONObject)JSON.Parse(e.data);
		Debug.Log ("Another player attacked " + playerJson["spell"] + ", " + playerJson["x"] + ", " + playerJson["y"] + ", " + playerJson["id"]);
		
		
		//Get other player by Id and move them
		var networkAttack = otherPlayers[playerJson["id"]].GetComponent<NetworkPlayer>();
		Vector2 mousePosition = new Vector2(float.Parse(playerJson["x"]), float.Parse(playerJson["y"]));
		Debug.Log ("Another player attacked " + int.Parse(playerJson["spell"]) + ", " + mousePosition);
		networkAttack.AttackTrigger(int.Parse(playerJson["spell"]), mousePosition);
	}
	
		
	//Method to trigger attack from NetworkPlayer
	private void OnFace (SocketIOEvent e)
	{
		//Convert string to Json (x, y, id)
		playerJson = (JSONObject)JSON.Parse(e.data);
		Debug.Log ("Another player faced " + playerJson["dir"] + ", " + playerJson["id"]);
		
		
		//Get other player by Id and move them
		var networkFace = otherPlayers[playerJson["id"]].GetComponent<NetworkPlayer>();
		networkFace.FaceDirection(int.Parse(playerJson["dir"]));
	}
	
	//Method to disconnect NetworkPlayer
	private void OnDisconnected (SocketIOEvent e)
	{
		//Convert string to Json
		playerJson = (JSONObject)JSON.Parse(e.data);
		Debug.Log ("Another player disconnected " + playerJson["id"]);
		
		//Delete other player
		Destroy(otherPlayers[playerJson["id"]]);
		otherPlayers.Remove(playerJson["id"]);
	}
	
	//Method to convert directions to Json format
	public static string DirectionsToJson(float x, float y){
		return string.Format(@"{{""x"":""{0}"", ""y"":""{1}""}}", x, y);
	}
	
	//Method to convert location to Json format
	public static string LocationToJson(Vector2 location){
		return string.Format(@"{{""x"":""{0}"", ""y"":""{1}""}}", location.x, location.y);
	}
	
	//Method to convert location to Json format
	public static string AttackToJson(int spellNumber, Vector2 location){
		return string.Format(@"{{""spell"":""{0}"", ""x"":""{1}"", ""y"":""{2}""}}", spellNumber, location.x, location.y);
	}
	
	//Method to convert int to Json format
	public static string IntToJson(int directionMod){
		return string.Format(@"{{""dir"":""{0}""}}", directionMod);
	}
	
}
