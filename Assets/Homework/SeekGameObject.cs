using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekGameObject : MonoBehaviour
{
    public GameObject m_Target;
    public SeekData m_seekData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_seekData.m_vTarget = m_Target.transform.position;
        //SeekBehavior.Seek(m_seekData);
        //SeekBehavior.Move(m_seekData);
        //m_seekData.m_vTarget = m_Target.transform.position;
        //SeekBehavior.Seek(m_seekData);
        //SeekBehavior.Move(m_seekData);
        SeekBehavior.SeekRigid(m_seekData);
    }
    private void FixedUpdate()
    {
        //m_seekData.m_vTarget = m_Target.transform.position;
        //SeekBehavior.Seek(m_seekData);
        //SeekBehavior.Move(m_seekData);

        //SeekBehavior.SeekRigid(m_seekData);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 3.0f);

    }
}
