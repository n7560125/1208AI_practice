using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFSM2 : MonoBehaviour{

	public enum eFSMState
	{
		NONE = -1,
		Idle,
		MoveToTarget,
		Chase,
		Attack,
		Dead
	}

    private delegate void DoState();
    private DoState m_DoState;
    private delegate void CheckAIState();
    private CheckAIState m_CheckState;

    private eFSMState m_eCurrentState;
	public AIData m_Data;
	private float m_fCurrentTime;
	private float m_fIdleTime;
	private GameObject m_CurrentEnemyTarget;
	private int m_iCurrentWanderPt;
	private GameObject [] m_WanderPoints;
	
	// Use this for initialization
	public void Start () {
		m_CurrentEnemyTarget = null;
		m_eCurrentState = eFSMState.Idle;
		m_fCurrentTime = 0.0f;
		m_fIdleTime = Random.Range(3.0f, 5.0f);
		m_iCurrentWanderPt = -1;
		m_WanderPoints = GameObject.FindGameObjectsWithTag("WanderPoint");
        m_DoState = DoIdleState;
        m_CheckState = CheckIdleState;

    }

	private GameObject CheckEnemyInSight(ref bool bAttack)
	{
		GameObject go = Main.m_Instance.GetPlayer();
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
		if(fDist < m_Data.m_fAttackRange)
		{
			bAttack = true;
			return go;
		}
		else if(fDist < m_Data.m_fSight)
		{
			bAttack = false;
			return go;
		}
		return null;
	}

	private bool CheckTargetEnemyInSight(GameObject target, ref bool bAttack)
	{
		GameObject go = target;
		Vector3 v = go.transform.position - this.transform.position;
		float fDist = v.magnitude;
		if (fDist < m_Data.m_fAttackRange)
		{
			bAttack = true;
			return true;
		}
		else if (fDist < m_Data.m_fSight)
		{
			bAttack = false;
			return true;
		}
		return false;
	}
    void CheckAttackState()
    {
        bool bAttack = false;
        bool bCheck = CheckTargetEnemyInSight(m_CurrentEnemyTarget, ref bAttack);
        if (bCheck == false)
        {
            m_eCurrentState = eFSMState.Idle;
            m_fCurrentTime = 0.0f;
            m_fIdleTime = Random.Range(3.0f, 5.0f);
            m_DoState = DoIdleState;
            m_CheckState = CheckIdleState;
            return;
        }
        if (bAttack == false)
        {
            m_Data.m_TargetObject = m_CurrentEnemyTarget;
            m_eCurrentState = eFSMState.Chase;
            m_DoState = DoChaseState;
            m_CheckState = CheckChaseState;
            return;
        }
    }
    void CheckChaseState()
    {
        // Check Dead.

        bool bAttack = false;
        bool bCheck = CheckTargetEnemyInSight(m_CurrentEnemyTarget, ref bAttack);
        if (bCheck == false)
        {
            m_eCurrentState = eFSMState.Idle;
            m_fCurrentTime = 0.0f;
            m_fIdleTime = Random.Range(3.0f, 5.0f);
            m_DoState = DoIdleState;
            m_CheckState = CheckIdleState;
            return;
        }
        if (bAttack)
        {
            m_eCurrentState = eFSMState.Attack;
            m_Data.m_fAttackTime = Random.Range(1.0f, 3.0f);
            m_fCurrentTime = 0.0f;
            m_DoState = DoAttackState;
            m_CheckState = CheckAttackState;
        }
    }
    void CheckMoveToState()
    {
        // Check Dead.

        bool bAttack = false;
        m_CurrentEnemyTarget = CheckEnemyInSight(ref bAttack);
        if (m_CurrentEnemyTarget != null)
        {
            m_Data.m_TargetObject = m_CurrentEnemyTarget;
            if (bAttack)
            {
                m_eCurrentState = eFSMState.Attack;
                m_DoState = DoAttackState;
                m_CheckState = CheckAttackState;
            }
            else
            {
                m_eCurrentState = eFSMState.Chase;
                m_DoState = DoChaseState;
                m_CheckState = CheckChaseState;
            }
            return;
        }

        if (m_Data.m_bMove == false)
        {
            m_eCurrentState = eFSMState.Idle;
            m_fCurrentTime = 0.0f;
            m_fIdleTime = Random.Range(3.0f, 5.0f);
            m_DoState = DoIdleState;
            m_CheckState = CheckIdleState;
        }
    }
    void CheckIdleState()
    {
        // Check Dead.
        bool bAttack = false;
        m_CurrentEnemyTarget = CheckEnemyInSight(ref bAttack);
        if (m_CurrentEnemyTarget != null)
        {
            m_Data.m_TargetObject = m_CurrentEnemyTarget;
            if (bAttack)
            {
                m_eCurrentState = eFSMState.Attack;
                m_DoState = DoAttackState;
                m_CheckState = CheckAttackState;
            }
            else
            {
                m_eCurrentState = eFSMState.Chase;
                m_DoState = DoChaseState;
                m_CheckState = CheckChaseState;
            }
            return;
        }
        // Wait to move.
        Debug.Log(m_fCurrentTime + ":" + m_fIdleTime);
        if (m_fCurrentTime > m_fIdleTime)
        {
            m_fCurrentTime = 0.0f;
            int iNewPt = Random.Range(0, m_WanderPoints.Length);
            if (m_iCurrentWanderPt == iNewPt)
            {
                return;
            }
            m_fIdleTime = 0.5f;
            m_iCurrentWanderPt = iNewPt;
            m_Data.m_vTarget = m_WanderPoints[m_iCurrentWanderPt].transform.position;
            m_Data.m_bMove = true;
            m_eCurrentState = eFSMState.MoveToTarget;
            m_DoState = DoMoveToState;
            m_CheckState = CheckMoveToState;
        }
    }

    void CheckState()
    {
        if (m_eCurrentState == eFSMState.Idle)
        {
            // Check Dead.
            bool bAttack = false;
            m_CurrentEnemyTarget = CheckEnemyInSight(ref bAttack);
            if (m_CurrentEnemyTarget != null)
            {
                m_Data.m_TargetObject = m_CurrentEnemyTarget;
                if (bAttack)
                {
                    m_eCurrentState = eFSMState.Attack;
                    m_DoState = DoAttackState;
                }
                else
                {
                    m_eCurrentState = eFSMState.Chase;
                    m_DoState = DoChaseState;
                }
                return;
            }
            // Wait to move.
            Debug.Log(m_fCurrentTime + ":" + m_fIdleTime);
            if (m_fCurrentTime > m_fIdleTime)
            {
                m_fCurrentTime = 0.0f;
                int iNewPt = Random.Range(0, m_WanderPoints.Length);
                if (m_iCurrentWanderPt == iNewPt)
                {
                    return;
                }
                m_fIdleTime = 0.5f;
                m_iCurrentWanderPt = iNewPt;
                m_Data.m_vTarget = m_WanderPoints[m_iCurrentWanderPt].transform.position;
                m_Data.m_bMove = true;
                m_eCurrentState = eFSMState.MoveToTarget;
                m_DoState = DoMoveToState;
            }
        }
        else if (m_eCurrentState == eFSMState.MoveToTarget)
        {
            // Check Dead.

            bool bAttack = false;
            m_CurrentEnemyTarget = CheckEnemyInSight(ref bAttack);
            if (m_CurrentEnemyTarget != null)
            {
                m_Data.m_TargetObject = m_CurrentEnemyTarget;
                if (bAttack)
                {
                    m_eCurrentState = eFSMState.Attack;
                    m_DoState = DoAttackState;
                }
                else
                {
                    m_eCurrentState = eFSMState.Chase;
                    m_DoState = DoChaseState;
                }
                return;
            }
           
            if (m_Data.m_bMove == false)
            {
                m_eCurrentState = eFSMState.Idle;
                m_fCurrentTime = 0.0f;
                m_fIdleTime = Random.Range(3.0f, 5.0f);
                m_DoState = DoIdleState;
            }
        }
        else if (m_eCurrentState == eFSMState.Chase)
        {
            // Check Dead.

            bool bAttack = false;
            bool bCheck = CheckTargetEnemyInSight(m_CurrentEnemyTarget, ref bAttack);
            if (bCheck == false)
            {
                m_eCurrentState = eFSMState.Idle;
                m_fCurrentTime = 0.0f;
                m_fIdleTime = Random.Range(3.0f, 5.0f);
                m_DoState = DoIdleState;
                return;
            }
            if (bAttack)
            {
                m_eCurrentState = eFSMState.Attack;
                m_Data.m_fAttackTime = Random.Range(1.0f, 3.0f);
                m_fCurrentTime = 0.0f;
                m_DoState = DoAttackState;
            }

        }
        else if (m_eCurrentState == eFSMState.Attack)
        {

            bool bAttack = false;
            bool bCheck = CheckTargetEnemyInSight(m_CurrentEnemyTarget, ref bAttack);
            if (bCheck == false)
            {
                m_eCurrentState = eFSMState.Idle;
                m_fCurrentTime = 0.0f;
                m_fIdleTime = Random.Range(3.0f, 5.0f);
                m_DoState = DoIdleState;
                return;
            }
            if (bAttack == false)
            {
                m_Data.m_TargetObject = m_CurrentEnemyTarget;
                m_eCurrentState = eFSMState.Chase;
                m_DoState = DoChaseState;
                return;
            }
        }
        else if (m_eCurrentState == eFSMState.Dead)
        {
            // Every state can be here.
            m_DoState = DoDeadState;
        }

    }

    void DoIdleState()
    {
        m_fCurrentTime += Time.deltaTime;
    }
    void DoMoveToState()
    {
        if (SteeringBehavior.CollisionAvoid(m_Data) == false)
        {
            SteeringBehavior.Seek(m_Data);
        }

        SteeringBehavior.Move(m_Data);
    }

    void DoChaseState()
    {
        m_Data.m_vTarget = m_Data.m_TargetObject.transform.position;
        if (SteeringBehavior.CollisionAvoid(m_Data) == false)
        {
            SteeringBehavior.Seek(m_Data);
        }

        SteeringBehavior.Move(m_Data);
    }
    void DoAttackState()
    {
        if (m_fCurrentTime > m_Data.m_fAttackTime)
        {
            m_fCurrentTime = 0.0f;
            // Do attack.
        }
        m_fCurrentTime += Time.deltaTime;
    }

    void DoDeadState()
    {

    }

	// Update is called once per frame
	void Update () {
        //CheckState();
        Debug.Log("m_CheckState");
        m_CheckState();
        m_DoState();
		
		
	}

	private void OnDrawGizmos()
	{
		if(m_Data == null)
		{
			return;
		}
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 2.0f);
		if (m_eCurrentState == eFSMState.Idle)
		{
			Gizmos.color = Color.green;
		} else if(m_eCurrentState == eFSMState.MoveToTarget)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(this.transform.position, m_Data.m_vTarget);
		}
		else if (m_eCurrentState == eFSMState.Chase)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(this.transform.position, m_Data.m_vTarget);
		}
		else if (m_eCurrentState == eFSMState.Attack)
		{
			Gizmos.color = Color.red;
		}
		else if (m_eCurrentState == eFSMState.Dead)
		{
			Gizmos.color = Color.gray;
		}
		Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fSight);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fAttackRange);
	}
}
