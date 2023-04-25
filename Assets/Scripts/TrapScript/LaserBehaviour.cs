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
    private float m_MaxDistance;

    private GameObject m_Target;

    [SerializeField]
    private float m_HitFrequency;
    private float m_HitCD;

    [SerializeField]
    private byte m_LaserDamage;

    public bool IsGuardingByEnemy;

    private bool IsPlayer;

    // Start is called before the first frame update
    void Start()
    {
        m_HitCD = m_HitFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] HitPoints = Physics.RaycastAll(m_Shooter.position, Vector3.down, m_MaxDistance, 1 << 6 | 1 << 0);
        foreach (RaycastHit hit in HitPoints) 
        {
            IsPlayer = false;
            m_Target = hit.collider.gameObject;

            if (m_Target.layer == LayerMask.NameToLayer("Player"))
            {
                IsPlayer = true;
                m_HitCD -= Time.deltaTime;
                if (m_HitCD > 0)
                    return;

                PlayerBehaviour playerBehaviour = m_Target.GetComponent<PlayerBehaviour>();
                if (playerBehaviour.PlayerLife < m_LaserDamage)
                {
                    playerBehaviour.PlayerLife = 0;
                    return;
                }

                m_Target.GetComponent<PlayerBehaviour>().PlayerLife -= m_LaserDamage;
                m_HitCD = m_HitFrequency;
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Vector3 temp = Vector3.zero;
        Vector3 temp2 = Vector3.zero;
        if (!IsPlayer) 
        { 
            temp = m_Target.transform.position - m_Shooter.position;
            temp2 = Vector3.zero - new Vector3(m_Shooter.position.x, 0, 0);
        }

        Gizmos.DrawLine(m_Shooter.position, IsPlayer ? m_Target.transform.position : (m_Shooter.position - new Vector3(0, Mathf.Sqrt(temp.sqrMagnitude - temp2.sqrMagnitude), 0)));
    }
}
