using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Remove public methods
public abstract class Character : MonoBehaviour {
	
	//The body giving the Character physics
	private Rigidbody2D rb;
	//The animator giving the Character animation
	protected Animator anim;
	//The Character's Movement speed
	[SerializeField]
	private float moveSpeed = 100f;
	//The Character's movement direction and aimAngle
	private Vector2 direction = new Vector2();
	//The character's spell list
	[SerializeField]
	private GameObject[] spellPrefabList;
	
	//Determines if Character is casting 
	private bool isCasting = false;
	//Determines if Character is casting an instant cast action
	private bool isInstantCasting = false;
	//Character casting frames
	private float frame = 0f;
	//Exit points for spells
	[SerializeField]
	private Transform[] exitPoints;
	//Locks direction during attack
	private bool directionLocked = false;
	
	//Attach NetMove Script
	protected NetMove netMove;
	
	//////////Base Methods Begin//////////
	//Initialization
	protected virtual void Start () {
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		//Initialize NetMove script
		netMove = GetComponent<NetMove>();
	}
	
	//Update
	protected virtual void Update () {

		//Flips between animation layers
		HandleLayers();
		
		//Cast Frame Count
		frame+=Time.deltaTime;
	}
	
	//Update with Time.deltaTime
	protected virtual void FixedUpdate () {}
	//////////Base Methods End//////////
	
	
	//////////Movement Begin//////////
	//Move the Character
	protected void Move(float dirX, float dirY) {
		direction.x = dirX; 
		direction.y = dirY;
		rb.AddForce(direction.normalized * moveSpeed);
	}

	//Determine if Character is moving
	private bool IsMoving {
		get {
			return direction.x != 0 || direction.y != 0;
		}
	}
	
	//Aim and face direction determination, networkplayer should never call this, while enemy ai can set direction to be the same as direction they are walking in
	protected void SetAim(Vector2 target, ref int directionMod) {
		if (!directionLocked) {
			//AimAngle Setup
			Vector3 target3d = new Vector3(target.x, target.y, 0);
			Vector3 aim = target3d - transform.position;
			float aimAngle = Mathf.Atan2(aim.y,aim.x) * Mathf.Rad2Deg + 180;
			
			//DirectionMod Setup
			//Left
			directionMod = 3; 
			//Down
			if(aimAngle >= 45 && aimAngle < 135)
				directionMod = 2; 
			//Right
			else if(aimAngle >= 135 && aimAngle < 225)
				directionMod = 1; 
			//Up
			else if(aimAngle >= 225 && aimAngle < 315)
				directionMod = 0; 
		}
	}
	
	//Aim for spell specificallly
	private int SetAimForSpell(Vector2 target) {
		//AimAngle Setup
		Vector3 target3d = new Vector3(target.x, target.y, 0);
		Vector3 aim = target3d - transform.position;
		float aimAngle = Mathf.Atan2(aim.y,aim.x) * Mathf.Rad2Deg + 180;
		
		//DirectionMod Setup
		//Left
		int directionMod = 3; 
		//Down
		if(aimAngle >= 45 && aimAngle < 135)
			directionMod = 2; 
		//Right
		else if(aimAngle >= 135 && aimAngle < 225)
			directionMod = 1; 
		//Up
		else if(aimAngle >= 225 && aimAngle < 315)
			directionMod = 0; 
		return directionMod;
	}
	//////////Movement End//////////
	
	
	//////////Animation Begin//////////
	//Controls Animation Layers
	private void HandleLayers() {
		//Update weight from Idle to Walk Animation Layer if Character is moving
		if (isInstantCasting) {
			ActivateLayer("Instant Layer");
		}
		else if (IsMoving) {
			ActivateLayer("Walk Layer");
		}
		else if (isCasting) {
			ActivateLayer("Cast Layer");
		}
		else {
			ActivateLayer("Idle Layer");
		}
	}
	
