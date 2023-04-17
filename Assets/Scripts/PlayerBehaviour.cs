using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Body")]
    [SerializeField]
    private Transform m_Feet;

    [Header("Speed")]
    [SerializeField]
    private float m_Accelerate;

    [SerializeField]
    private float m_DashPower;

    [SerializeField]
    private float m_JumpHeight;

    [SerializeField]
    private float m_MaxSpeed;

    [HideInInspector] 
    public byte PlayerLife = 10;

    private Animator m_Animator;

    private Rigidbody m_Rigidbody;

    private Vector3 m_Move = new();
    private Vector3 m_LastMove = new();

    private bool m_IsDahing = false;
    private float m_DashCd;

    private bool m_IsJumping = false;
    public bool m_CanDoubleJump;
    private bool m_IsGrounded { get { return Physics.Raycast(m_Feet.transform.position, Vector3.down, 0.2f); } }

    private bool m_IsStuckLeft;
    private bool m_IsStuckRight;
    [HideInInspector]
    public bool m_HasTakenExplosion;

    // To move into UI
    [SerializeField]
    TMP_Text m_PlayerLifeText;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerLifeText.text = "Player Life Point : ";
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat("SpeedX", m_Move.x);

        m_PlayerLifeText.text = "Player Life Point : " + PlayerLife;

        if (m_Move != Vector3.zero)
        {
            Quaternion ToRotation = Quaternion.LookRotation(m_Move, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ToRotation, 1080 * Time.deltaTime);
            m_LastMove = m_Move;
        }

        if (m_DashCd > 0)
        {
            m_DashCd -= Time.deltaTime;
        }
    }
    
    private void FixedUpdate()
    {
        if(m_HasTakenExplosion) 
        {
            if(m_IsGrounded) 
            {
                m_HasTakenExplosion = false;
            }
        }
        else
        {
            if ((m_Rigidbody.velocity.x >= -m_MaxSpeed && m_Rigidbody.velocity.x <= m_MaxSpeed) || (Mathf.Sign(m_Move.x) != Mathf.Sign(m_Rigidbody.velocity.x)))
            {
                if ((!m_IsStuckLeft && Mathf.Sign(m_Move.x) == -1) || (!m_IsStuckRight && Mathf.Sign(m_Move.x) == 1))
                {
                    Vector3 Velocity = new(m_Move.x * m_Accelerate, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
                    m_Rigidbody.velocity = Velocity;
                }
            }
        }
        
        m_IsStuckRight = false;
        m_IsStuckLeft = false;

        if (m_IsDahing)
            Dash();

        if (m_IsJumping)
            Jump();

        if (m_IsGrounded)
        {
            m_CanDoubleJump = true;
        }
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
        if (_context.ReadValueAsButton() == true && (m_IsGrounded == true || m_CanDoubleJump == true))
        {
            if (m_IsGrounded == false) 
            { 
                m_CanDoubleJump = false;
            }
            m_IsJumping = true;
        }
    }

    public void Jump()
    {
        Vector3 velocity = m_Rigidbody.velocity;
        velocity.y = 0;
        m_Rigidbody.velocity = velocity;
        m_Rigidbody.AddForce(new Vector3(0, m_JumpHeight + Mathf.Abs(m_Rigidbody.velocity.y), 0), ForceMode.VelocityChange);
        m_IsJumping = false;
    }

    public void Dash()
    {
        m_Rigidbody.AddForce(m_LastMove * m_DashPower, ForceMode.VelocityChange);
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