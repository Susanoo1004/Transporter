using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable : MonoBehaviour
{
    [Header("Laser")]
    [SerializeField]
    private LaserBehaviour m_LaserToDisable;

    [SerializeField]
    private EnemyBehaviour[] m_EnemyGuardian;

    [Header("Value")]
    [SerializeField]
    private float m_DisableFrequency;
    private float m_DisableTimer;

    private bool m_Enable;

    // Start is called before the first frame update
    void Start()
    {
        m_DisableTimer = m_DisableFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_LaserToDisable.IsGuardingByEnemy) 
        {
            m_DisableTimer -= Time.deltaTime;

            if (m_DisableTimer > 0)
                return;
            
            if (m_LaserToDisable.enabled == false)
                m_LaserToDisable.enabled = true;
            else
                m_LaserToDisable.enabled = false;

            m_DisableTimer = m_DisableFrequency;
        }
        else
        {
            int enemyDead = 0;
            for(int i = 0; i < m_EnemyGuardian.Length; i++) 
            {
                if (m_EnemyGuardian[i] == null)
                    enemyDead++;
            }

            if(enemyDead == m_EnemyGuardian.Length)
                m_LaserToDisable.enabled = false;
        }
        
    }
    
}
