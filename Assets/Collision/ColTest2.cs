using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColTest2 : MonoBehaviour
{

    public AIData m_Data;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CollisionAvoid2.CheckObstacleInRange(m_Data);
    }

    private void OnDrawGizmos()
    {
        if (m_Data != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fRadius);
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * this.m_Data.m_fProbeLength);
            Gizmos.color = Color.yellow;
            Vector3 vLastTemp = -this.transform.right;
            for (int i = 1; i < 180; i++)
            {
                Vector3 vTemp = Quaternion.Euler(0.0f, 1.0f * i, 0.0f) * -this.transform.right;
                Vector3 vStart = this.transform.position + vLastTemp * m_Data.m_fProbeLength;
                Vector3 vEnd = this.transform.position + vTemp * m_Data.m_fProbeLength;
                vLastTemp = vTemp;
                Gizmos.DrawLine(vStart, vEnd);
            }

        }
        


    }
}
