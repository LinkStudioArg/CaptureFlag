using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ItemSpawner : NetworkBehaviour {
	public GameObject itemPrefab;
	public Vector2 offsetX;
	public Vector2 offsetY;
	// Use this for initialization
	public  void Start()
	{	Debug.Log ("Server");
		CmdSpawnNewItem ();
	}

	[Command]
	public void CmdSpawnNewItem()
	{
		Vector3 spawnPosition = new Vector3 (Random.Range (offsetX.x, offsetX.y), 1f, Random.Range (offsetY.x, offsetY.y));
		GameObject spawnedItem =Instantiate(itemPrefab, spawnPosition, Quaternion.identity) as GameObject;
		NetworkServer.Spawn (spawnedItem);

	}
}
