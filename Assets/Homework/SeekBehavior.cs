using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehavior
{
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
        Vector3 vF = t.forward;
        if (data.m_fTempTurnForce > data.m_fMaxRotSpeed)
        {
            data.m_fTempTurnForce = data.m_fMaxRotSpeed;
        }
        else if (data.m_fTempTurnForce < -data.m_fMaxRotSpeed)
        {
            data.m_fTempTurnForce = -data.m_fMaxRotSpeed;
        }
        vF = vF + vR * data.m_fTempTurnForce;
        t.forward = vF;
        if (data.m_fMoveForce > data.m_fMaxSpeed)
        {
            data.m_fMoveForce = data.m_fMaxSpeed;
        }
        else if (data.m_fMoveForce < -data.m_fMaxSpeed)
        {
            data.m_fMoveForce = -data.m_fMaxSpeed;
        }
        data.mRB.AddForce(t.forward * data.m_fMoveForce);
        //data.mRB.velocity = t.forward * data.m_fMoveForce;
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
        data.m_fTempTurnForce = fDotR*0.1f;
        data.m_fMoveForce = fDotF*10.0f;
        data.m_bMove = true;
        return true;
    }
    static public void SeekRigid(SeekData data)
    {
        //if (data.m_bMove==false)
        //{
        //    return;
        //}
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
        data.m_fTempTurnForce = fDotR * 10.0f;
        data.m_fMoveForce = fDotF * 10.0f;

        Transform t = data.m_GO.transform;
        Vector3 vR = t.right;
        Vector3 vF = t.forward;

        vF = vF + vR * data.m_fTempTurnForce*Time.deltaTime;
        t.forward = vF;

        data.mRB.AddForce(t.forward * data.m_fMoveForce);

        if (fDist < 2.5f)
        {
            data.mRB.velocity = Vector3.zero;
            //data.m_bMove = false;
            return;
        }
        else if (fDist < 20.0f)
        {
            data.m_fRampedSpeed = data.m_fMaxSpeed * (fDist / 20.0f);
            data.m_fClippedSpeed = Mathf.Min(data.m_fRampedSpeed, data.m_fMaxSpeed);
            return;
        }
        else
        { 
            data.mRB.AddForce(t.forward * data.m_fMoveForce);
            data.mRB.velocity = Vector3.ClampMagnitude(data.mRB.velocity, data.m_fMaxSpeed);
        }

    }
    
}
