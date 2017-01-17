using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemBehaviour : NetworkBehaviour {
	[SyncVar]
	public bool hasOwner = false;
	[SyncVar]
	public GameObject owner;



	void Update()
	{

			if (hasOwner && owner != null) {			
				Vector3 Pos = new Vector3(owner.transform.position.x, 4f, owner.transform.position.z);
			transform.position = Vector3.Slerp (transform.position, Pos, 100f * Time.deltaTime);
			}

			
	}

	void OnTriggerEnter(Collider col){
		if (!isServer)
			return;
		
		if (col.gameObject.tag == "Player" && !hasOwner) {
			hasOwner = true;
			owner = col.gameObject;
		}
	}


}
