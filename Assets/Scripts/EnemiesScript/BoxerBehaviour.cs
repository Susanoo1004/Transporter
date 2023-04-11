using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
<<<<<<< HEAD
using UnityEngine.AI;

public class BoxerBehaviour : EnemyBehaviour
{


    private void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_EnemyHP = 2;
    }
=======

public class BoxerBehaviour : EnemyBehaviour
{
    

    bool IsPatrolling = true;
>>>>>>> 57fee85 (Change Turret into Boxer Enemy (no asstes and hp loss) #8)

    // Start is called before the first frame update
    void Start()
    {
        m_NavAgent.SetDestination(m_NavPoints[0].position);
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        MyUpdate();
=======
        if (IsPatrolling)
            EnemyPatroll();
        else
            FocusPlayer();

>>>>>>> 57fee85 (Change Turret into Boxer Enemy (no asstes and hp loss) #8)
    }

    public override void FocusPlayer()
    {
<<<<<<< HEAD
        m_NavAgent.SetDestination(m_Target.transform.position);
        if (Vector3.Distance(transform.position, m_Target.transform.position) <= m_NavAgent.stoppingDistance)
=======
        m_NavAgent.SetDestination(m_Target.position);
        if (Vector3.Distance(transform.position, m_Target.position) <= 1.6f)
>>>>>>> 57fee85 (Change Turret into Boxer Enemy (no asstes and hp loss) #8)
        {
            m_HitCD -= Time.deltaTime;

            if (m_HitCD > 0)
                return;

<<<<<<< HEAD
            m_Target.GetComponent<PlayerBehaviour>().PlayerLife -= 1;
=======
            Debug.Log("Ouch");
>>>>>>> 57fee85 (Change Turret into Boxer Enemy (no asstes and hp loss) #8)
            m_HitCD = m_HitFrequency;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPatrolling = false;
            m_NavAgent.speed = 2.5f;
<<<<<<< HEAD
            m_NavAgent.stoppingDistance = 1.75f;
            m_Target = other.gameObject;
        }
    }

=======
            m_NavAgent.stoppingDistance = 1.5f;
            m_Target = other.transform;
        }
    }
>>>>>>> 57fee85 (Change Turret into Boxer Enemy (no asstes and hp loss) #8)
}
