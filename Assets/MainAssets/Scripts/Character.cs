using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour {
	
	//The body giving the Character physics
	private Rigidbody2D rb;
	//The animator giving the Character animation
	private Animator anim;
	//The Character's Movement speed
	public float moveSpeed = 100f;
	//The Character's direction
	private Vector2 direction = new Vector2();
	

	
	//Initialization
	protected virtual void Start () {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}
	
	//Update getting used in PlayerMove
	protected virtual void Update () {
		//Flips between animation layers
		HandleLayers();
	}
	
	//Update with Time.deltaTime
	protected virtual void FixedUpdate () {
			Move(direction.x, direction.y);
	}
	
	//Move the Character
	protected void Move(float dirX, float dirY) {
		direction.x = dirX; //TODO: The reset to 0 does not happen since the method is only triggered when numbers are not 0
		direction.y = dirY; //TODO: The reset to 0 does not happen since the method is only triggered when numbers are not 0
		rb.AddForce(direction.normalized * moveSpeed);
		
	}
	
	//Determine if Character is moving
	private bool IsMoving {
		get {
			return direction.x != 0 || direction.y != 0;
		}
	}
	
	//Controls Animation Layers 
	private void HandleLayers() {
		
		//Update weight from Idle to Walk Animation Layer and vice versa based on if Character is moving or not
		if (IsMoving) {
			ActivateLayer("Walk Layer");
			//Debug.Log("x: " + direction.x + ", y: " + direction.y);
			
			//Sets animation parameter so he faces correct direction
			anim.SetFloat("x", direction.x);	
			anim.SetFloat("y", direction.y);
		}
		else {
			ActivateLayer("Idle Layer");
		}
	}
	
	//Method for use in HandleLayers
	//TODO: Ensure I can complete attack animations before doing anything else
	private void ActivateLayer (string layerName) {
		//Deactivate all layers
		for (int i = 0; i < anim.layerCount; i++) {
			anim.SetLayerWeight(i,0);
		}
		//Activate desired layer
		anim.SetLayerWeight(anim.GetLayerIndex(layerName),1);
	}
}
