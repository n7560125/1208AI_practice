using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehavior
{
    static public void aaa(float a)
    {
        a = 1.0f;
    }
    static public void Move(AIData data)
    {
        //確認obj是否已到終點
        if (data.m_bMove == false)
        {
            return;
        }
        //宣告變數
        Transform t = data.m_Go.transform;
        Vector3 cPos = data.m_Go.transform.position;
        Vector3 vR = t.right;
        Vector3 vOriF = t.forward;
        Vector3 vF = data.m_vCurrentVector;
        if(data.m_fTempTurnForce > data.m_fMaxRot)
        {
            data.m_fTempTurnForce = data.m_fMaxRot;
        } else if(data.m_fTempTurnForce < -data.m_fMaxRot)
        {
            data.m_fTempTurnForce = -data.m_fMaxRot;
        }
        //先算轉向力量, 將obj面對方向更新.
        vF = vF + vR * data.m_fTempTurnForce;
        vF.Normalize();
        t.forward = vF;
        
        //依照time.deltaTime為速度做更新.
        data.m_Speed = data.m_Speed + data.m_fMoveForce * Time.deltaTime;
        if(data.m_Speed < 0.01f)
        {
            data.m_Speed = 0.01f;
        } else if(data.m_Speed > data.m_fMaxSpeed)
        {
            data.m_Speed = data.m_fMaxSpeed;
        }
        //判斷碰撞
        if (data.m_bCol == false)
        {
            Debug.Log("CheckCollision");
            if (SteeringBehavior.CheckCollision(data))
            {
                Debug.Log("CheckCollision true");
                t.forward = vOriF;
            }
            else
            {
                Debug.Log("CheckCollision true");
            }
        } else
        {
            //若obj速度接近0(因前面有障礙物而接近停止), 則給予其大的轉向力道轉彎.
            if (data.m_Speed < 0.02f)
            {
                if(data.m_fTempTurnForce > 0)
                {
                    t.forward = vR;
                } else
                {
                    t.forward = -vR;
                }
                
            }
        }
        //計算面對方向移動.
        cPos = cPos + t.forward * data.m_Speed;
        t.position = cPos;
    }

    static public bool CheckCollision(AIData data)
    {
        List<Obstacle> m_AvoidTargets = Main.m_Instance.GetObstacles();
        if(m_AvoidTargets == null)
        {
            return false;
        }
        Transform ct = data.m_Go.transform;
        Vector3 cPos = ct.position;
        Vector3 cForward = ct.forward;
        Vector3 vec;

        float fDist = 0.0f;
        float fDot = 0.0f;
        int iCount = m_AvoidTargets.Count;
        for (int i = 0; i < iCount; i++)
        {
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

            return true;
        }
        return false;
    }

    /// <summary>
    /// 碰撞迴避
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    static public bool CollisionAvoid(AIData data)
    {
        //宣告list並存入所有obstacles的資料.
        List<Obstacle> m_AvoidTargets = Main.m_Instance.GetObstacles();
        Transform ct = data.m_Go.transform;
        //宣告obj的position向量.
        Vector3 cPos = ct.position;
        //宣告obj的x軸(前進方向)向量.
        Vector3 cForward = ct.forward;
        data.m_vCurrentVector = cForward;
        //宣告obj與障礙物兩點圓心向量.
        Vector3 vec;
        //宣告最高威脅障礙物與obj的probe的垂直距離.
        float fFinalDotDist;
        //宣告最高威脅障礙物與obj的probe的平行距離.
        float fFinalProjDist;
        //宣告最高威脅障礙物與obj的圓心距離.
        Vector3 vFinalVec = Vector3.forward;
        //宣告最高威脅障礙物
        Obstacle oFinal = null;
        float fDist = 0.0f;
        float fDot = 0.0f;
        float fFinalDot = 0.0f;
        int iCount = m_AvoidTargets.Count;

        float fMinDist = 10000.0f;
        //對每個於m_AvoidTargets的障礙物資料判斷
        for (int i = 0; i < iCount; i++)
        {
            //step_1, 先以obj為圓心, 並以probelength做半徑畫圓, 並判斷each障礙物是否於圓內, 若無則劃分outside_test.
            vec = m_AvoidTargets[i].transform.position - cPos;
            vec.y = 0.0f;
            fDist = vec.magnitude;
            if (fDist > data.m_fProbeLength + m_AvoidTargets[i].m_fRadius)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            }
            //step_2, 以each障礙物圓心與obj面對方向做內積, 並以正負判斷該障礙物是否於obj行進方向的前半圓中, 若無則劃分outside_test.
            vec.Normalize();
            fDot = Vector3.Dot(vec, cForward);
            if (fDot < 0)
            {
                m_AvoidTargets[i].m_eState = Obstacle.eState.OUTSIDE_TEST;
                continue;
            } else if(fDot > 1.0f)
            {
                fDot = 1.0f;
            }
            //step_3, 判斷障礙物是否有碰撞可能，並判斷該障礙物是否為最高威脅障礙物.
            //通過step_1, step_2判斷者，劃分至inside_test.
            m_AvoidTargets[i].m_eState = Obstacle.eState.INSIDE_TEST;
            //計算障礙物與obj在probe上的平行距離.
            float fProjDist = fDist * fDot;
            //計算障礙物與obj的probe的垂直距離.
            float fDotDist = Mathf.Sqrt(fDist * fDist - fProjDist * fProjDist);
            //判斷obj的probe與障礙物兩點圓心距離是否大於r1+r2, 若是則continue(無碰撞可能).若否則繼續.
            if (fDotDist > m_AvoidTargets[i].m_fRadius + data.m_fRadius)
            {
                continue;
            }
            //判斷該障礙物是否為obj的最高威脅障礙物, 若是則替代, 若否則繼續.
            if (fDist < fMinDist)
            {
                fMinDist = fDist;
                fFinalDotDist = fDotDist;
                fFinalProjDist = fProjDist;
                vFinalVec = vec;
                oFinal = m_AvoidTargets[i];
                fFinalDot = fDot;
            }

        }
        //for迴圈結束, 判斷是否有最高威脅障礙物
        if(oFinal != null)
        {
            Vector3 vCross = Vector3.Cross(cForward, vFinalVec);
            float fTurnMag = Mathf.Sqrt(1.0f - fFinalDot * fFinalDot);
            if (vCross.y > 0.0f)
            {
                fTurnMag = -fTurnMag;
            }
            data.m_fTempTurnForce = fTurnMag;

            float fTotalLen = data.m_fProbeLength + oFinal.m_fRadius;
            float fRatio = fMinDist / fTotalLen;
            if(fRatio > 1.0f)
            {
                fRatio = 1.0f;
            }
            fRatio = 1.0f - fRatio;
            data.m_fMoveForce = -fRatio;
            oFinal.m_eState = Obstacle.eState.COL_TEST;
            data.m_bCol = true;
            data.m_bMove = true;
            return true;
        }
        data.m_bCol = false;
        return false;
    }

    static public bool Flee(AIData data)
    {
        Vector3 cPos = data.m_Go.transform.position;
        Vector3 vec = data.m_vTarget - cPos;
        vec.y = 0.0f;
        float fDist = vec.magnitude;
        data.m_fTempTurnForce = 0.0f;
        if (data.m_fProbeLength < fDist)
        {
            if(data.m_Speed > 0.01f)
            {
                data.m_fMoveForce = -1.0f;
            } 
            
            data.m_bMove = true;
            return false;
        }

        Vector3 vf = data.m_Go.transform.forward;
        Vector3 vr = data.m_Go.transform.right;
        data.m_vCurrentVector = vf;
        vec.Normalize();
        float fDotF = Vector3.Dot(vf, vec);
        if (fDotF < -0.96f)
        {
            fDotF = -1.0f;
            data.m_vCurrentVector = -vec;
            //  data.m_Go.transform.forward = -vec;
            data.m_fTempTurnForce = 0.0f;
            data.m_fRot = 0.0f;
        }
        else
        {
            if (fDotF > 1.0f)
            {
                fDotF = 1.0f;
            }
            float fDotR = Vector3.Dot(vr, vec);

            if (fDotF > 0.0f)
            {

                if (fDotR > 0.0f)
                {
                    fDotR = 1.0f;
                }
                else
                {
                    fDotR = -1.0f;
                }

            }
            Debug.Log(fDotR);
            data.m_fTempTurnForce = -fDotR;

            // data.m_fTempTurnForce *= 0.1f;


        }

        data.m_fMoveForce = -fDotF;
        data.m_bMove = true;
        return true;
    }

    static public bool Seek(AIData data)
    {
        //宣告cPos=obj的位置向量, vec=obj至target的向量(desire vec).
        Vector3 cPos = data.m_Go.transform.position;
        Vector3 vec = data.m_vTarget - cPos;
        //固定y軸.
        vec.y = 0.0f;
        //宣告fDist = vec長度.
        float fDist = vec.magnitude;
        //Dead zone.
        if (fDist < data.m_Speed + 0.001f)
        {
            Vector3 vFinal = data.m_vTarget;
            vFinal.y = cPos.y;
            data.m_Go.transform.position = vFinal;
            data.m_fMoveForce = 0.0f;
            data.m_fTempTurnForce = 0.0f;
            data.m_Speed = 0.0f;
            data.m_bMove = false;
            return false;
        }
        //宣告vf=obj的面對方向的單位向量, vr=obj的右邊方向的單位向量.
        Vector3 vf = data.m_Go.transform.forward;
        Vector3 vr = data.m_Go.transform.right;
        data.m_vCurrentVector = vf;
        vec.Normalize();
        //宣告fDotF=vf與vec內積.
        float fDotF = Vector3.Dot(vf, vec);
        //debug.
        if(fDotF > 0.96f)
        {
            fDotF = 1.0f;
            data.m_vCurrentVector = vec;
            data.m_fTempTurnForce = 0.0f;
            data.m_fRot = 0.0f;
        } else
        {
            if (fDotF < -1.0f)
            {
                fDotF = -1.0f;
            }
            float fDotR = Vector3.Dot(vr, vec);

            if (fDotF < 0.0f)
            {
               
                if (fDotR > 0.0f)
                {
                    fDotR = 1.0f;
                } else
                {
                    fDotR = -1.0f;
                }
               
            } 
        //若距離短則轉彎&前進force增加.
            if(fDist < 3.0f)
            {
                fDotR *= (fDist / 3.0f + 1.0f);
            }
            data.m_fTempTurnForce = fDotR;

        }

        if(fDist < 3.0f)
        {
            Debug.Log(data.m_Speed);
            if(data.m_Speed > 0.1f)
            {
                data.m_fMoveForce = -(1.0f - fDist/3.0f)*5.0f;
            } else
            {
                data.m_fMoveForce = fDotF*100.0f;
            }
            
        } else
        {
            data.m_fMoveForce = 100.0f;
        }


       
        data.m_bMove = true;
        return true;
    }
}