	//Method for use in HandleLayers
	private void ActivateLayer (string layerName) {
		//Deactivate all layers
		for (int i = 0; i < anim.layerCount; i++) {
			anim.SetLayerWeight(i,0);
		}
		//Activate desired layer
		anim.SetLayerWeight(anim.GetLayerIndex(layerName),1);
	}
	
	//Face direction animation
	public void FaceDirection(int netDirectionMod) {
		//Left
		if(netDirectionMod == 3) {
			anim.SetFloat("x", -1);	
			anim.SetFloat("y", 0);
		}
		//Down
		else if(netDirectionMod == 2) {
			anim.SetFloat("x", 0);	
			anim.SetFloat("y", -1);
		}
		//Right
		else if(netDirectionMod == 1) {
			anim.SetFloat("x", 1);	
			anim.SetFloat("y", 0);
		}
		//Up
		else if(netDirectionMod == 0) {
			anim.SetFloat("x", 0);	
			anim.SetFloat("y", 1);
		}
	}
	//////////Animation End//////////
	
	
	//////////Attack Begin//////////
	//Attack Trigger
	public bool AttackTrigger(int spellNumber, Vector2 targetPosition) {
		//TODO: While buff/item, Spellspeed = Spellspeed * characterAttackSpeed modifier, characterAttackSpeed modifier would have to get loaded when Network Player gets loaded
		//TODO: The characterAttackSpeed will need 3 or more values (melee, ranged, spell) to allow different weapons to apply a different modifier
		Spell spell = spellPrefabList[spellNumber].GetComponent<Spell>();
		//TODO: This has to be characterAttackSpeed to allow re-keybinding, but right now it can't be because if faster than melee attack speed it will overwrite animation
		Spell melee = spellPrefabList[0].GetComponent<Spell>();
		
		//Instant
		if (!spell.CastTime){
			//Disallow casting if last cast was too recent
			if (!isInstantCasting && frame >= melee.SpellSpeed) {
				StartCoroutine(InstantAttack(spellNumber, targetPosition, spell.SpellSpeed, spell.SpellLock));
				return true;
			}
		}
		//Cast
		else {
			//Disallow casting if already casting or moving
			if (!isCasting && !IsMoving && !isInstantCasting) {
				StartCoroutine(CastTimeAttack(spellNumber, targetPosition, spell.SpellSpeed, spell.SpellLock));
				return true;
			}
		}
		return false;
	}
	
	//TODO: Turn Instant Attack to channeling attack, may need to lock position based on variable
	//Instant Attack
	private IEnumerator InstantAttack(int spellNumber, Vector2 targetPosition, float attackSpeed, bool spellLock) {
		Debug.Log("Attack begin, isInstantCasting: " + isInstantCasting);
		//Trigger attack animation layer
		isCasting = true; 
		isInstantCasting = true;
		//Yield 2 frames before relocking direction
		//Note this would only be needed in a 2D since in 3D mouse position would never dictate which way the character is facing
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		directionLocked = spellLock;
		//Trigger attack animation within layer
		//TODO: spellNumber may not always be 0
		if (spellNumber == 0) 
			anim.SetBool("attack", isCasting); 
		else
			anim.SetBool("cast", isCasting); 
		//Reset yield frame
		frame = 0;
		
		//The cast time
		yield return new WaitUntil(() => frame >= attackSpeed);
		
		//Perform attack
		//TODO: Melee attack script needed, cannot wait until after yield though, it should happen immediately
		if (spellNumber == 0) 
			MeleeAttack(spellPrefabList[spellNumber], targetPosition);  
		else
			CastSpellTwo(spellPrefabList[spellNumber], targetPosition);  
		
		//Finish cast
		isCasting = false;
		isInstantCasting = false;
		directionLocked = false;
		anim.SetBool("attack", isCasting);
		anim.SetBool("cast", isCasting);
		Debug.Log("Attack end, isInstantCasting: " + isInstantCasting);
	}
	
