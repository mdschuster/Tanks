using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bobber : MonoBehaviour {

	private float speed = 1;
	private float ypos;
	private int direction = -1;
	private Transform originalTransform;

	// Use this for initialization
	void Start () {
		originalTransform = this.transform;
		ypos = originalTransform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 y = new Vector3(0f,0f,0f);
		//down first
		y.y = this.transform.position.y;
		y.y = y.y + speed * direction * Time.deltaTime;
		this.transform.position = y;
		if (y.y < 1.25f) {
			direction = 1;
		} else if (y.y > 1.75f) {
			direction = -1;
		}
	}
}
