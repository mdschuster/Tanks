using System;
using UnityEngine;

[Serializable]
public class TankManager   //not derived from monobehavior
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;            //shooting and moving scripts need the player number for input 
    [HideInInspector] public string m_ColoredPlayerText;    //store colored text
    [HideInInspector] public GameObject m_Instance;         //storing the instance of the tank
    [HideInInspector] public int m_Wins;                    //number of wins that this tank has


    private TankMovement m_Movement;       //references to tank moving and shooting
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject; //referece to canvas, to turn UI on and off


    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();  //getting tank components
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject; //finding component of canvas and getting game object

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

		//html-like rich text
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

		//getting the meshrenderer of the tank (in child)
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

		//changing all materials of all renderers to the player color
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }

	//disable moving, shooting and tank UI
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

	//enable moving, shooting, and tank UI
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

	//rest to spawn point
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

		//turn everything off, then back on again
        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
