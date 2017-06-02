using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;  //how long it takes to get from min force to max force

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;  //when tank gets enables, the launch force is min
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

		//how fast the charge needs to happen (v=dx/dt)
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
    	//multiple cases
		m_AimSlider.value=m_MinLaunchForce; //default is smallest arrow (or no arrow)

		if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
			//max charge but not fired
			m_CurrentLaunchForce=m_MaxLaunchForce;
			Fire ();                                 //fire when we get to max force

		} else if (Input.GetButtonDown (m_FireButton)) {
			//when button is first pushed down
			m_Fired=false;
			m_CurrentLaunchForce = m_MinLaunchForce;  //set force back to min (just started charging)
			m_ShootingAudio.clip = m_ChargingClip;
			m_ShootingAudio.Play ();                 //play charging sound  interrupted by fireing clip (uses same audio source)

		} else if (Input.GetButton (m_FireButton) && !m_Fired) {
			//holding button down (and not fired)
			m_CurrentLaunchForce+=m_ChargeSpeed*Time.deltaTime;  //update charge force
			m_AimSlider.value=m_CurrentLaunchForce;   //set aim silder

		} else if (Input.GetButtonUp (m_FireButton) && !m_Fired) {
			//let go of button and not yet fired
			Fire();
		}
	}


    private void Fire()
    {
        // Instantiate and launch the shell.
		m_Fired=true;
		Rigidbody shellInstance = (Rigidbody)Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation);

		shellInstance.velocity = m_FireTransform.forward * m_CurrentLaunchForce;

		//play launch sound

		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play ();

		m_CurrentLaunchForce = m_MinLaunchForce;

    }
}