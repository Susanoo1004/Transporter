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

    private PlayerInput m_PlayerInput;

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
                m_PlayerInput.SwitchCurrentActionMap(m_OldActionMap);
                m_HasBeenTrap = false;
            }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out PlayerBehaviour player))
            return;

        if (!other.TryGetComponent(out Rigidbody rigidbody))
            return;

        if (player.DashTimer > 0)
            return;

        if (player.IsGrounded)
            rigidbody.AddForce((transform.position - other.transform.position).normalized * m_AttractionSpeed * 2, ForceMode.Acceleration);
        else
            rigidbody.AddForce((transform.position - other.transform.position).normalized * m_AttractionSpeed, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_Target = collision.transform;
            Debug.Log("Collision Player");
            m_OldActionMap = m_Target.GetComponent<PlayerInput>().currentActionMap.name;
            m_Target.TryGetComponent(out PlayerInput playerInput);
            m_PlayerInput = playerInput;
            playerInput.SwitchCurrentActionMap("Trap");
            m_HasBeenTrap = true;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Projectiles"))
            Destroy(gameObject);
    }

}
