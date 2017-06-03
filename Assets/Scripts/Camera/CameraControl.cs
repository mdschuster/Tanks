using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f;                  
    [HideInInspector] public Transform[] m_Targets; 


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
		//find from desired pos of the camera in the rig's local space
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

		//looping over tanks
        for (int i = 0; i < m_Targets.Length; i++)
        {
			//they must be active
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

			//find target in rig's local space
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

			//vector to target postion from desired postion
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

			//use whichever is bigger, size or abs(pos to target) for y
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

			//for x (full x size is size*aspect)
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
		//add buffer
        size += m_ScreenEdgeBuffer;

		//don't go smaller than the minSize
        size = Mathf.Max(size, m_MinSize);

        return size;
    }

	//for game manager, will set straight, rather than smooth move
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}