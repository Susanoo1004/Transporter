using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretBehaviour : MonoBehaviour
{

    [SerializeField]
    private float m_AttractionSpeed;

    private Transform m_Target;

    private string m_OldActionMap;

    private bool m_HasBeenTrap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_HasBeenTrap)
            if (Vector3.Distance(transform.position, m_Target.position) > 2.0f)
            {
                m_Target.GetComponent<PlayerInput>().SwitchCurrentActionMap(m_OldActionMap);
                m_HasBeenTrap = false;
            }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PlayerBehaviour player)  && !other.TryGetComponent(out MagneticObject magneticObject))
            return;

        if (other.GetComponent<PlayerBehaviour>().IsGrounded)
            other.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position).normalized * m_AttractionSpeed * 2, ForceMode.Acceleration);
        else
            other.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position).normalized * m_AttractionSpeed, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_Target = collision.transform;
            m_OldActionMap = m_Target.GetComponent<PlayerInput>().currentActionMap.name;
            m_Target.GetComponent<PlayerInput>().SwitchCurrentActionMap("Trap");
            m_HasBeenTrap = true;
        }
        
    }
}
