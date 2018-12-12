using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : Character {
	
	//Direction of Player
	private float dirX, dirY;
	
	//Initialization
	protected override void Start() {
		
		//Include Character Start
		base.Start();
	}
	
	//Update is called once per frame
	protected override void Update () {
		
		//Include Character Update
		base.Update();
	}
	
	//Update with Time.deltaTime, overrides Character move, keeping it here for Network movements
	protected override void FixedUpdate () {
		//Move network player
		Move(dirX, dirY);

	}
	
	//Externally set direction (from Network)
	public void SetDirection (float x, float y) {
		dirX = x;
		dirY = y;
	}
	
	//Teleport the Character
	public void Teleport(float dirX, float dirY) {
		transform.position = new Vector3(dirX, dirY, 0); 
	}
}
