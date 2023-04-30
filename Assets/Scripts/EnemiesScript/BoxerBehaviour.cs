using UnityEngine;
using UnityEngine.AI;

public class BoxerBehaviour : EnemyBehaviour
{
    [SerializeField]
    private float m_AttackRange;

    [SerializeField]
    private AudioSource m_BoxerFstpWalk;

    [SerializeField]
    private AudioClip[] m_BoxerFstpWalkList;

    [SerializeField]
    private AudioSource m_BoxerFstpRun;

    [SerializeField]
    private AudioClip[] m_BoxerFstpRunList;

    [SerializeField]
    private AudioSource m_BoxerPunch;

    [SerializeField]
    private AudioClip[] m_BoxerPunchList;

    [SerializeField]
    private AudioSource m_ShieldAttack;
        
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

                //son du boxeur punch (aléatoire)
                play_BoxerPunch();

                //son du shield attack (aléatoire) --> les gp doivent faire un script :)
                m_ShieldAttack.Play();
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

    // ms : footsteps sound on animator
    public void play_BoxerFstpWalk()
    {

        int index = Random.Range(0, m_BoxerFstpWalkList.Length);
        m_BoxerFstpWalk.PlayOneShot(m_BoxerFstpWalkList[index]);
    }

    public void play_BoxerFstpRun()
    {

        int index = Random.Range(0, m_BoxerFstpRunList.Length);
        m_BoxerFstpRun.PlayOneShot(m_BoxerFstpRunList[index]);
    }

    public void play_BoxerPunch()
    {

        int index = Random.Range(0, m_BoxerPunchList.Length);
        m_BoxerPunch.PlayOneShot(m_BoxerPunchList[index]);
    }
      
}
