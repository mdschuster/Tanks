using UnityEngine;

public class TankMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;         //player number will get changed by manager
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    //audio source, will play either clip below
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;      //change pitch slightly based on this range

	private string m_MovementAxisName;     //string from the input parameters, see input manager (edit->project settings->input)
    private string m_TurnAxisName;         
    private Rigidbody m_Rigidbody;         //reference to tank's ridid body, used to move tank around
    private float m_MovementInputValue;    //store values for input and use where needed
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         //pitch changes around this value


	//callled when scene first starts
    private void Awake()
    {
		//stores referece to the rigidbody that this script is attached to
        m_Rigidbody = GetComponent<Rigidbody>();
    }

	//called when script is turned on: after awake but before any updates
    private void OnEnable ()
    {
		//don't want kinematic to be on when it's on, otherwise it's not affected by forces
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }

	//when script is disabled, this is called
    private void OnDisable ()
    {
		//no forces can affect the tank now!
        m_Rigidbody.isKinematic = true;
    }
		
    private void Start()
    {
		//sets input axis to name+player number
        m_MovementAxisName = "Vertical" + m_PlayerNumber;
        m_TurnAxisName = "Horizontal" + m_PlayerNumber;

		//stores original pitch
        m_OriginalPitch = m_MovementAudio.pitch;
    }

	//runs every frame, not stable
    private void Update()
    {
        // Store the player's input and make sure the audio for the engine is playing.
		m_MovementInputValue = Input.GetAxis (m_MovementAxisName);
		m_TurnInputValue = Input.GetAxis (m_TurnAxisName);

		EngineAudio ();	//manage engine sounds, every frame it plays the correct audio
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.

		//not turning and not moving forward (not holding any key to move or turn)
		if (Mathf.Abs (m_MovementInputValue) < 0.1f && Mathf.Abs (m_TurnInputValue) < 0.1f) {
			//check if the driving clip is playing
			if (m_MovementAudio.clip == m_EngineDriving) {
				m_MovementAudio.clip = m_EngineIdling;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play();
			}
		} else { //driving
			//check if the idling clip is playing
			if (m_MovementAudio.clip == m_EngineIdling) {
				m_MovementAudio.clip = m_EngineDriving;
				m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
				m_MovementAudio.Play();
			}
		}
    }

	//runs at a fixed rate based largly on the physics engine
    private void FixedUpdate()
    {
        // Move and turn the tank.
		Move ();
		Turn ();
    }

	//moves the tank
    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
		//vector movement    forward vector of tacnk  input value  speed   dt
		Vector3 velocity = transform.forward * m_MovementInputValue * m_Speed;
		//m_Rigidbody.MovePosition (m_Rigidbody.position + Velocity*Time.deltaTime); //doesn't work anymore see internet
		m_Rigidbody.velocity = velocity;
    }

	//turn the tank
    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
    }
}