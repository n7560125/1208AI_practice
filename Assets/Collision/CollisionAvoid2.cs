using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 繼承上一步，增加判斷物件面對方向障礙物添加
/// </summary>
public class CollisionAvoid2  {

    static public bool CheckObstacleInRange(AIData data)
    {
        List<Obstacle> m_AvoidTargets = Main.m_Instance.GetObstacles();
        Transform ct = data.m_Go.transform;
        Vector3 cPos = ct.position;
        Vector3 cForward = ct.forward;
        Vector3 vec;
        float fDist = 0.0f;
        float fDot = 0.0f;
        int iCount = m_AvoidTargets.Count;
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

            //用障礙物距離單位向量與物件方向做內積，判斷是否為正(方向近似)，若為負則劃分outside;
            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            
            if(fDot < 0)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }
            m_AvoidTargets[i].m_eState = Obstacle.eState.INSIDE_TEST;
        }

        return true;
    }
}
