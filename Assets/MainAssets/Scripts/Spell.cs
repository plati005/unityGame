using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour {
	
	//Spell movement variable
	private Rigidbody2D myRb;
	private Vector2 direction;
	
	//Determines if spell is homing or not
	private bool isHoming = true;
	
	//////////Serialized Variables Begin//////////
	//Spell speed and variables, serialized for ease of tuning
	[SerializeField]
	private float moveSpeed;
	
	//Determines if spell has cast time or is instant cast
	[SerializeField]
	private bool spellCasted;
	public bool CastTime{
		get { return spellCasted; }
		set { spellCasted = value; }
	}
	
	//Spell cast direction locks character
	[SerializeField]
	private bool spellLocked;
	public bool SpellLock{
		get { return spellLocked; }
		set { spellLocked = value; }
	}
	
	//Spell cast speed
	[SerializeField]
	private float spellCastingSpeed;
	public float SpellSpeed{
		get { return spellCastingSpeed; }
		set { spellCastingSpeed = value; }
	}
	//////////Serialized Variables Begin//////////
	
	//Spell Exits for this time
	float spellTimer = 10f;
	
	//Spell Target
	private GameObject target;
	public Vector2 TargetPosition {get; set;}
	private Vector3 targetPosition3d;		
			
			
	//Spell Finisher
	[SerializeField]
	private GameObject finisher;
	
	//Initialization
	private void Start () {
		myRb = GetComponent<Rigidbody2D>();
		Debug.Log("target2: " + target);
		
		//TODO: Find target nearest to mouseLocation
		//TODO: Also this is wrong code for debug only
		target = GameObject.Find("Object");
		
		
		targetPosition3d.x = TargetPosition.x;
		targetPosition3d.y = TargetPosition.y;
		
		//Initialize direction based on whether spell is homing or not
		if (isHoming) {
			direction = target.transform.position - transform.position;
			myRb.velocity = ((targetPosition3d - transform.position).normalized * moveSpeed/3);
		}
		else {
			direction = targetPosition3d - transform.position;
		}
		
		Invoke("SpellTimeLimit", spellTimer);
	}

	//Spell Movement
	private void FixedUpdate() {
		
		//For Debug only
		//target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		//Continuously adjust direction if spell is homing
		if (isHoming) {
			//TODO: Add 1-2 seconds of spell going forward before anything just to test it
			direction = target.transform.position - transform.position;
			myRb.AddForce(direction.normalized * moveSpeed);
		}
		else {
			myRb.velocity = direction.normalized * moveSpeed;
		}
		//Debug.Log("velocity: " + myRb.velocity);
		
		//Spell Animation Angle
		float angle = Mathf.Atan2(myRb.velocity.y,myRb.velocity.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
	
	//Spell Destroy
	private void OnTriggerEnter2D(Collider2D other){
		Debug.Log("COLLISION with " + other.name + ", tag: " + other.gameObject.tag);
		//TODO: This will need to get changed to playerId
		if(other.gameObject.tag!="Player"){
			Instantiate(finisher,transform.position, Quaternion.identity);
			Destroy(gameObject);
			other.gameObject.SendMessage("ApplyDamage", 10, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	//Spell Time Limit
	private void SpellTimeLimit(){
		Instantiate(finisher,transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
