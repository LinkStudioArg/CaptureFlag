using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : NetworkBehaviour {

	public float speed = 10f;
	public float rotateSpeed = 10f;

	Rigidbody rigid;
	// Use this for initialization
	void Start () {
		if(!isLocalPlayer)
		{
			this.enabled = false;
		}		

		rigid = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		InputCheck ();

	}

	void InputCheck()
	{
		float dir = CrossPlatformInputManager.GetAxis ("Horizontal");
		Debug.Log (dir);
		if ( dir != 0) {
			Move (dir);
		}
	}

	void Move(float direction)
	{
		
		rigid.velocity = transform.forward * speed;

		//rigid.rotation = Quaternion.Euler (0.0f, 0.0f, rigid.velocity.x);

//		rigid.MovePosition (transform.position + transform.forward * speed * Time.fixedDeltaTime);
		rigid.MoveRotation (rigid.rotation * Quaternion.Euler (new Vector3(0,  direction * rotateSpeed * Time.fixedDeltaTime, 0)));
	
//		transform.Translate (Vector3.forward *speed*  Time.fixedDeltaTime);
//		transform.Rotate (transform.up * direction * rotateSpeed * Time.fixedDeltaTime);
	}


}
