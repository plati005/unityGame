using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Character {
	
	//Direction of Player
	private float dirX, dirY;
	
	//Link PlayerMove to Network script
	//private NetMove netMove;
	public GameObject network;
	
	//Health related values
	public Stat health;
	public float iniHealth=100;
	
	//Mana related values
	public Stat mana;
	public float iniMana=50;
	
	//Initialization
	protected override void Start() {
		
		//Connect to NetMove script
		//netMove = GetComponent<NetMove>();
		
		
		//Health and Mana initialization
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
		//TODO: Check performance if IF statement is removed
		if (IsMovingTrigger) {
			Move(dirX, dirY);
			//var script = network.GetComponent<NetworkTwo>();
			NetworkTwo.SendMove(dirX, dirY);
		}
		//Else needed for animation to revert to Idle
		else {
			Move(0,0);
		}
		
	}
	
	//Determine if Character was triggered to move
	private bool IsMovingTrigger {
		get {
			return dirX != 0 || dirY != 0;
		}
	}


}
