using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Character {

	private float dirX, dirY;
	
	//Health related values
	//TODO: Move into Character
	public Stat health;
	public float iniHealth=100;
	
	//Mana related values
	//TODO: Move into Character
	public Stat mana;
	public float iniMana=50;
	
	//Initialization
	protected override void Start() {
		
		//Health and Mana initialization
		//TODO: Move into Character
		health.Initialize(iniHealth,iniHealth);
		mana.Initialize(iniMana,iniMana);
		
		//Include Character Start
		base.Start();
	}
	
	//Update is called once per frame
	protected override void Update () {
		//Get inputs
		dirX = Input.GetAxisRaw("Horizontal");
		dirY = Input.GetAxisRaw("Vertical");
		
		//Include Character Update
		base.Update();
		
		//THIS IS FOR DEBUGGING ONLY!
		////
		if (Input.GetKeyDown(KeyCode.I)) {
			health.MyCurrentValue -= 10;
			mana.MyCurrentValue -= 10;
		}
		if (Input.GetKeyDown(KeyCode.O)) {
			health.MyCurrentValue += 10;
			mana.MyCurrentValue += 10;
		}
		////
	}
	
	//Update with Time.deltaTime, overrides Character move, keeping it here for Network movements
	protected override void FixedUpdate () {
		
		//Here should be my trigger
		//if (IsMovingTrigger) {
			Move(dirX, dirY);
				
			//send pos to node
			//Debug.Log("sending position to node" + Network.VectorToJson(position));
			//socket.Emit("move", new JSONObject(Network.VectorToJson(position)));
		//}
		
	}
	
	//TODO: Delete this
	//Determine if Character was triggered to move
	private bool IsMovingTrigger {
		get {
			return dirX != 0 || dirY != 0;
		}
	}

	

	//TODO: To be used
	/*
	
	//network move
	public void OnMove(Vector2 position){
		//send pos to node
		Debug.Log("sending position to node" + Network.VectorToJson(position));
		//socket.Emit("move", new JSONObject(Network.VectorToJson(position)));
	}
	
	//method for sending position to sockets
	string VectorToJson(Vector2 vector){
		return string.Format(@"{{""x"":""{0}"", ""y"":""{1}""}}", vector.x, vector.y);
	}
	*/
}
