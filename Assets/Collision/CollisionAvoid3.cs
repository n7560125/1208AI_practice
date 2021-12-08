using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 繼承上一步，增加以probe判斷潛在障礙物
/// </summary>
public class CollisionAvoid3
{

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

            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            if (fDot < 0)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }
            m_AvoidTargets[i].m_eState = Obstacle.eState.INSIDE_TEST;
            //計算fDist投影至fDot方向之長度
            float fProjDist = fDist * fDot;
            //計算障礙物至Probe中心線的垂直距離, fProjDist, fDist, fDotDist形成一直角三角形.
            float fDotDist = Mathf.Sqrt(fDist * fDist - fProjDist * fProjDist);
            //判定是否碰撞(兩圓圓心距離>r1+r2為未碰撞)
            if(fDotDist > m_AvoidTargets[i].m_fRadius + data.m_fRadius)
            {
                continue;
            }
            m_AvoidTargets[i].m_eState = Obstacle.eState.COL_TEST;


        }

        return true;
    }
}