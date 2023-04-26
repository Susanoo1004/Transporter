using UnityEngine;
using UnityEngine.AI;

public class BoxerBehaviour : EnemyBehaviour
{
    [SerializeField]
    private float m_AttackRange;

    private void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
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

            if (m_Target.TryGetComponent(out PlayerBehaviour player) && player.m_InvicibilityTimer < 0)
            {
                player.TakeDamage(m_EnemyDamage);
                m_HitCD = m_HitFrequency;
                m_Animator.Play("Hit");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPatrolling = false;
            m_NavAgent.speed = m_NavAgent.speed * 2;
            m_NavAgent.stoppingDistance = m_AttackRange;
            m_Target = other.gameObject;
        }
    }

}
