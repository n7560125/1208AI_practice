using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekBehavior
{
    static public void SeekRigid(SeekData data)
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
        //debug.
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

        //determine turn force and move force.
        data.m_fTempTurnForce = fDotR * 2.0f;
        data.m_fMoveForce = fDotF * 600.0f;

        Transform t = data.m_GO.transform;
        Vector3 vR = t.right;
        Vector3 vF = t.forward;

        //turn force result.
        if (data.m_fTempTurnForce > data.m_fMaxRotSpeed)
        {
            data.m_fTempTurnForce = data.m_fMaxRotSpeed;
        }
        else if (data.m_fTempTurnForce < -data.m_fMaxRotSpeed)
        {
            data.m_fTempTurnForce = -data.m_fMaxRotSpeed;
        }
        vF = vF + vR * data.m_fTempTurnForce*Time.deltaTime;
        t.forward = vF;

        //dead zone.
        if (fDist < 2.0f)
        {
            data.mRB.velocity = Vector3.zero;
            data.mRB.angularVelocity = Vector3.zero;
            return;
        }
        //arrival behavior, use velocity minus radius
        else if (fDist < data.m_fMinusRadius)
        {
            data.m_fRampedSpeed = data.m_fMaxSpeed * (fDist / data.m_fMinusRadius);
            data.m_fClippedSpeed = Mathf.Min(data.m_fRampedSpeed, data.m_fMaxSpeed);
            data.mRB.AddForce(t.forward * data.m_fMoveForce * Time.deltaTime);
            if (data.m_fClippedSpeed > 1.0f)
            {
                data.mRB.velocity = Vector3.ClampMagnitude(data.mRB.velocity, data.m_fClippedSpeed);
            }
            else
            {
                data.mRB.velocity = Vector3.ClampMagnitude(data.mRB.velocity, 1.0f);
            }

            return;
        }
        else
        { 
            data.mRB.AddForce(t.forward * data.m_fMoveForce*Time.deltaTime);
            data.mRB.velocity = Vector3.ClampMagnitude(data.mRB.velocity, data.m_fMaxSpeed);
        }

    }
    
}