	//Cast Time Attack
	private IEnumerator CastTimeAttack(int spellNumber, Vector2 targetPosition, float spellSpeed, bool spellLock) {
		Debug.Log("Cast begin");
		//Trigger attack animation layer
		isCasting = true;
		directionLocked = spellLock;
		//Trigger attack animation within layer
		anim.SetBool("cast", isCasting); 
		//Reset yield frame
		frame = 0;
		
		//The cast time
		yield return new WaitUntil(() => /*lambda function*/ frame >= spellSpeed || IsMoving || isInstantCasting);
		
		//Cast the spell
		if (frame >= spellSpeed) CastSpell(spellPrefabList[spellNumber], targetPosition);
		
		//Finish casting properly unless interrupted by instant cast
		if (!isInstantCasting){
			//Finish cast
			isCasting = false;
			anim.SetBool("cast", isCasting);
			Debug.Log("Cast end, isCasting:" + isCasting);
		}
		directionLocked = false;
	}
	
	
	//Melee attack - this is in test
	private void MeleeAttack(GameObject spellPrefab, Vector2 targetPosition){
		int directionMod = SetAimForSpell(targetPosition);
		//Get exit point position
		Vector2 exitPointPosition = new Vector2(exitPoints[directionMod].position.x, exitPoints[directionMod].position.y);
		//Debug.Log("Melee attack position: " + tempVector1);
		//Get offset position (using character scale)
		Vector2 offset = exitPoints[directionMod].GetComponent<CircleCollider2D>().offset;
		Vector2 scaledOffset = new Vector2(exitPoints[directionMod].transform.parent.parent.localScale.x * offset.x, exitPoints[directionMod].parent.parent.localScale.y * offset.y);
		//Debug.Log("Melee attack offset: " + tempVector3);
		//Add exit point position and offset
		Vector2 tempVector4 = exitPointPosition + scaledOffset;
		//Debug.Log("Final attack locaiton: " + tempVector4);
		
		//TODO: I may be able to write this method in Spell and just call the method
		Collider2D[] colliders = Physics2D.OverlapCircleAll(tempVector4, 0.27f, 0);
		foreach (Collider2D c in colliders) {
			Debug.Log("OverlapCircle: " + c.name);
		}
	}
	
	//Cast spell
	//TODO: I think I can merge this with CastSpell and just check if spell is directionMod or targetPosition or not, still going to need seperate space for meleeAttack
	private void CastSpellTwo(GameObject spellPrefab, Vector2 targetPosition){
		int directionMod = SetAimForSpell(targetPosition);
		//Get exit point position
		Vector2 exitPointPosition = new Vector2(exitPoints[directionMod].position.x, exitPoints[directionMod].position.y);
		//Debug.Log("Melee attack position: " + tempVector1);
		//Get offset position (using character scale)
		Vector2 offset = exitPoints[directionMod].GetComponent<CircleCollider2D>().offset;
		Vector2 scaledOffset = new Vector2(exitPoints[directionMod].transform.parent.parent.localScale.x * offset.x, exitPoints[directionMod].parent.parent.localScale.y * offset.y);
		//Debug.Log("Melee attack offset: " + tempVector3);
		//Add exit point position and offset
		Vector2 tempVector4 = exitPointPosition + scaledOffset;
		//Debug.Log("Final attack locaiton: " + tempVector4);

		
		var spellObject = Instantiate(spellPrefab, tempVector4, Quaternion.identity, transform);
		spellObject.GetComponent<Spell>().TargetPosition = targetPosition;
	}
	
	
	//Cast spell method overload
	private void CastSpell(GameObject spellPrefab, Vector2 targetPosition){
		//I have directionMod, but I am recalculating because otherwise I have to send another variable across network
		//Left - 3/Down - 2/Right - 1/Up - 0
		int directionMod = SetAimForSpell(targetPosition);
		var spellObject = Instantiate(spellPrefab,exitPoints[directionMod].position, Quaternion.identity, transform); //Note: Unintended move of object happens when player moves becasue it's instantiated as a child. This is actively ignored.
		
		spellObject.GetComponent<Spell>().TargetPosition = targetPosition;
	}
	//////////Attack End//////////
	

}
