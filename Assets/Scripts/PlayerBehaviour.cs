using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    BoxCollider m_Collider;

    [Header("Body")]
    [SerializeField]
    private Transform m_Feet;

    [Header("Movement")]
    [SerializeField]
    private float m_Accelerate;

    [SerializeField]
    private float m_MaxSpeed;

    [SerializeField]
    private float m_DashPower;

    [SerializeField]
    private float m_DashCooldownDuration;
    private float m_DashCooldown;

    [SerializeField]
    private float m_MinJumpForce;

    [SerializeField]
    private float m_MaxJumpForce;

    [SerializeField]
    private float m_JumpTime;
    private float m_JumpTimer;

    private bool m_IsJumping = false;
    private bool m_CanDoubleJump;

    private bool m_IsDashing = false;
    private float m_DashCd;

    private Vector3 m_Move = new();
    private Vector3 m_LastMove = new();

    [HideInInspector] 
    public byte PlayerLife = 10;

    // To move into UI
    [SerializeField]
    TMP_Text m_PlayerLifeText;

    private Animator m_Animator;

    private Rigidbody m_Rigidbody;

    private bool m_IsStuckLeft;
    private bool m_IsStuckRight;
    [HideInInspector]
    public bool m_HasTakenExplosion;

    [Header("Magnet")]
    [SerializeField]
    private Transform m_Magnet;
    private MagnetBehaviour m_MagnetBehaviour;

    [SerializeField]
    private float m_HoverTime;

    [SerializeField]
    private float m_ThrowForce;

    [SerializeField]
    private float m_ThrowTime;

    [SerializeField]
    private float m_PullTime;

    private Vector2 m_Aim;

    private bool HasMagnet { get { return m_Magnet.parent == transform; } }

    private bool IsGrounded { get {
            return Physics.Raycast(m_Feet.transform.position, Vector3.down, 0.05f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.size.z/2, 0, 0), Vector3.down, 0.05f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.size.z/2, 0, 0), Vector3.down, 0.05f); } }

    private GameObject m_StandingOnObject { get {
        bool center = Physics.Raycast(m_Feet.transform.position, Vector3.down, out RaycastHit CenterHit, 0.05f);
        bool left = Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.size.z/2, 0, 0), Vector3.down, out RaycastHit LeftHit, 0.05f);
        bool right = Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.size.z/2, 0, 0), Vector3.down, out RaycastHit RightHit, 0.05f);
        
          /*
        if (center)
            return CenterHit.transform.gameObject;
        if (left)
            return LeftHit.transform.gameObject;
        if (right)
            return RightHit.transform.gameObject;
        return null;
        */

        return center ? CenterHit.transform.gameObject : right ? RightHit.transform.gameObject : left ? LeftHit.transform.gameObject : null;
    } }


    private void OnValidate()
    {
        m_MagnetBehaviour = m_Magnet.GetComponent<MagnetBehaviour>();
        m_MagnetBehaviour.HoverTime = m_HoverTime;
        m_MagnetBehaviour.ThrowForce = m_ThrowForce;
    }

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        m_MagnetBehaviour = m_Magnet.GetComponent<MagnetBehaviour>();
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

        if (m_DashCooldown > 0)
            m_DashCooldown -= Time.deltaTime;

        if (m_JumpTimer > 0)
            m_JumpTimer -= Time.deltaTime;

        if (!HasMagnet && m_MagnetBehaviour.HoverTimer <= 0)
        {
            if (m_MagnetBehaviour.IsThrowing)
            {
                m_Magnet.GetComponent<BoxCollider>().enabled = false;
                m_MagnetBehaviour.PullTime = m_PullTime;
                m_MagnetBehaviour.TravelTimer = m_PullTime;
                m_MagnetBehaviour.IsThrowing = false;
            }
            else if (m_MagnetBehaviour.TravelTimer <= 0)
            {
                m_Magnet.GetComponent<Rigidbody>().isKinematic = true;
                m_Magnet.SetParent(transform, true);
                m_Magnet.localPosition = new Vector3(0f, 0.2f, 1f);
            }
        }
    }
    
    private void FixedUpdate()
    {
        if(m_HasTakenExplosion) 
        {
            if(IsGrounded) 
                m_HasTakenExplosion = false;
        }
        else
        {
            if ((m_Rigidbody.velocity.x >= -m_MaxSpeed && m_Rigidbody.velocity.x <= m_MaxSpeed) || (Mathf.Sign(m_Move.x) != Mathf.Sign(m_Rigidbody.velocity.x)))
            {
                   if ((!m_IsStuckLeft && Mathf.Sign(m_Move.x) == -1) || (!m_IsStuckRight && Mathf.Sign(m_Move.x) == 1) || IsGrounded)
                    {
                        m_Rigidbody.AddForce(m_Move * m_Accelerate * Time.fixedDeltaTime, ForceMode.VelocityChange);

                        //Vector3 Velocity = new(m_Move.x * m_Accelerate, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
                        //m_Rigidbody.velocity = Velocity;
                    }
            }
        }
        
        m_IsStuckRight = false;
        m_IsStuckLeft = false;

        if (m_IsDashing)
            Dash();

        if (m_IsJumping)
            Jump();

        if (IsGrounded)
        {
            Rigidbody rigidbody = m_StandingOnObject.GetComponent<Rigidbody>();
            if (rigidbody)
                m_Rigidbody.velocity += rigidbody.velocity * Time.fixedDeltaTime;

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
        if (_context.ReadValueAsButton() == true && m_DashCooldown <= 0)
        {
            m_IsDashing = true;
            m_DashCooldown = m_DashCooldownDuration;
        }
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (_context.started && (IsGrounded == true || m_CanDoubleJump == true))
        {
            if (!IsGrounded)
            {
                Vector3 velocity = m_Rigidbody.velocity;
                velocity.y = 0;
                m_Rigidbody.velocity = velocity;
                m_CanDoubleJump = false;
            }

            m_Rigidbody.AddForce(Vector3.up * m_MinJumpForce, ForceMode.VelocityChange);
            m_JumpTimer = m_JumpTime;
        }

        if (_context.ReadValueAsButton() == true && m_JumpTimer > 0)
            m_IsJumping = true;
        else
            m_IsJumping = false;
    }

    public void OnMagnetThrow(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            if (HasMagnet)
            {
                m_Magnet.transform.localPosition = new Vector3(0f, -0.2f, 0f);
                m_Magnet.transform.rotation = new Quaternion(0, 0, 0, 0);
                m_Magnet.SetParent(null, true);
                m_Magnet.GetComponent<BoxCollider>().enabled = true;
                m_Magnet.GetComponent<Rigidbody>().isKinematic = false;
                m_Magnet.GetComponent<Rigidbody>().velocity = m_Rigidbody.velocity;

                m_MagnetBehaviour.Aim = new Vector3(m_Aim.x, m_Aim.y, 0);
                m_MagnetBehaviour.TravelTimer = m_ThrowTime;
                m_MagnetBehaviour.HoverTimer = m_HoverTime;
                m_MagnetBehaviour.IsThrowing = true;
                m_MagnetBehaviour.ThrowForce = m_ThrowForce;
            }
            else if (m_MagnetBehaviour.TravelTimer <= 0)
            {
                m_Magnet.GetComponent<BoxCollider>().enabled = false; 
                m_MagnetBehaviour.PullTime = m_PullTime;
                m_MagnetBehaviour.TravelTimer = m_PullTime;
                m_MagnetBehaviour.IsThrowing = false;
                m_MagnetBehaviour.HoverTimer = 0;
            }
        }
    }

    public void OnAim(InputAction.CallbackContext _context)
    {
        if (GetComponent<PlayerInput>().defaultActionMap == "Keyboard")
            m_Aim =  (_context.ReadValue<Vector2>() - new Vector2(Screen.width / 2f, Screen.height / 2f)).normalized;
        else
            m_Aim = _context.ReadValue<Vector2>().normalized;
    }

    public void Jump()
    {
        m_Rigidbody.AddForce(Vector3.up * m_MaxJumpForce * 3f * (m_JumpTimer/(m_JumpTime*1.2f)) * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }

    public void Dash()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.AddForce(m_LastMove * m_DashPower, ForceMode.VelocityChange);
        m_IsDashing = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts) 
        {
            // wall Collision
            if (Vector3.Angle(contact.normal, Vector3.up) == 90f)
            {
                m_IsStuckLeft = Mathf.Sign(contact.normal.x) == 1;
                m_IsStuckRight = Mathf.Sign(contact.normal.x) == -1;
                break;
            }
        }
    }
}
