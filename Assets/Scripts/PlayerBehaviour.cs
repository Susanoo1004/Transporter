using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerBehaviour : MonoBehaviour
{
    BoxCollider m_Collider;

    [Header("Body")]
    [SerializeField]
    private Transform m_Feet;
    [SerializeField]
    private Transform m_PlayerModel;
    private Vector3 m_ModelBaseLocalScale;

    [SerializeField]
    private float m_InvicibilityTime;
    [HideInInspector]
    public float m_InvicibilityTimer;

    [SerializeField]
    private Transform m_Arm;

    // Arm
    private float m_ResetArmPos = 0.75f;
    private bool m_ResetArm;
    private Vector3 m_ArmBaseLocalScale;

    [Header("Movement")]
    [SerializeField]
    private float m_Accelerate;

    public float MaxSpeed;

    [SerializeField]
    private float m_DashPower;

    [SerializeField]
    private float m_DashDistance;
    [SerializeField]
    private float m_DashTime;
    [HideInInspector]
    public float DashTimer;
    private bool m_IsDashing = false;
    private bool m_CanDash = true;
    private Vector3 m_PositionBeforeDash;

    [SerializeField]
    private float m_JumpForce;

    private bool m_IsJumping = false;


    [HideInInspector]
    public Vector3 Move = new();
    private Vector3 m_LastMove = Vector3.right;

    [HideInInspector]
    public byte PlayerLife = 10;

    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private Animator m_ArmAnimator;
    private PlayerInput m_PlayerInput;

    private bool m_IsStuckLeft;
    private bool m_IsStuckRight;

    [HideInInspector]
    public bool m_HasTakenExplosion;

    [Header("Magnet")]
    [SerializeField]
    private Transform m_Magnet;
    [SerializeField]
    private Transform m_MagnetOnArmTransform;
    private MagnetBehaviour m_MagnetBehaviour;

    [SerializeField]
    private float m_MagnetCooldownTime;
    private float m_MagnetCooldownTimer;

    [SerializeField]
    private float m_HoverTime;

    [SerializeField]
    private float m_AttractionTime;

    [SerializeField]
    private float m_PlayerAttractionSpeed;

    [SerializeField]
    private float m_RepulsiveForce;

    [SerializeField]
    private float m_ThrowForce;

    [SerializeField]
    private float m_ThrowTime;

    [SerializeField]
    private float m_PullTime;

    [SerializeField]
    private float m_PlayerToMagnetDistance;

    private Vector2 m_Aim = new Vector2(1, 0);

    [SerializeField]
    private AudioSource m_MagnetSwitchSound;

    [SerializeField]
    private AudioClip[] m_MagnetSwitchList;

    [SerializeField]
    private AudioSource m_PlayerDash;

    [SerializeField]
    private AudioClip[] m_PlayerDashList;

    [SerializeField]
    private AudioSource m_LifeLose;

    [SerializeField]
    private AudioSource m_MagnetPush;

    [SerializeField]
    private AudioClip[] m_MagnetPushList;

    [SerializeField]
    private AudioSource m_MagnetBack;

    [SerializeField]
    private AudioSource m_MagnetThrow;

    [SerializeField]
    private AudioSource m_PlayerJump;

    [SerializeField]
    private AudioClip[] m_PlayerJumpList;

    [HideInInspector]
    public Vector3 SurfaceNormal;

    private bool m_BufferThrow;

    [HideInInspector]
    public Vector3 CurrentCheckpoint = Vector3.zero;

    private float m_DeathTimer;
    private float m_DeathTime = 2.0f;

    private bool HasMagnet { get { return m_Magnet.parent == transform; } }

    public bool IsGrounded
    {
        get
        {
            return Physics.Raycast(m_Feet.transform.position, Vector3.down, 0.025f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.size.z / 2, 0, 0), Vector3.down, 0.025f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.size.z / 2, 0, 0), Vector3.down, 0.025f);
        }
    }

    private GameObject m_StandingOnObject
    {
        get
        {
            bool center = Physics.Raycast(m_Feet.transform.position, Vector3.down, out RaycastHit CenterHit, 0.025f);
            bool left = Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.size.z / 2, 0, 0), Vector3.down, out RaycastHit LeftHit, 0.025f);
            bool right = Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.size.z / 2, 0, 0), Vector3.down, out RaycastHit RightHit, 0.025f);

            return center ? CenterHit.transform.gameObject : right ? RightHit.transform.gameObject : left ? LeftHit.transform.gameObject : null;
        }
    }


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        m_MagnetBehaviour = m_Magnet.GetComponent<MagnetBehaviour>();
        m_ArmAnimator = m_Arm.GetComponentInChildren<Animator>();
        m_PlayerInput = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_ModelBaseLocalScale = m_PlayerModel.localScale;
        m_InvicibilityTimer = m_InvicibilityTime;
        m_ArmBaseLocalScale = m_Arm.localScale;
        m_DeathTimer = m_DeathTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat("SpeedX", Mathf.Abs(m_Rigidbody.velocity.x / 2));
        m_Animator.SetFloat("SpeedY", m_Rigidbody.velocity.y / 2);
        m_Animator.SetBool("Jump", m_IsJumping);
        m_ArmAnimator.SetBool("Jump", m_IsJumping);

        {
            Vector3 direction = m_Magnet.position - m_Arm.position;

            Quaternion ToRotation = Quaternion.LookRotation(direction, Vector3.Cross(direction, Vector3.forward));
            m_Arm.rotation = ToRotation;
            m_Arm.Rotate(transform.forward, 90);
        }

        // Rotation in every case
        // Player magnetized towards MagneticObject
        if (m_MagnetBehaviour.IsPlayerMagnetized && !m_MagnetBehaviour.IsPlayerAttached)
        {
            Vector3 direction = m_Magnet.position - transform.position;

            Quaternion ToRotation = Quaternion.LookRotation(direction, Vector3.Cross(direction, Vector3.forward));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ToRotation, 1080 * Time.deltaTime);
        }
        // Player attached to magnetic object
        else if (!m_MagnetBehaviour.IsPlayerMagnetized && m_MagnetBehaviour.IsPlayerAttached)
        {
            Vector3 normal = m_MagnetBehaviour.GetPlayerAttachedObjectNormal;
            if (normal != Vector3.zero)
            {
                Quaternion ToRotation = Quaternion.LookRotation(Vector3.Cross(normal, Vector3.forward), normal);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, ToRotation, 1080 * Time.deltaTime);
            }
        }
        // else
        else
        {
            if (Move != Vector3.zero)
                m_LastMove = Move;
            Quaternion ToRotation = Quaternion.LookRotation(m_LastMove, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ToRotation, 1080);
        }


        if (m_Aim == Vector2.zero)
            m_MagnetBehaviour.MagnetDefaultPositions = m_MagnetOnArmTransform.position;
        else
            m_MagnetBehaviour.MagnetDefaultPositions = new Vector3(m_Arm.position.x, m_Arm.position.y, 0) + (Vector3)m_Aim * m_PlayerToMagnetDistance;

        if (HasMagnet)
            m_Magnet.position = m_MagnetBehaviour.MagnetDefaultPositions;

        if (DashTimer > 0)
            DashTimer -= Time.deltaTime;

        if (m_InvicibilityTimer > 0)
            m_InvicibilityTimer -= Time.deltaTime;

        if (m_MagnetCooldownTimer > 0)
            m_MagnetCooldownTimer -= Time.deltaTime;


        if (!m_MagnetBehaviour.IsPlayerMagnetized && !HasMagnet && m_MagnetBehaviour.HoverTimer <= 0)
        {
            if (!m_MagnetBehaviour.IsPlayerAttached)
            {
                if (m_MagnetBehaviour.IsThrowing)
                    SetPullProperties();
                else if (m_MagnetBehaviour.TravelTimer <= 0)
                    AttachMagnet();
            }
            else
            {
                AttachMagnet();
            }
        }

        if ((IsGrounded || m_MagnetBehaviour.IsPlayerAttached) && HasMagnet && DashTimer <= 0)
            m_CanDash = true;

        if (m_BufferThrow && m_MagnetCooldownTimer <= 0 && DashTimer <= 0)
            MagnetThrow();

        if (transform.rotation.y > 0)
        {
            m_Arm.localScale = m_ArmBaseLocalScale * -1;
        }
        else if (transform.rotation.y < 0)
        {
            m_Arm.localScale = m_ArmBaseLocalScale;
        }

        if (PlayerLife == 0)
        {
            m_Animator.Play("Dead");
            m_Arm.gameObject.SetActive(false);
            m_PlayerInput.SwitchCurrentActionMap("Menu");

            m_DeathTimer -= Time.deltaTime;

            if (m_DeathTimer > 0)
                return;

            m_DeathTimer = m_DeathTime;
            transform.position = CurrentCheckpoint;
            m_PlayerInput.SwitchCurrentActionMap("Gameplay");
            m_Animator.Play("Movement");
            PlayerLife = 10;
            m_Arm.gameObject.SetActive(true);

        }

        if (m_ResetArm)
        {
            m_ResetArmPos -= Time.deltaTime;
            if (m_ResetArmPos <= 0)
            {
                m_Arm.position -= Vector3.down / 3.5f;
                m_ResetArmPos = 0.75f;
                m_ResetArm = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_InvicibilityTimer > 0)
            m_PlayerModel.localScale = m_PlayerModel.localScale == Vector3.zero ? m_ModelBaseLocalScale : Vector3.zero;
        else
            m_PlayerModel.localScale = m_ModelBaseLocalScale;

        if (!m_MagnetBehaviour.IsPlayerMagnetized && !m_MagnetBehaviour.IsPlayerAttached)
        {
            if ((m_Rigidbody.velocity.x >= -MaxSpeed && m_Rigidbody.velocity.x <= MaxSpeed) || (Mathf.Sign(Move.x) != Mathf.Sign(m_Rigidbody.velocity.x)))
            {
                if ((!m_IsStuckLeft && Mathf.Sign(Move.x) == -1) || (!m_IsStuckRight && Mathf.Sign(Move.x) == 1) || IsGrounded)
                    m_Rigidbody.AddForce(Move * m_Accelerate * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }
        }
        else if (!m_MagnetBehaviour.IsPlayerMagnetized && m_MagnetBehaviour.IsPlayerAttached)
        {
            if (m_MagnetBehaviour.PlayerAttachedObject.TryGetComponent(out Collider collider) && transform.TryGetComponent(out BoxCollider boxCollider))
                transform.position = collider.ClosestPointOnBounds(transform.position) + m_MagnetBehaviour.GetPlayerAttachedObjectNormal * boxCollider.size.y / 2;
            m_Rigidbody.velocity = Vector3.zero;
        }

        m_IsStuckRight = false;
        m_IsStuckLeft = false;

        if (m_IsDashing || DashTimer > 0)
            Dash();

        if (IsGrounded)
        {
            if (m_StandingOnObject.TryGetComponent(out Rigidbody rigidbody))
            {
                m_Rigidbody.velocity += rigidbody.velocity * Time.fixedDeltaTime;
            }
            else
            {
                if (Move == Vector3.zero)
                    m_Rigidbody.AddForce(new Vector3(-m_Rigidbody.velocity.x / 5.0f, 0.0f, 0.0f), ForceMode.VelocityChange);
            }

            m_Animator.SetBool("Landed", true);

        }
        else
        {
            m_Animator.SetBool("Landed", false);
        }

        if (!IsGrounded && !m_MagnetBehaviour.IsPlayerAttached)
            m_Rigidbody.velocity += (Vector3.down/10) * m_GravityMultiplier;
    }

    public void OnMovement(InputAction.CallbackContext _context)
    {
        Vector2 move = _context.ReadValue<Vector2>();
        Move = new Vector3(move.x, 0, 0);
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (_context.started && IsGrounded == true)
        {
            m_IsJumping = true;
            m_ResetArm = true;
            m_Rigidbody.AddForce(Vector3.up * m_JumpForce, ForceMode.VelocityChange);
            m_Animator.Play("Jump");
            m_Arm.position += Vector3.down / 3.5f;
            play_PlayerJump();
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
            m_BufferThrow = true;
            m_IsDashing = true;
        }
        if (_context.canceled)
        {
            m_BufferThrow = false;
            m_IsDashing = false;
        }
    }

    private void MagnetThrow()
    {
        if (HasMagnet)
        {
            if (m_MagnetBehaviour.IsPlayerAttached)
                DettachPlayer();
            
            if (m_MagnetBehaviour.HasMagnetizedObject)
                ThrowMagnetizedObject();
            else
                SetThrowProperties();
        }
        else if (m_MagnetBehaviour.TravelTimer <= 0)
        {
            SetPullProperties();
        }

        m_MagnetCooldownTimer = m_MagnetCooldownTime;
        m_Magnet.position = new Vector3(m_Magnet.position.x, m_Magnet.position.y, 0);
    }

    

    public void OnChangePolarity(InputAction.CallbackContext _context)
    {
        if (_context.started && HasMagnet)
        {
            m_MagnetBehaviour.IsPositive = !m_MagnetBehaviour.IsPositive;

            if (m_MagnetBehaviour.IsPlayerAttached)
                DettachPlayer();

            if (m_MagnetBehaviour.HasMagnetizedObject)
                DropMagnetizedObject();

            m_MagnetBehaviour.IgnoreObject = null;


            //ms : son changement polarity
           play_MagnetSwitch();
          

        }
        
    }

    

    public void OnAim(InputAction.CallbackContext _context)
    {
        if (m_PlayerInput.defaultActionMap == "Keyboard")
            m_Aim = (_context.ReadValue<Vector2>() - new Vector2(Screen.width / 2f, Screen.height / 2f)).normalized;
        else
            m_Aim = _context.ReadValue<Vector2>().normalized;
    }

    public void Dash()
    {
        if (DashTimer > 0)
        {
            if (!TryGetComponent(out BoxCollider boxCollider))
                return;
            Vector3 distance = m_Magnet.position - transform.position;
            Vector3 direction = distance.normalized;
            Vector3 halfHeight = Vector3.up * boxCollider.size.y / 2 * 0.9f;
            int layer = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Magnet"));

            if (Physics.CapsuleCast(m_Magnet.position + halfHeight, transform.position - halfHeight, boxCollider.size.z / 2, direction, distance.magnitude, layer))
            {
                DashTimer = 0;
                return;
            }

            m_MagnetCooldownTimer = m_MagnetCooldownTime;
            transform.position = Vector3.Lerp(m_PositionBeforeDash, m_Magnet.position, 1 - DashTimer / m_DashTime);
            m_Rigidbody.velocity = Vector3.zero;


            // ms : son Dash
            play_PlayerDash();


            return;
        }

        else if (m_CanDash && (m_Magnet.position - transform.position).magnitude >= m_DashDistance + m_PlayerToMagnetDistance) //&& (m_Magnet.position - transform.position).magnitude <= m_DashDistance +1)
        {
            m_CanDash = false;
            DashTimer = m_DashTime;
            m_MagnetBehaviour.HoverTimer = m_DashTime;
            m_MagnetBehaviour.TravelTimer = 0;
            m_PositionBeforeDash = transform.position;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // wall Collision
            if (Vector3.Angle(contact.normal, transform.up) == 90f)
            {
                m_IsStuckLeft = Mathf.Sign(contact.normal.x) == 1;
                m_IsStuckRight = Mathf.Sign(contact.normal.x) == -1;
                break;
            }
        }
    }

    private void SetThrowProperties()
    {
        m_Magnet.SetParent(null, true);
        m_Magnet.transform.rotation = new Quaternion(0, 0, 0, 0);
        m_Magnet.GetComponent<BoxCollider>().enabled = true;
        m_Magnet.GetComponent<SphereCollider>().enabled = true;
        m_Magnet.GetComponent<Rigidbody>().isKinematic = false;
        //m_Magnet.GetComponent<Rigidbody>().velocity = m_Rigidbody.velocity;

        m_MagnetBehaviour.Aim = new Vector3(m_Aim.x, m_Aim.y, 0);
        m_MagnetBehaviour.TravelTimer = m_ThrowTime;
        m_MagnetBehaviour.HoverTimer = m_HoverTime;
        m_MagnetBehaviour.AttractionTime = m_AttractionTime;
        m_MagnetBehaviour.PlayerAttractionSpeed = m_PlayerAttractionSpeed;
        m_MagnetBehaviour.IsThrowing = true;
        m_MagnetBehaviour.ThrowForce = m_ThrowForce;
        m_MagnetBehaviour.PlayerThrowForce = m_Rigidbody.velocity;
        m_MagnetBehaviour.RepulsiveForce = m_RepulsiveForce;

        //son de MagnetThrow (lancement de l'aimant)
        m_MagnetThrow.Play();
    }

    private void SetPullProperties()
    {
        m_Magnet.GetComponent<BoxCollider>().enabled = false;
        m_Magnet.GetComponent<SphereCollider>().enabled = false;
        m_Magnet.GetComponent<Rigidbody>().isKinematic = true;

        m_MagnetBehaviour.PullTime = m_PullTime;
        m_MagnetBehaviour.TravelTimer = m_PullTime;
        m_MagnetBehaviour.IsThrowing = false;
        m_MagnetBehaviour.HoverTimer = 0;
        m_MagnetBehaviour.IgnoreObject = null;
        m_MagnetBehaviour.LastPosition = m_Magnet.position;

        //son MagnetBack
        m_MagnetBack.Play();
    }

    private void AttachMagnet()
    {
        m_Magnet.GetComponent<Rigidbody>().isKinematic = true;
        m_Magnet.position = new Vector3(m_Magnet.position.x, m_Magnet.position.y, 0);
        m_Magnet.SetParent(transform, true);
                
    }

    private void ThrowMagnetizedObject()
    {

        if (m_MagnetBehaviour.IsPlayerMagnetized)
        {
            m_MagnetBehaviour.MagnetizedObject = null;
            m_MagnetBehaviour.IsPlayerMagnetized = false;
        }
        else
        {
            m_MagnetBehaviour.MagnetizedObject.SetParent(null, true);
            m_MagnetBehaviour.MagnetizedObject.position = new Vector3(m_MagnetBehaviour.MagnetizedObject.position.x,
                                                                      m_MagnetBehaviour.MagnetizedObject.position.y,
                                                                      0);
            m_MagnetBehaviour.MagnetizedObject.rotation = new Quaternion(0, 0, 0, 0);
            //son
            play_MagnetPush();

            if (m_MagnetBehaviour.MagnetizedObject.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false;
                rigidbody.velocity = m_Rigidbody.velocity;
                rigidbody.AddForce(m_Aim * m_RepulsiveForce, ForceMode.VelocityChange);
            }
            if (m_MagnetBehaviour.MagnetizedObject.TryGetComponent(out Collider collider))
                collider.isTrigger = false;

            m_MagnetBehaviour.MagnetizedObject = null;
        }
    }

    private void DropMagnetizedObject()
    {
        m_MagnetBehaviour.MagnetizedObject.SetParent(null, true);
        m_MagnetBehaviour.MagnetizedObject.position = new Vector3(m_MagnetBehaviour.MagnetizedObject.position.x,
                                                                  m_MagnetBehaviour.MagnetizedObject.position.y,
                                                                  0);
        m_MagnetBehaviour.MagnetizedObject.rotation = new Quaternion(0, 0, 0, 0);

        if (m_MagnetBehaviour.MagnetizedObject.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.velocity = m_Rigidbody.velocity;
            rigidbody.isKinematic = false;
        }
        if (m_MagnetBehaviour.MagnetizedObject.TryGetComponent(out Collider collider))
            collider.isTrigger = false;


        m_MagnetBehaviour.MagnetizedObject = null;
    }

    private void DettachPlayer()
    {
        m_MagnetBehaviour.IsPlayerAttached = false;
        m_MagnetBehaviour.PlayerAttachedObject = null;
        m_Rigidbody.useGravity = true;
        //son de player quand il se d�tache de la platforme
    }

    public void TakeDamage(byte damage)
    {
        if (PlayerLife < damage)
            PlayerLife = 0;
        else
            PlayerLife -= damage;
        m_InvicibilityTimer = m_InvicibilityTime;
        m_Animator.Play("Hurt");

        m_LifeLose.Play();
    }

    public void play_MagnetSwitch()
    {

        int index = Random.Range(0, m_MagnetSwitchList.Length);
        m_MagnetSwitchSound.PlayOneShot(m_MagnetSwitchList[index]);
    }

    public void play_PlayerDash()
    {

        int index = Random.Range(0, m_PlayerDashList.Length);
        m_PlayerDash.PlayOneShot(m_PlayerDashList[index]);
    }

    public void play_MagnetPush()
    {

        int index = Random.Range(0, m_MagnetPushList.Length);
        m_MagnetPush.PlayOneShot(m_MagnetPushList[index]);
    }

    public void play_PlayerJump()
    {

        int index = Random.Range(0, m_PlayerJumpList.Length);
        m_PlayerJump.PlayOneShot(m_PlayerJumpList[index]);
    }
}
