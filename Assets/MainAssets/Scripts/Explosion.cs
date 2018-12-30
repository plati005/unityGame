using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	// Use this for initialization
	private void Start () {
		Invoke("Die", 1f);
	}
	
	private void Die () {
		Destroy(gameObject);
	}
}
