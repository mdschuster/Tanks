using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour {

	public Transform[] spawnLocations;
	public GameObject health;

	private GameObject[] instance;
	private float spawnCooldown=10.0f;
	private float spawnTime;
	private bool[] pickedup;
	private int spawnNumber=2;

	private void Start(){
		spawnTime = spawnCooldown;
		instance = new GameObject[spawnLocations.Length];
		pickedup = new bool[spawnNumber];
		for (int i = 0; i < spawnNumber; i++) {
			pickedup[i] = true;
		}

	}

	public void CheckSpawn(){

		for (int i = 0; i < spawnLocations.Length; i++) {
			if (pickedup[i]) {
				spawnTime -= Time.deltaTime;
			}
			if (spawnTime <= 0.0f && pickedup[i]) {
				instance [i] = (GameObject)Instantiate (health, spawnLocations [i].position, spawnLocations [i].rotation);
				instance [i].transform.parent = GameObject.FindGameObjectWithTag ("PickupManager").transform;
				instance [i].GetComponent<Pickup_Health> ().setID (i);
				spawnTime = spawnCooldown;
				pickedup[i] = false;
			}
		}


		return;
	}

	public void Pickedup(bool temp, int id){
		this.pickedup [id] = temp;
	}

}
