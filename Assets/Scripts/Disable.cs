using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Disable : MonoBehaviour
{
    [Header("Laser")]
    [SerializeField]
    private LaserBehaviour m_LaserToDisable;

    [SerializeField]
    private EnemyBehaviour[] m_EnemyGuardian;

    [Header("Value")]
    [SerializeField]
    private float m_ActivateFrequency;
    private float m_ActivateTimer;

    [SerializeField]
    private float m_DisableFrequency;
    private float m_DisableTimer;

    [SerializeField]
    private VisualEffect m_VisualEffect;


    // Start is called before the first frame update
    void Start()
    {
        m_DisableTimer = m_DisableFrequency;
        m_ActivateTimer = m_ActivateFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_LaserToDisable.IsGuardingByEnemy) 
        {
            if(m_LaserToDisable.enabled) 
            {
                m_ActivateTimer -= Time.deltaTime;

                if (m_ActivateTimer > 0)
                    return;

                m_VisualEffect.enabled = false;
                m_LaserToDisable.enabled = false;

                m_ActivateTimer = m_ActivateFrequency;
            }
            else
            {
                m_DisableTimer -= Time.deltaTime;

                if (m_DisableTimer > 0)
                    return;

                m_VisualEffect.enabled = true;
                m_LaserToDisable.enabled = true;

                m_DisableTimer = m_DisableFrequency;

            }

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
            {
                m_LaserToDisable.enabled = false;
                m_VisualEffect.enabled = false;
            }
            else
            {
                m_VisualEffect.enabled = true;
            }
        }
        
    }
    
}
