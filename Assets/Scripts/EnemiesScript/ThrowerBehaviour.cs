using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowerBehaviour : MonoBehaviour
{

    private Transform m_Target;

    [SerializeField]
    private GameObject m_Bullet;

    [SerializeField]
    private float m_ShootTime;
    private float m_ShootTimer;

    [SerializeField]
    private float m_ThrowForce;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Target != null)
        {
            if (m_ShootTimer > 0)
            {
                m_ShootTimer -= Time.deltaTime;
            }
            else
            {
                Vector3 distance = m_Target.position - transform.position;

                GameObject bulletShot = Instantiate(m_Bullet, transform.position + distance.normalized, Quaternion.identity);

                bulletShot.layer = LayerMask.NameToLayer("Enemy Projectiles");

                if (bulletShot.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.AddForce(distance.normalized * m_ThrowForce);

                if (bulletShot.TryGetComponent(out MagneticObject magnetic))
                    magnetic.DestroyOnCollision = true;

                m_ShootTimer = m_ShootTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_Target = other.transform;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == m_Target.gameObject)
        {
            m_Target = null;
        }
    }
}
