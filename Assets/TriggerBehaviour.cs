using UnityEngine;
using System.Collections;

public class TriggerBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnTriggerEnter(Collider other) {
		//Debug.Log ("Collided with " + other.name);
		/* Ignore collision with other quads */
		if (!other.name.StartsWith("QuadTemplate"))
			gameObject.rigidbody.isKinematic = true;

	}
}
