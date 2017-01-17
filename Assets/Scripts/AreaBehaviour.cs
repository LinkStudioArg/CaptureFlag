using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class AreaBehaviour : NetworkBehaviour {
	public int areaID;

	public ItemSpawner itemSpawner;
	void Start(){
		itemSpawner =  FindObjectOfType<ItemSpawner> ();
	}
	void OnTriggerEnter(Collider col){
		Debug.Log ("Triggered " + col.gameObject.name);
		if (!isServer)
			return;
		if (col.gameObject.tag == "Player") {
			
			PlayerBehaviour playerB = col.gameObject.GetComponent<PlayerBehaviour> ();
			if (!playerB.areaSet) {
				playerB.areaSet = true;
				playerB.areaID = areaID;
			}
		} else if (col.gameObject.tag == "Item") {
			ItemBehaviour item = col.gameObject.GetComponent<ItemBehaviour> ();
			if (item.hasOwner) {
				Debug.Log ("hasOwner" + item.hasOwner);
				if (item.owner.GetComponent<PlayerBehaviour>().areaID == areaID) {
					Debug.Log ("shouldDestroy" );
					NetworkServer.Destroy (col.gameObject);
					itemSpawner.CmdSpawnNewItem ();
				}
			}
		}
	}
}
