using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SeekData
{
    public float m_fSpeed;
    public float m_fMaxSpeed;
    public float m_fRotSpeed;
    public float m_fMaxRotSpeed;
    public GameObject m_GO;
    public Rigidbody mRB;
    [HideInInspector]
    public Vector3 m_vTarget;
    [HideInInspector]
    public float m_fTempTurnForce;
    [HideInInspector]
    public float m_fMoveForce;
    [HideInInspector]
    public bool m_bMove;
    [HideInInspector]
    public Vector3 m_vCurrentVector;

}
