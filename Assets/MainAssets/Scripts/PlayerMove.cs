using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Character {
	
	//Direction of Player
	private float dirX, dirY;
	
	//Attach NetMove Script
	private NetMove netMove;
	
	//Health related values
	//TODO: Move health and mana out
	public Stat health;
	public float iniHealth=100;
	
	//Mana related values
	//TODO: Move health and mana out
	public Stat mana;
	public float iniMana=50;
	
	//Initialization
	protected override void Start() {
		
		//Initialize NetMove script
		netMove = GetComponent<NetMove>();
		
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
		dirX = Input.GetAxisRaw("Horizontal");
		dirY = Input.GetAxisRaw("Vertical");
		//Debug.Log("dirX is:"+dirX+", dirY is:"+dirY);
		
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
		//IF statement exists for performance
		if (IsMovingTrigger) {
			Move(dirX, dirY);
			netMove.SendMove(dirX, dirY);
		}
		//Else needed for animation to revert to Idle
		else {
			Move(0,0);
		}
		
	}
	
	//Determine if Character was triggered to move via Input specifically
	private bool IsMovingTrigger {
		get {
			return dirX != 0 || dirY != 0;
		}
	}


}
