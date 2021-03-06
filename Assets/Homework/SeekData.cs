using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SeekData
{
    public float m_fSpeed;
    public float m_fMaxSpeed=20.0f;
    public float m_fRotSpeed;
    public float m_fMaxRotSpeed=20.0f;
    public float m_fMinusRadius = 20.0f;
    public GameObject m_GO;
    public Rigidbody mRB;
    [HideInInspector]
    public float m_fRampedSpeed;
    public float m_fClippedSpeed;
    [HideInInspector]
    public Vector3 m_vTarget;
    [HideInInspector]
    public float m_fTempTurnForce;
    [HideInInspector]
    public float m_fMoveForce;

}
