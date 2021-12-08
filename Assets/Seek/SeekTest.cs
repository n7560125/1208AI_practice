using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekTest : MonoBehaviour
{
    public GameObject m_Target;
    public AIData m_Data;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Data.m_vTarget = m_Target.transform.position;
        SteeringBehavior.Seek(m_Data);
      //  SteeringBehavior.CollisionAvoid(m_Data);
        SteeringBehavior.Move(m_Data);
    }

    private void OnDrawGizmos()
    {
        if (m_Data != null)
        {
            if(m_Data.m_fMoveForce > 0.0f)
            {
                Gizmos.color = Color.blue;
            } else
            {
                Gizmos.color = Color.red;
            }
            
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * m_Data.m_fMoveForce * 2.0f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward*2.0f);

            Gizmos.color = Color.gray;
            Gizmos.DrawLine(this.transform.position, m_Data.m_vTarget);
        }




    }
}
