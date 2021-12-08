using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAvoid4
{

    static public bool CheckObstacleInRange(AIData data)
    {
        List<Obstacle> m_AvoidTargets = Main.m_Instance.GetObstacles();
        Transform ct = data.m_Go.transform;
        Vector3 cPos = ct.position;
        Vector3 cForward = ct.forward;
        Vector3 vec;
        float fFinalDotDist;
        float fFinalProjDist;
        Vector3 vFinalVec;
        Obstacle oFinal = null;
        float fDist = 0.0f;
        float fDot = 0.0f;
        int iCount = m_AvoidTargets.Count;
        float fMinDist = 10000.0f;
        for (int i = 0; i < iCount; i++)
        {
            //m_AvoidTargets[i]
            vec = m_AvoidTargets[i].transform.position - cPos;
            vec.y = 0.0f;
            fDist = vec.magnitude;
            if (fDist > data.m_fProbeLength + m_AvoidTargets[i].m_fRadius)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }

            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            if (fDot < 0)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }
            m_AvoidTargets[i].m_eState = Obstacle.eState.INSIDE_TEST;
            float fProjDist = fDist * fDot;
            float fDotDist = Mathf.Sqrt(fDist * fDist - fProjDist * fProjDist);
            if (fDotDist > m_AvoidTargets[i].m_fRadius + data.m_fRadius)
            {
                continue;
            }
            //算出距離obj最近的障礙物(威脅最大)
            if(fDist < fMinDist)
            {
                fMinDist = fDist;
                fFinalDotDist = fDotDist;
                fFinalProjDist = fProjDist;
                vFinalVec = vec;
                oFinal = m_AvoidTargets[i];
            } 
           // m_AvoidTargets[i].m_eState = Obstacle.eState.COL_TEST;
        }
        //若obj的probe內目前無障礙物，回傳false.
        if(oFinal == null)
        {
            return false;
        }
        else
        {
            oFinal.m_eState = Obstacle.eState.COL_TEST;
        }
        return true;
    }
}