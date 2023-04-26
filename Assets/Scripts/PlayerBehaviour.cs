using TMPro;
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

    [Header("Movement")]
    [SerializeField]
    private float m_Accelerate;

    [SerializeField]
    private float m_MaxSpeed;

    [SerializeField]
    private float m_DashPower;

    [SerializeField]
    private float m_DashDistance;
    [SerializeField]
    private float m_DashTime;
    private float m_DashTimer;
    private bool m_IsDashing = false;
    private bool m_CanDash = true;
    private Vector3 m_PositionBeforeDash;

    [SerializeField]
    private float m_MinJumpForce;

    [SerializeField]
    private float m_MaxJumpForce;

    [SerializeField]
    private float m_JumpTime;
    private float m_JumpTimer;

    private bool m_IsJumping = false;
    private bool m_CanDoubleJump;


    private Vector3 m_Move = new();
    private Vector3 m_LastMove = Vector3.right;

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
    [SerializeField]
    private Transform m_MagnetOnArmTransform;
    private MagnetBehaviour m_MagnetBehaviour;

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

    private Vector2 m_Aim = new Vector2(1,0);
    
    [HideInInspector]
    public Vector3 SurfaceNormal;

    private bool HasMagnet { get { return m_Magnet.parent == transform; } }

    public float m_AnimatorCancelTimer = 0.1f;

    public bool IsGrounded
    {
        get
        {
            return Physics.Raycast(m_Feet.transform.position, Vector3.down, 0.025f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.size.x / 2, 0, 0), Vector3.down, 0.025f)
                || Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.size.x / 2, 0, 0), Vector3.down, 0.025f);
        }
    }

    private GameObject m_StandingOnObject
    {
        get
        {
            bool center = Physics.Raycast(m_Feet.transform.position, Vector3.down, out RaycastHit CenterHit, 0.05f);
            bool left = Physics.Raycast(m_Feet.transform.position + new Vector3(-m_Collider.size.z / 2, 0, 0), Vector3.down, out RaycastHit LeftHit, 0.05f);
            bool right = Physics.Raycast(m_Feet.transform.position + new Vector3(m_Collider.size.z / 2, 0, 0), Vector3.down, out RaycastHit RightHit, 0.05f);

            return center ? CenterHit.transform.gameObject : right ? RightHit.transform.gameObject : left ? LeftHit.transform.gameObject : null;
        }
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
        m_ModelBaseLocalScale = m_PlayerModel.localScale;
        m_InvicibilityTimer = m_InvicibilityTime;
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat("SpeedX", Mathf.Abs(m_Rigidbody.velocity.x/2));
        m_Animator.SetFloat("SpeedY", m_Rigidbody.velocity.y/2);
        m_Animator.SetBool("Jump", m_IsJumping);

        m_PlayerLifeText.text = "Player Life Point : " + PlayerLife;

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
            if (m_Move != Vector3.zero)
                m_LastMove = m_Move;
            Quaternion ToRotation = Quaternion.LookRotation(m_LastMove, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, ToRotation, 1080);
        }


        if (m_Aim == Vector2.zero)
            m_MagnetBehaviour.MagnetDefaultPositions = m_MagnetOnArmTransform.position;
        else
            m_MagnetBehaviour.MagnetDefaultPositions = transform.position + (Vector3)m_Aim * m_PlayerToMagnetDistance;
        if (HasMagnet)
            m_Magnet.position = m_MagnetBehaviour.MagnetDefaultPositions;

        if (m_JumpTimer > 0)
            m_JumpTimer -= Time.deltaTime;

        if ((IsGrounded || m_MagnetBehaviour.IsPlayerAttached) && HasMagnet && m_DashTimer <= 0)
            m_CanDash = true;

        if (m_DashTimer > 0)
            m_DashTimer -= Time.deltaTime;

        if (m_InvicibilityTimer > 0)
            m_InvicibilityTimer -= Time.deltaTime;
        

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

        if (PlayerLife == 0)
        {
            Destroy(gameObject);
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
                        m_Rigidbody.AddForce(m_Move * m_Accelerate * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }
            }
        }
        else if (!m_MagnetBehaviour.IsPlayerMagnetized && m_MagnetBehaviour.IsPlayerAttached)
        {
            if (m_MagnetBehaviour.PlayerAttachedObject.TryGetComponent(out Collider collider) && transform.TryGetComponent(out BoxCollider boxCollider))
                transform.position = collider.ClosestPointOnBounds(transform.position) + m_MagnetBehaviour.GetPlayerAttachedObjectNormal * boxCollider.size.y / 2;
            m_Rigidbody.velocity = Vector3.zero;
            /*
            if (m_MagnetBehaviour.PlayerAttachedObject.TryGetComponent(out Collider collider))
                transform.position = collider.ClosestPoint(transform.position) + m_MagnetBehaviour.GetPlayerAttachedObjectNormal * boxCollider.size.y / 2;
            */

            /*  MOUVEMENT
            Vector3 direction = Vector3.Cross(m_MagnetBehaviour.GetPlayerAttachedObjectNormal, Vector3.forward).normalized;
            m_Rigidbody.velocity = direction * m_Move.magnitude * m_MaxSpeed * 0.5f * Time.fixedDeltaTime;
            */
        }

        m_IsStuckRight = false;
        m_IsStuckLeft = false;

        if (m_IsDashing || m_DashTimer > 0)
            Dash();

        if (m_IsJumping)
            Jump();



        if (IsGrounded)
        {
            if (m_StandingOnObject.TryGetComponent(out Rigidbody rigidbody))
                m_Rigidbody.velocity += rigidbody.velocity * Time.fixedDeltaTime;

            m_CanDoubleJump = true;

            m_Animator.SetBool("Landed", true);
        }
        else
        {
            m_Animator.SetBool("Landed", false);
        }
    }

    public void OnMovement(InputAction.CallbackContext _context)
    {
        Vector2 move = _context.ReadValue<Vector2>();
        m_Move = new Vector3(move.x, 0, 0);
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (_context.ReadValueAsButton() == true && m_JumpTimer > 0)
            m_IsJumping = true;
        else
            m_IsJumping = false;

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
    }

    public void OnMagnetThrow(InputAction.CallbackContext _context)
    {
        if (_context.started)
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
        }

        m_IsDashing = _context.ReadValueAsButton();
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
        }
    }

    public void OnAim(InputAction.CallbackContext _context)
    {
        if (GetComponent<PlayerInput>().defaultActionMap == "Keyboard")
            m_Aim = (_context.ReadValue<Vector2>() - new Vector2(Screen.width / 2f, Screen.height / 2f)).magnitude < m_PlayerToMagnetDistance ? Vector3.zero : (_context.ReadValue<Vector2>() - new Vector2(Screen.width / 2f, Screen.height / 2f)).normalized;
        else
            m_Aim = _context.ReadValue<Vector2>().normalized;
    }

    public void Jump()
    {
        m_Animator.Play("Jump");
        m_Rigidbody.AddForce(Vector3.up * m_MaxJumpForce * 3f * (m_JumpTimer / (m_JumpTime * 1.2f)) * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    public void Dash()
    {
        if (m_DashTimer > 0)
        {
            if (!TryGetComponent(out BoxCollider boxCollider))
                return;
            Vector3 distance = m_Magnet.position - transform.position;
            Vector3 direction = distance.normalized;
            Vector3 halfHeight = Vector3.up * boxCollider.size.y / 2 * 0.9f;
            int layer = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Magnet"));

            if (Physics.CapsuleCast(m_Magnet.position + halfHeight, transform.position - halfHeight, boxCollider.size.z / 2, direction, distance.magnitude, layer))
            {
                m_DashTimer = 0;
                return;
            }

            transform.position = Vector3.Lerp(m_PositionBeforeDash, m_Magnet.position, 1 - m_DashTimer / m_DashTime);
            m_Rigidbody.velocity = Vector3.zero;
            return;
        }

        if (m_CanDash && (m_Magnet.position - transform.position).magnitude >= m_DashDistance) //&& (m_Magnet.position - transform.position).magnitude <= m_DashDistance +1)
        {
            m_CanDash = false;
            m_IsDashing = false;
            m_DashTimer = m_DashTime;
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
        m_Magnet.transform.rotation = new Quaternion(0, 0, 0, 0);
        m_Magnet.SetParent(null, true);
        m_Magnet.GetComponent<BoxCollider>().enabled = true;
        m_Magnet.GetComponent<SphereCollider>().enabled = true;
        m_Magnet.GetComponent<Rigidbody>().isKinematic = false;
        m_Magnet.GetComponent<Rigidbody>().velocity = m_Rigidbody.velocity;

        m_MagnetBehaviour.Aim = new Vector3(m_Aim.x, m_Aim.y, 0);
        m_MagnetBehaviour.TravelTimer = m_ThrowTime;
        m_MagnetBehaviour.HoverTimer = m_HoverTime;
        m_MagnetBehaviour.AttractionTime = m_AttractionTime;
        m_MagnetBehaviour.PlayerAttractionSpeed = m_PlayerAttractionSpeed;
        m_MagnetBehaviour.IsThrowing = true;
        m_MagnetBehaviour.ThrowForce = m_ThrowForce;
        m_MagnetBehaviour.PlayerThrowForce = m_Rigidbody.velocity;
        m_MagnetBehaviour.RepulsiveForce = m_RepulsiveForce;
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
    }

    private void AttachMagnet()
    {
        m_Magnet.GetComponent<Rigidbody>().isKinematic = true;
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
            m_MagnetBehaviour.MagnetizedObject.rotation = new Quaternion(0,0,0,0);

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
    }

    public void TakeDamage(byte damage)
    {
        PlayerLife -= damage;
        m_InvicibilityTimer = m_InvicibilityTime;
    }
}
