using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;         
    public TankManager[] m_Tanks;
	public PickupManager m_Pickups;


    private int m_RoundNumber;              //current round number
    private WaitForSeconds m_StartWait;     //for coroutine waits
    private WaitForSeconds m_EndWait;       
	private TankManager m_RoundWinner;      //winner of round (of type tankmanager, referes to specific tanks)
	private TankManager m_GameWinner;       //winner of game (of type tankmanager)


    private void Start()
    {
		//sets up waitforseconds
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

		//coroutine, stgarts the gameloop coroutine (each round round in gameloop)
		//coroutine waits while round is being played, then round plays
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
		//loops through tank manager, sets the instance in 
		//that tank manager to the instantiated object, sets player number, runs setup (in tank manager)
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
		//get the number of tanks for array of transforms
        Transform[] targets = new Transform[m_Tanks.Length];

		//loop through targets, set each target equal to the tank manager's instance transofm
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

		//sets the camera control's targets to the tank targets
        m_CameraControl.m_Targets = targets;
    }

	//coroutines have return type IEnumerator
    private IEnumerator GameLoop()
    {
		//yield causes stop execution of that function, waits for condition (or time)
		//and comes back again and starts again when it left off
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
			//reloading current level
            SceneManager.LoadScene(0);
        }
        else
        {
			//no yeild return, function will finish, and there will be another game loop running
            StartCoroutine(GameLoop());
        }
    }

	//reset all tanks
	//disable all tank controls
	//set camera pos and size
	//increment round number
	//set message UI
    private IEnumerator RoundStarting()
    {
		ResetAllTanks ();

		DisableTankControl ();

		m_CameraControl.SetStartPositionAndSize ();

		m_RoundNumber++;

		m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }

	//enable tank control
	//Empty message UI, don't want to see "round n" all the time
	//wait for one tank left
    private IEnumerator RoundPlaying()
    {
		EnableTankControl ();

		m_MessageText.text = string.Empty;

		while (!OneTankLeft ()) {

			//check pickup spawn
			m_Pickups.CheckSpawn();

			yield return null; //with null, just means come back the next frame
		}

		//if execution makes it here, roundending is next

    }

	//disable all tank contorls
	//clear exsisted round winner
	//check for game winner
	//calculate message UI
    private IEnumerator RoundEnding()
    {

		DisableTankControl ();

		m_RoundWinner = null; //tank manager, not an int or something

		m_RoundWinner = GetRoundWinner ();  //finds tank that's active and returns it

		if (m_RoundWinner != null) {
			m_RoundWinner.m_Wins++;  //links to tank manager (where wins is held)
		}

		m_GameWinner = GetGameWinner ();

		string message = EndMessage (); //get the message depending on above stuff

		m_MessageText.text = message;

        yield return m_EndWait; //sets the end wait
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf) //is this tank active?
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin) //is the number of wins that this tank has equal to the number of rounds needed to win the game
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!"; //default condition

        if (m_RoundWinner != null)  //if theres a round winner
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)//these messages are for all tanks after every round
        {
			if (m_Tanks [i].m_Wins == 1) {
				message += m_Tanks [i].m_ColoredPlayerText + ": " + m_Tanks [i].m_Wins + " WIN\n";

			} else {
				message += m_Tanks [i].m_ColoredPlayerText + ": " + m_Tanks [i].m_Wins + " WINS\n";
			}
        }

        if (m_GameWinner != null) //is this tank the game winner? message is not added, this is all that is shown
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}