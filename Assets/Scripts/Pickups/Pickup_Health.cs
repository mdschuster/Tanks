using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Health : MonoBehaviour {

	public LayerMask tankmask;
	private TankHealth health;
	private float healthAmount=20;

	void OnTriggerEnter(Collider other){
		//checking if the layermask and layer are the same
		if(tankmask.value != (tankmask.value | (1<<other.gameObject.layer))) {
			return;
		}
		//grab the tank health component of the tank that ran over it
		health = other.GetComponent<TankHealth> ();
		//check if that tank already has max health
		if (health.m_StartingHealth == health.getHealth ()) {
			return;
		}

		//take a negative amount of damage
		health.TakeDamage (-healthAmount);
		//if total health goes over max health,, reduce to max health 
		if (health.getHealth() > health.m_StartingHealth) {
			health.TakeDamage (health.getHealth()-health.m_StartingHealth);
		}

		GetComponentInParent<PickupManager> ().Pickedup = true;

		//destroy pickup
		Destroy (gameObject);

	}
}
