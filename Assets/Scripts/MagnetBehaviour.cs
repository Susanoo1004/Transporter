using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MagnetBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform m_Player;

    private Rigidbody m_Rigidbody;

    public bool IsPositive;

    [SerializeField]
    private Material m_PositiveMaterial;
    [SerializeField]
    private Material m_NegativeMaterial;

    //[HideInInspector]
    public float RepulsiveForce;
    //[HideInInspector]
    public float AttractionTime;
    //[HideInInspector]
    public float PlayerAttractionSpeed;
    //[HideInInspector]
    public float AttractionTimer;
    //[HideInInspector]
    public float ThrowForce;
    //[HideInInspector]
    public float HoverTime;
    //[HideInInspector]
    public float HoverTimer;
    //[HideInInspector]
    public float PullTime;
    //[HideInInspector]
    public float TravelTimer;
    //[HideInInspector]
    public bool IsThrowing;
    //[HideInInspector]
    public bool IsPlayerMagnatized;


    //[HideInInspector]
    public Transform MagnetizedObject;
    private Vector3 m_LastMagnetizedObjectPosition;

    private Vector3 m_LastPosition;
    private Vector3 m_LastPlayerPosition;

    [HideInInspector]
    public Vector3 Aim;


    private bool HasMagnet { get { return transform.parent == m_Player.transform; } }
    public bool HasMagnetizedObject { get { return MagnetizedObject != null; } }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        MagnetizedObject = null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnValidate()
    {
        if (IsPositive)
            GetComponent<Renderer>().material = m_PositiveMaterial;
        else
            GetComponent<Renderer>().material = m_NegativeMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPositive)
            GetComponent<Renderer>().material = m_PositiveMaterial;
        else
            GetComponent<Renderer>().material = m_NegativeMaterial;
    
        if (!IsPlayerMagnatized)
        {
            if (AttractionTimer > 0)
            {
                AttractionTimer -= Time.deltaTime;
            }

            if (TravelTimer > 0)
            {
                TravelTimer -= Time.deltaTime;
            }
            else if (HoverTimer > 0)
            {
                HoverTimer -= Time.deltaTime;
                m_LastPosition = transform.position;
            }
        }

        if (HasMagnetizedObject)
            m_LastMagnetizedObjectPosition = MagnetizedObject.position;
    }

    private void FixedUpdate()
    {
        if (TravelTimer > 0)
        {
            if (!IsThrowing) // Coming Back
                transform.position = Vector3.Lerp(m_LastPosition, m_Player.position, 1-TravelTimer/PullTime);
            else // Throwing
                m_Rigidbody.AddForce(Aim * ThrowForce, ForceMode.Acceleration);
        }
        else //if (HoverTimer > 0)
        {
            m_Rigidbody.velocity = Vector3.zero;
        }


        if (HasMagnetizedObject && !IsPlayerMagnatized)
        {
            if (AttractionTimer > 0)
            {
                AttractionTimer -= Time.fixedDeltaTime;
                MagnetizedObject.position = Vector3.Lerp(m_LastMagnetizedObjectPosition, transform.position, 1-AttractionTimer/AttractionTime);
            }
            else
            {
                MagnetizedObject.SetParent(transform, true);
                MagnetizedObject.localPosition = Vector3.zero;
            }
        }

        if (HasMagnetizedObject && IsPlayerMagnatized)
        {
            Vector3 distance = transform.position - m_Player.position;
            Vector3 direction = distance.normalized;

            if (!m_Player.TryGetComponent(out BoxCollider boxCollider))
                return;

            Vector3 halfHeight = boxCollider.transform.rotation * Vector3.up * boxCollider.size.y / 2 * 0.7f;
            
            int layer = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Magnet"));

            if (Physics.CapsuleCast(m_Player.position + halfHeight, m_Player.position - halfHeight, boxCollider.size.x/2, direction, out RaycastHit hit, distance.magnitude, layer) && hit.transform.gameObject != MagnetizedObject.gameObject)
            {
                MagnetizedObject = null;
                IsPlayerMagnatized = false;
            }
            else
            {
                if (m_Player.TryGetComponent(out Rigidbody rigidbody))
                    rigidbody.velocity = direction * PlayerAttractionSpeed;
            }

            

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        TravelTimer = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasMagnetizedObject)
            return;

        if (other.TryGetComponent(out MagneticObject magneticObject))
        {
            TravelTimer = 0;

            MagneticObject.Polarity polarity = IsPositive ? MagneticObject.Polarity.POSITIVE : MagneticObject.Polarity.NEGATIVE;

            if (magneticObject.AffectSelf)
            {
                if (polarity == magneticObject.polarity || magneticObject.polarity == MagneticObject.Polarity.BOTH_REPULSIVE) // Repulse
                {
                    if (other.TryGetComponent(out Rigidbody rigidbody))
                        rigidbody.AddForce(Aim * RepulsiveForce, ForceMode.VelocityChange);
                }
                else if (magneticObject.polarity == MagneticObject.Polarity.POSITIVE
                      || magneticObject.polarity == MagneticObject.Polarity.NEGATIVE
                      || magneticObject.polarity == MagneticObject.Polarity.BOTH_ATTRACTIVE) // Attract
                {
                    AttractionTimer = AttractionTime;
                    MagnetizedObject = magneticObject.transform;

                    if (other.TryGetComponent(out Rigidbody rigidbody))
                        rigidbody.isKinematic = true;
                    if (other.TryGetComponent(out Collider collider))
                        collider.isTrigger = true;
                }
            }


            if (magneticObject.AffectPlayer) 
            {
                if (polarity == magneticObject.polarity || magneticObject.polarity == MagneticObject.Polarity.BOTH_REPULSIVE) // Repulse
                {
                    Vector3 direction = (m_Player.position - transform.position).normalized;

                    if (m_Player.TryGetComponent(out Rigidbody rigidbody))
                    {
                        if (Vector3.Angle(direction, rigidbody.velocity) > 90)
                            rigidbody.velocity = Vector3.zero;
                        rigidbody.AddForce(direction * RepulsiveForce, ForceMode.VelocityChange);
                    }
                }
                else if (magneticObject.polarity == MagneticObject.Polarity.POSITIVE
                      || magneticObject.polarity == MagneticObject.Polarity.NEGATIVE
                      || magneticObject.polarity == MagneticObject.Polarity.BOTH_ATTRACTIVE) // Attract
                {
                    IsPlayerMagnatized = true;
                    MagnetizedObject = magneticObject.transform;
                }
            }

        GetComponent<SphereCollider>().enabled = false;

        }
    }
}