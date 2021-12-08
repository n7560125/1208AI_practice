using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Check Distance in range.
/// <summary>
/// 以obj作為圓心，並以probe長度作為半徑畫圓(gizmos)，在圓內者判斷為潛在障礙物(威脅較大);
/// </summary>
public class CollisionAvoid1
{
    static public bool CheckObstacleInRange(AIData data)
    {
        //宣告障礙物list;
        List<Obstacle> m_AvoidTargets = Main.m_Instance.GetObstacles();
        Transform ct = data.m_Go.transform;
        Vector3 cPos = ct.position;
        Vector3 cForward = ct.forward;
        Vector3 vec;
        float fDist = 0.0f;
        int iCount = m_AvoidTargets.Count;
        //對每個障礙物做威脅(潛在)判定
        for(int i = 0; i < iCount; i++)
        {
            //m_AvoidTargets[i]
            vec = m_AvoidTargets[i].transform.position - cPos;
            vec.y = 0.0f;
            fDist = vec.magnitude;
            //距離半徑圓外的障礙物劃分為outside;
            if (fDist > data.m_fProbeLength + m_AvoidTargets[i].m_fRadius)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }
            m_AvoidTargets[i].m_eState = Obstacle.eState.INSIDE_TEST;
        }

        return true;
    }
}
