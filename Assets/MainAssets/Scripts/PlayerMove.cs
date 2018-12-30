using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Character {
	
	//Direction of Player
	private float dirX, dirY;
	
	//Old direction of Player to be compared against to send over network
	private float oldX=0, oldY=0;
	//Attach NetMove Script
	//private NetMove netMove;
	
	//Health related values
	//TODO: Move health and mana out, make private Serialize field
	public Stat health;
	public float iniHealth=100;
	//Mana related values
	//TODO: Move health and mana out, make private Serialize field
	public Stat mana;
	public float iniMana=50;
	
	//Direction Character is facing, needed as parameter here becasue if direction is locked, value from previous update should be used in next update
	//It's also out here because FaceDirection is what NetworkPlayer will use. It cannot live inside Character. If inside character setaim will never get calle, and value that face direction is dependant on will never get refreshed for network player, but it will live in their update, which is not good.
	private int directionMod=2;
	//Old directionMod of Player to be compared against to send over network
	private int oldDirectionMod=2;
	
	//////////Base Methods Begin//////////
	//Initialization
	protected override void Start() {
		//Initialize NetMove script
		//netMove = GetComponent<NetMove>();
		
		//Health and Mana initialization
		//TODO: Move health and mana out
		health.Initialize(iniHealth,iniHealth);
		mana.Initialize(iniMana,iniMana);
		
		//Include Character Start
		base.Start();
	}
	
	//Update is called once per frame
	protected override void Update () {
		//Get inputs
		SetDirection(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		
	
		//Set up directionMod
		Vector2 mouseLocation = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//mouseLocation.z=0;
		SetAim(mouseLocation, ref directionMod);
		//Debug.Log("directionMod " + directionMod);
		//Get player to face correct direction based on aim
		//TODO: Need second directionMod purely based on direction facing as opposed to using mouse
		FaceDirection(directionMod);
		//Trigger to face direction on network
		if (IsFacingChange) {
			netMove.SendDirectionMod(directionMod);
			oldDirectionMod = directionMod;
		}
		
		//TODO: Fire1 will trigger attack while Fire2 will select target
		//TODO: directionMod will be influenced by direction walking when not attacking
		//InstantAttack
		if (Input.GetButton("Fire1")){
			if (AttackTrigger(0, mouseLocation))
				//Trigger to have player attack on network
				netMove.SendAttackTrigger(0, mouseLocation);
		}
		
		//CastTimeAttack
		if (Input.GetButtonDown("Fire2")){
			//Note: Think of the 1 as "Action Button 1" from WOW
			if (AttackTrigger(1, mouseLocation))
				//Trigger to move player on network
				netMove.SendAttackTrigger(1, mouseLocation);
		}
		
		//TODO: Use These, they will be triggers to Fire1 if they require aiming, otherwise directly cast a spell
		if (Input.GetKeyDown(KeyCode.Alpha1)){
			if(AttackTrigger(2, mouseLocation))
				//Trigger to move player on network
				netMove.SendAttackTrigger(2, mouseLocation);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)){
			if(AttackTrigger(3, mouseLocation))
				//Trigger to move player on network
				netMove.SendAttackTrigger(3, mouseLocation);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)){
			if(AttackTrigger(4, mouseLocation))
				//Trigger to move player on network
				netMove.SendAttackTrigger(4, mouseLocation);
		}
		
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
		//Move my player
		Move(dirX, dirY);
		//Trigger to move player on network
		if (IsMovingChange) {
			netMove.SendMove(dirX, dirY);
			oldX = dirX;
			oldY = dirY;
		}
		
		//Include Character FixedUpdate
		base.FixedUpdate();
	}
	//////////Base Methods End//////////
	
	//Internally set direction (from Input)
	private void SetDirection (float x, float y) {
		dirX = x;
		dirY = y;
		//Debug.Log("dirX is:"+dirX+", dirY is:"+dirY);
	}

	
	//Determine if there is a change in direction that needs to get sent over the network
	private bool IsMovingChange {
		get {
			return dirX != oldX || dirY != oldY;
		}
	}
	
	//Determine if there is a change in facing that needs to get sent over the network
	private bool IsFacingChange {
		get {
			return directionMod != oldDirectionMod;
		}
	}
	
	
	
	
	//combat notes: two attacks (relative to moving)
		//instant   - castable while moving
			//1.hold animation and action for 0.5sec (essentially limited by attack speed)
			//2.remove animation on other action (not limited by attack speed) - changed to just have no animation
		//cast time - not castable while moving
	//combat notes: character faces mouse
		//if mouse is facing moving direction character moves normal
		//if not chracter moves backwards slower
	//combat notes: two attacks (relative to direction)
		//aiming    - goes direction character faces
		//homing    - can use targeting system for these, I won't have real targeting system, just go in general direction then find nearest target, lots of mobile mmos have auto target which is shit, id use it for only certain spells
}
