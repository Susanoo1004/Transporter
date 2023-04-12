using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KamikazeBehaviour : EnemyBehaviour
{
    private bool IsExploding;

    private bool ShowExplosionRange;

    [Header("Explosion")]
    [SerializeField]
    private float m_DetonationRadius;

    [SerializeField]
    private float m_ExplosionRadius;

    [SerializeField]
    private float m_ExplosionKnocknack;

    private void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_EnemyHP = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_HitCD = m_HitFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        MyUpdate();
    }

    public override void FocusPlayer()
    {
        if (!IsExploding)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_DetonationRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    IsExploding = true;
                    return;
                }
            }

            m_NavAgent.SetDestination(m_Target.transform.position);
        }
        else
        {
            ShowExplosionRange = true;

            m_HitCD -= Time.deltaTime;
            
            if (m_HitCD > 0)
                return;

            if (Vector3.Distance(transform.position, m_Target.transform.position) < m_ExplosionRadius)
                m_Target.GetComponent<PlayerBehaviour>().PlayerLife -= 2;

            // To add an explosion force
            m_Target.GetComponent<Rigidbody>().AddExplosionForce(m_ExplosionKnocknack, transform.position + Vector3.down, m_ExplosionRadius);
            m_Target.GetComponent<PlayerBehaviour>().m_HasTakenExplosion = true;

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_DetonationRadius);

        if (ShowExplosionRange) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
            Gizmos.color = Color.white;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPatrolling = false;
            m_NavAgent.speed = 6.0f;
            m_NavAgent.stoppingDistance = 5.0f;
            m_Target = other.gameObject;
        }
 
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_DetonationRadius);

        if (ShowExplosionRange) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
            Gizmos.color = Color.white;
        }
 
    }
}
