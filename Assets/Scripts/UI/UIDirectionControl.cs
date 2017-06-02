using UnityEngine;

public class UIDirectionControl : MonoBehaviour
{
    public bool m_UseRelativeRotation = true;  


    private Quaternion m_RelativeRotation;     


    private void Start()
    {
		//find local rotation of canvas
        m_RelativeRotation = transform.parent.localRotation;
    }


    private void Update()
    {
		//use that rotation all the time, this is so the bar doesn't rotate when you rotate the tank
        if (m_UseRelativeRotation)
            transform.rotation = m_RelativeRotation;
    }
}
