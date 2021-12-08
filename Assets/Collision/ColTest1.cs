using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColTest1 : MonoBehaviour {

    public AIData m_Data;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CollisionAvoid1.CheckObstacleInRange(m_Data);
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(this.transform.position, m_Data.m_fProbeLength);




    }
}
