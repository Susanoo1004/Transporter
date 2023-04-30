using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        if (Physics.Raycast(m_Shooter.position, -m_Shooter.up, out RaycastHit Hit, m_Distance, 1 << 6 | 1 << 0))
        {
            m_Target = Hit.collider.gameObject;

            if (m_Target.layer == LayerMask.NameToLayer("Default"))
                return;

            if (m_Target.layer == LayerMask.NameToLayer("Player"))
            {
                m_HitCD -= Time.deltaTime;
                if (m_HitCD > 0)
                    return;

                PlayerBehaviour playerBehaviour = m_Target.GetComponent<PlayerBehaviour>();

                if (playerBehaviour.PlayerLife != 0)
                    playerBehaviour.TakeDamage(m_LaserDamage);

                m_HitCD = m_HitFrequency;
                return;
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
