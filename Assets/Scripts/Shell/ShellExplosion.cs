using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;   //all tanks are on layer mask
    public ParticleSystem m_ExplosionParticles;       //explosion
    public AudioSource m_ExplosionAudio;       //audio source       
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
	public float m_MaxLifeTime = 2f;         //shells shouldn't exist forever (in sec)       
    public float m_ExplosionRadius = 5f;     //radius of damage


    private void Start()
    {
		//destroys object after max life time, no mater what else happens
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

		//explodes when it hits collider
		//looks for all colliders in the overlap sphere area with tankmask
		Collider[] colliders=Physics.OverlapSphere(transform.position,m_ExplosionRadius,m_TankMask);

		for (int i = 0; i < colliders.Length; i++) {
			Rigidbody targetRigidbody = colliders [i].GetComponent<Rigidbody> (); //try to find rigid body on collider
			if (!targetRigidbody) {
				continue;
			}

			targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

			TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth> ();

			if (!targetHealth) {
				continue;
			}

			float damage = CalculateDamage (targetRigidbody.position);
			targetHealth.TakeDamage (damage);
		}

		//deal with explosion and audio

		m_ExplosionParticles.transform.parent = null; //unparent explosion, so it still plays when shell gets destroyed
		m_ExplosionParticles.Play();
		m_ExplosionAudio.Play ();
		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration); //destroy only the explosion particles gameobject (not the particles themselves
		Destroy (gameObject);

    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

		Vector3 distVector = targetPosition - transform.position;
		float explosionDistance = distVector.magnitude;

		//relative distance

		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

		return Mathf.Max(0f,relativeDistance * m_MaxDamage); //no negative damage value because of edge case


    }
}