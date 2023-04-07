using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : EnemyBehaviour
{
    [SerializeField]
    float m_ShootFrequency;

    float m_ShootCD;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        m_ShootCD -= Time.deltaTime;

        if (m_ShootCD > 0)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit");
            m_ShootCD = m_ShootFrequency;
        }
    }
}
