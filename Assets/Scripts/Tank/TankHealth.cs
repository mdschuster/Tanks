using UnityEngine;
using UnityEngine.UI;

public class TankHealth : MonoBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            

	//called at start of game
    private void Awake()
    {
		//instantiate explosion prefab, get the particle system component, all in one!
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

		//don't make it active (no explosion at the start
        m_ExplosionParticles.gameObject.SetActive(false);
    }

	//when tank gets turned back on, set these (not dead, starting health, refreshing health UI)
    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

	//public because we want shells to do the damage
    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.
		m_CurrentHealth -= amount;

		//refresh UI
		SetHealthUI();

		//if current health <=0 and we're not already dead
		if (m_CurrentHealth <= 0f && !m_Dead) {
			OnDeath ();
		}
	}


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
		m_Slider.value = m_CurrentHealth; //based on current health

		//Lerp goes from 0->1, this is why there's a division
		m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);

    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
		//this is when the tank first dies
		m_Dead = true;

		//play all effects
		//move explosion to position of tank
		m_ExplosionParticles.transform.position = transform.position;
		m_ExplosionParticles.gameObject.SetActive (true); //set explosion active
		m_ExplosionParticles.Play ();  //play explosion
		m_ExplosionAudio.Play (); //play explosion audio

		//turn tank off
		gameObject.SetActive (false);
    }
}