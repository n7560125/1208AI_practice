using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehavior
{
    static float timer = 0;

    static public void Move(SeekData data)
    {
        if (data.m_bMove == false)
        {
            return;
        }
        Transform t = data.m_GO.transform;
        Vector3 cPos = data.m_GO.transform.position;
        Vector3 vR = t.right;
        Vector3 vOriF = t.forward;
        Vector3 vF = data.m_vCurrentVector;
        if (data.m_fTempTurnForce > data.m_fMaxRotSpeed)
        {
            data.m_fTempTurnForce = data.m_fMaxRotSpeed;
        }
        else if (data.m_fTempTurnForce < -data.m_fMaxRotSpeed)
        {
            data.m_fTempTurnForce = -data.m_fMaxRotSpeed;
        }
        if (data.m_fMoveForce > data.m_fMaxSpeed)
        {
            data.m_fMoveForce = data.m_fMaxSpeed;
        }
        else if (data.m_fMoveForce < -data.m_fMaxSpeed)
        {
            data.m_fMoveForce = -data.m_fMaxSpeed;
        }
        data.mRB.AddForce(t.forward * data.m_fMoveForce);
        data.mRB.AddForce(t.right * data.m_fTempTurnForce);
    }
    static public bool Seek(SeekData data)
    {
        Vector3 cPos = data.m_GO.transform.position;
        //vec=Desire Vector.
        Vector3 vec = data.m_vTarget - cPos;
        vec.y = 0.0f;
        float fDist = vec.magnitude;
        Vector3 vf = data.m_GO.transform.forward;
        Vector3 vr = data.m_GO.transform.right;
        //Dot.
        float fDotF = Vector3.Dot(vf, vec);
        float fDotR = Vector3.Dot(vr, vec);
        if (fDotF > 1.0f)
        {
            fDotF = 1.0f;
        }
        else
        {
            if (fDotF < -1.0f)
            {
                fDotF = -1.0f;
            }
        }
        if (fDotR > 1.0f)
        {
            fDotR = 1.0f;
        }
        else
        {
            if (fDotR < -1.0f)
            {
                fDotR = -1.0f;
            }
        }
        data.m_fTempTurnForce = fDotR*10.0f;
        data.m_fMoveForce = fDotF*10.0f;
        data.m_bMove = true;
        return true;
    }
    static public void SeekRigid(SeekData data)
    {
        Vector3 cPos = data.m_GO.transform.position;
        Vector3 vec = data.m_vTarget - cPos;
        vec.y = 0.0f;
        Vector3 vf = data.m_GO.transform.forward;
        Vector3 vr = data.m_GO.transform.right;
        vec.Normalize();
        Vector3 steerVec = vec - data.m_vCurrentVector;
        steerVec.y = 0;
        float fDotF = Vector3.Dot(vf, vec);
        float fDotR = Vector3.Dot(vr, vec);
        //float vf = Vector3.Dot(steerVec, data.m_vCurrentVector);
        //float vr = Vector3.Dot(steerVec, data.m_GO.transform.right);
        data.mRB.AddForce(vf*fDotF * 10.0f);
        data.mRB.AddForce(vr*fDotR* 10.0f);
    }
    
}
