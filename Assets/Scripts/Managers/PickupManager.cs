using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {

	public Transform[] spawnLocations;
	public GameObject health;

	private GameObject[] instance;
	private float spawnCooldown=10.0f;
	private float spawnTime;
	private bool pickedup;

	private void Start(){
		pickedup = true;
		spawnTime = spawnCooldown;
		instance = new GameObject[spawnLocations.Length];

	}

	public void CheckSpawn(){

		if (pickedup) {
			spawnTime -= Time.deltaTime;
		}
		if (spawnTime <= 0.0f && pickedup) {
			for (int i = 0; i < spawnLocations.Length; i++) {
				instance [i] = (GameObject)Instantiate (health, spawnLocations [i].position, spawnLocations [i].rotation);
				instance [i].transform.parent = GameObject.FindGameObjectWithTag ("PickupManager").transform;
				spawnTime = spawnCooldown;
				pickedup = false;
			}
		}
		return;
	}

	public bool Pickedup {
		get {
			return pickedup;
		}
		set {
			pickedup = value;
		}
	}
}
