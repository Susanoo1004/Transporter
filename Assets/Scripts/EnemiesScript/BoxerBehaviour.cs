using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class BoxerBehaviour : EnemyBehaviour
{
    private void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_EnemyHP = 2;
        m_Animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_NavAgent.SetDestination(m_NavPoints[0].position);
    }

    // Update is called once per frame
    void Update()
    {
        MyUpdate();
        m_Animator.SetFloat("SpeedX", m_NavAgent.speed);
    }

    public override void FocusPlayer()
    {
        m_NavAgent.SetDestination(m_Target.transform.position);
        if (Vector3.Distance(transform.position, m_Target.transform.position) <= m_NavAgent.stoppingDistance)
        {
            m_HitCD -= Time.deltaTime;

            if (m_HitCD > 0)
                return;

            m_Target.GetComponent<PlayerBehaviour>().PlayerLife -= 1;
            m_HitCD = m_HitFrequency;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPatrolling = false;
            m_NavAgent.speed = 3.0f;
            m_NavAgent.stoppingDistance = 1.75f;
            m_Target = other.gameObject;
        }
    }

}
