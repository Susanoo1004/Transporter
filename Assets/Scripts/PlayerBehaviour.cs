using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    private Vector3 m_Move = new();
    private Vector3 m_LastMove = new();

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    CapsuleCollider m_Collider;

    [SerializeField]
    Transform m_Feet;

    [SerializeField]
    Transform m_Hand;

    [SerializeField]
    private float m_Accelerate;

    [SerializeField]
    private float m_DashPower;

    [SerializeField]
    private float m_JumpHeight;

    [SerializeField]
    private float m_MaxSpeed;

    private bool m_IsDahing = false;
    private float m_DashCd;

    public bool m_IsJumping = false;

    private bool m_IsGrounded { get {
            return Physics.Raycast(m_Feet.transform.position, Vector3.down, 0.05f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.radius, 0, 0), Vector3.down, 0.05f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.radius, 0, 0), Vector3.down, 0.05f); } }

    private bool m_IsStuckLeft;
    private bool m_IsStuckRight;

    private GameObject m_StandingOnObject { get {
        Physics.Raycast(m_Feet.transform.position, Vector3.down, out RaycastHit CenterHit, 0.05f);
        Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.radius, 0, 0), Vector3.down, out RaycastHit LeftHit, 0.05f);
        Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.radius, 0, 0), Vector3.down, out RaycastHit RightHit, 0.05f);
        if (CenterHit.transform.gameObject == null)
            return RightHit.transform.gameObject == null ? LeftHit.transform.gameObject : RightHit.transform.gameObject;
        return CenterHit.transform.gameObject;
    } }


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (m_Move != Vector3.zero)
        {
            Quaternion ToRotation = Quaternion.LookRotation(m_Move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ToRotation, 1080 * Time.deltaTime);
            m_LastMove = m_Move;
        }

        if (m_DashCd > 0)
        {
            m_DashCd -= Time.deltaTime;
            return;
        }
    }
    
    private void FixedUpdate()
    {

        if ((m_Rigidbody.velocity.x >= -m_MaxSpeed && m_Rigidbody.velocity.x <= m_MaxSpeed) || (Mathf.Sign(m_Move.x) != Mathf.Sign(m_Rigidbody.velocity.x)) )
        {
            if ((!m_IsStuckLeft && Mathf.Sign(m_Move.x) == -1) || (!m_IsStuckRight && Mathf.Sign(m_Move.x) == 1))
            {
                m_Rigidbody.AddForce(m_Move * m_Accelerate);
            }
        }
        
        m_IsStuckRight = false;
        m_IsStuckLeft = false;

        if (m_IsDahing)
            Dash();

        if (m_IsJumping)
            Jump();
    }

    public void OnMovement(InputAction.CallbackContext _context)
    {
        Vector2 move = _context.ReadValue<Vector2>();
        m_Move = new Vector3(move.x , 0, 0);
    }

    public void OnDash(InputAction.CallbackContext _context)
    {
        if (_context.ReadValueAsButton() == true && m_DashCd <= 0)
        {
            m_IsDahing = true;
            m_DashCd = 2.0f;
        }
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (_context.ReadValueAsButton() == true && (m_IsGrounded == true))
        {
            m_IsJumping = true;
        }
    }

    public void Jump()
    {
        m_Rigidbody.AddForce(new Vector3(0, m_JumpHeight, 0), ForceMode.Impulse);
        m_IsJumping = false;
    }
    public void Dash()
    {
        m_Rigidbody.AddForce(m_LastMove * m_DashPower, ForceMode.Impulse);
        m_IsDahing = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts) 
        {
            if (Vector3.Angle(contact.normal, Vector3.up) == 90f)
            {
                m_IsStuckLeft = Mathf.Sign(contact.normal.x) == 1;
                m_IsStuckRight = Mathf.Sign(contact.normal.x) == -1;
                break;
            }
        }
    }
}