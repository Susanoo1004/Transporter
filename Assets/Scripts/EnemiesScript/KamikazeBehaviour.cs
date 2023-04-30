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

    [SerializeField]
    private AudioSource m_KamikazeExplosion;

    [SerializeField]
    private AudioSource m_KamikazeFtsp;

    [SerializeField]
    private AudioClip[] m_KamikazeFtspList;

    [SerializeField]
    private AudioSource m_KamikazeDetectionSound;

    [SerializeField]
    private AudioSource m_KamikazeRollLoop;

    [SerializeField]
    private AudioSource m_KamikazeGoToRoll;

   
    private bool m_KamikazeRollPlay = true;

    private void Awake()
    {
        m_NavAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
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
        m_Animator.SetFloat("SpeedX", m_NavAgent.speed);
    }

    public override void FocusPlayer()
    {
        if (!IsExploding)
        {
            if (m_KamikazeRollPlay)
            {
                m_KamikazeRollLoop.Play();
                m_KamikazeRollPlay = false;

            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, m_DetonationRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {                   
                    IsExploding = true;
                    m_KamikazeExplosion.Play();
                    return;                    
                }
            }

            m_NavAgent.SetDestination(m_Target.transform.position);
        }
        else
        {
            m_Animator.Play("Explode");            
            Vector3 vecBetweenTargetandKamikaze = (m_Target.transform.position + Vector3.up / 2) - (transform.position + Vector3.down);
            m_Animator.SetFloat("SpeedX", 1);

            m_Animator.applyRootMotion = true;
            m_NavAgent.enabled = true;

            if (!m_Target.GetComponent<PlayerBehaviour>().IsGrounded)
            {
                m_Animator.applyRootMotion = false;
                m_NavAgent.enabled = false;

                if (!HasJump)
                {
                    transform.position += Vector3.up / 2;

                    Rigidbody rb = GetComponent<Rigidbody>();
                    rb.AddForce(Vector3.up * 2, ForceMode.VelocityChange);
                    rb.AddForce(vecBetweenTargetandKamikaze.normalized * m_JumpForce, ForceMode.VelocityChange);
                    HasJump = true;
                }
            }
            else
            {
                m_NavAgent.SetDestination(m_Target.transform.position);

            }

            m_HitCD -= Time.deltaTime;

            if (vecBetweenTargetandKamikaze.magnitude >= 2.8f && m_HitCD > 0.0f)
                return;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 2.8f, 1 << 6);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (m_Target.TryGetComponent(out PlayerBehaviour player) && player.m_InvicibilityTimer < 0)
                    {
                        player.TakeDamage(m_EnemyDamage);
                    }
                    break;
                }
            }

            m_KamikazeRollLoop.enabled = false; 
            
            // ms : son explosion
            

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

            m_KamikazeDetectionSound.Play();
            // ms : son detection 

        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, m_DetonationRadius);
    }

    public void play_KamikazeFtsp()
    {
        
        int index = Random.Range(0, m_KamikazeFtspList.Length);
        m_KamikazeFtsp.PlayOneShot(m_KamikazeFtspList[index]);
    }

    public void play_KamikazeGoToRoll()
    {
        m_KamikazeGoToRoll.Play();
        
    }
}
