using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField]
    protected float m_HitFrequency;

    [SerializeField]
    protected byte m_EnemyLife;

    [SerializeField]
    protected byte m_EnemyDamage;

    protected NavMeshAgent m_NavAgent;

    protected Animator m_Animator;

    public Transform[] m_NavPoints;

    protected GameObject m_Target;

    protected float m_HitCD;

    protected bool IsPatrolling = true;


    int index;

    private void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void MyUpdate()
    {
        if (IsPatrolling)
            EnemyPatroll();
        else
            FocusPlayer();

        if (m_EnemyLife >= 250)
            m_EnemyLife = 0;

        if (m_EnemyLife == 0)
            Destroy(gameObject);
    }

    public void EnemyPatroll()
    {
        m_NavAgent.SetDestination(m_NavPoints[index].position);

        if (Vector3.Distance(transform.position, m_NavPoints[index].position) <= 1.2f)
        {
            index = (index + 1) % m_NavPoints.Length;
        }
    }

    public virtual void FocusPlayer()
    {
        m_NavAgent.SetDestination(m_Target.transform.position);
        if (Vector3.Distance(transform.position, m_Target.transform.position) <= 1.6f)
        {
            m_HitCD -= Time.deltaTime;

            if (m_HitCD > 0)
                return;

            Debug.Log("Hit");
            m_HitCD = m_HitFrequency;
        }
    }
}
