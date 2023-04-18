using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KamikazeBehaviour : EnemyBehaviour
{
    private bool IsExploding;

    private bool HasJump;

    [Header("Explosion")]
    [SerializeField]
    private float m_DetonationRadius;

    [SerializeField]
    private float m_JumpForce;

    [SerializeField]
    private float m_ExplosionKnocknack;

    public Transform player;

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
            GetComponent<NavMeshAgent>().enabled = false;
            Vector3 vecBetweenTargetandKamikaze = (m_Target.transform.position + Vector3.up/2) - (transform.position + Vector3.down/2);
            if (!HasJump)
            {
                transform.position += Vector3.up/5;
                GetComponent<Rigidbody>().AddForce(vecBetweenTargetandKamikaze.normalized * m_JumpForce, ForceMode.VelocityChange);
                HasJump = true;
            }

            m_HitCD -= Time.deltaTime;

            if (vecBetweenTargetandKamikaze.magnitude >= 2.8f && m_HitCD > 0.0f)
                return;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.8f, 1 << 6);
           foreach (Collider collider in colliders)
           {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    m_Target.GetComponent<PlayerBehaviour>().PlayerLife -= 2;

                    // To add an explosion force
                    m_Target.GetComponent<Rigidbody>().AddExplosionForce(m_ExplosionKnocknack * Time.fixedDeltaTime, transform.position + Vector3.down, 1);
                    m_Target.GetComponent<PlayerBehaviour>().m_HasTakenExplosion = true;
                    break;
                }
           }

           Destroy(gameObject);
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
    }
}
