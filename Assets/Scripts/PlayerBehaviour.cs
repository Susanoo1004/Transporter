using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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

    private bool m_IsDashing = false;

    private bool m_IsJumping = false;

    [Header("Magnet")]
    [SerializeField]
    private Transform m_Magnet;
    private MagnetBehaviour m_MagnetBehaviour;

    [SerializeField]
    private float m_HoverTime;

    [SerializeField]
    private float m_ThrowDistance;

    [SerializeField]
    private float m_ThrowTime;

    [SerializeField]
    private float m_PullTime;


    private bool HasMagnet { get { return m_Magnet.parent == transform; } }

    ////////////

    private Vector2 m_Aim;

    private bool IsGrounded { get {
            return Physics.Raycast(m_Feet.transform.position, Vector3.down, 0.05f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.radius, 0, 0), Vector3.down, 0.05f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.radius, 0, 0), Vector3.down, 0.05f); } }

    private bool m_IsStuckLeft;
    private bool m_IsStuckRight;

    private GameObject m_StandingOnObject { get {
        bool center = Physics.Raycast(m_Feet.transform.position, Vector3.down, out RaycastHit CenterHit, 0.05f);
        bool left = Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.radius, 0, 0), Vector3.down, out RaycastHit LeftHit, 0.05f);
        bool right = Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.radius, 0, 0), Vector3.down, out RaycastHit RightHit, 0.05f);
        
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
    }

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<CapsuleCollider>();
        m_MagnetBehaviour = m_Magnet.GetComponent<MagnetBehaviour>();
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

        if (m_DashCooldown > 0)
            m_DashCooldown -= Time.deltaTime;

        if (m_JumpTimer > 0)
            m_JumpTimer -= Time.deltaTime;
        /*
        if (!HasMagnet && m_MagnetBehaviour.TravelTimer <= 0 && m_MagnetBehaviour.HoverTimer <= 0)
        {
            m_Magnet.GetComponent<BoxCollider>().enabled = false;
            m_Magnet.GetComponent<Rigidbody>().isKinematic = true;
            m_MagnetBehaviour.TravelTime = m_PullTime;
            m_MagnetBehaviour.TravelTimer = m_PullTime;
            m_MagnetBehaviour.HoverTimer = 0;
            
            m_Magnet.SetParent(transform, true);
            m_Magnet.localPosition = new Vector3(0f, 0.2f, 1f);
        }
            */
    }
    
    private void FixedUpdate()
    {
        if ((m_Rigidbody.velocity.x >= -m_MaxSpeed && m_Rigidbody.velocity.x <= m_MaxSpeed) || (Mathf.Sign(m_Move.x) != Mathf.Sign(m_Rigidbody.velocity.x)) )
        {
            if ((!m_IsStuckLeft && Mathf.Sign(m_Move.x) == -1) || (!m_IsStuckRight && Mathf.Sign(m_Move.x) == 1) || IsGrounded)
                m_Rigidbody.AddForce(m_Move * m_Accelerate, ForceMode.Acceleration);
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
        if (_context.ReadValueAsButton() == true)
        {
            if (IsGrounded == true)
            {
                m_Rigidbody.AddForce(Vector3.up * m_MinJumpForce, ForceMode.VelocityChange);
                m_JumpTimer = m_JumpTime;
                m_IsJumping = true; 
            }
        }
        else
        {
            m_IsJumping = false;
        }
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

                m_MagnetBehaviour.Target = transform.position + new Vector3(m_Aim.x, m_Aim.y, 0) * m_ThrowDistance;
                m_MagnetBehaviour.TravelTime = m_ThrowTime;
                m_MagnetBehaviour.TravelTimer = m_ThrowTime;
                m_MagnetBehaviour.HoverTimer = m_HoverTime;
                m_MagnetBehaviour.IsThrowing = true;
            }
            else
            {
                m_Magnet.GetComponent<BoxCollider>().enabled = false; 
                m_MagnetBehaviour.TravelTime = m_PullTime;
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

        if (m_JumpTimer < 0)
            m_IsJumping = false;
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