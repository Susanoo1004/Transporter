using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class LaserBehaviour : MonoBehaviour
{

    [SerializeField] 
    private Transform m_Shooter;

    [SerializeField]
    private float m_Distance;

    private GameObject m_Target;

    [SerializeField]
    private float m_HitFrequency;
    private float m_HitCD;

    [SerializeField]
    private byte m_LaserDamage;

    public bool IsGuardingByEnemy;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] HitPoints = Physics.RaycastAll(m_Shooter.position, -m_Shooter.up, m_Distance, 1 << 6 | 1 << 0);
        foreach (RaycastHit hit in HitPoints) 
        {
            m_Target = hit.collider.gameObject;

            if (m_Target.layer == LayerMask.NameToLayer("Player"))
            {
                m_HitCD -= Time.deltaTime;
                if (m_HitCD > 0)
                    return;

                PlayerBehaviour playerBehaviour = m_Target.GetComponent<PlayerBehaviour>();
                playerBehaviour.TakeDamage(m_LaserDamage);

                m_Target.GetComponent<PlayerBehaviour>().PlayerLife -= m_LaserDamage;
                m_HitCD = m_HitFrequency;
            }
        }

        if (IsGuardingByEnemy) 
        {
            // ms : son, laser allume en loop
        }
        else
        {
            // ms : son, laser allume 3 sec
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(m_Shooter.position, m_Shooter.position - (m_Shooter.up * m_Distance));
    }
}
