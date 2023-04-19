using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
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

    [HideInInspector]
    public float RepulsiveForce;
    [HideInInspector]
    public float AttractionTime;
    [HideInInspector]
    public float AttractionTimer;
    [HideInInspector]
    public float ThrowForce;
    [HideInInspector]
    public float HoverTime;
    [HideInInspector]
    public float HoverTimer;
    [HideInInspector]
    public float PullTime;
    [HideInInspector]
    public float TravelTimer;
    [HideInInspector]
    public bool IsThrowing;


    [HideInInspector]
    public Transform MagnetizedObject;
    private Vector3 m_LastMagnetizedObjectPosition;

    [HideInInspector]
    public Vector3 Aim;

    private Vector3 m_LastPosition;

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
    
        if (TravelTimer > 0)
        {
            TravelTimer -= Time.deltaTime;
        }
        else if (HoverTimer > 0)
        {
            HoverTimer -= Time.deltaTime;
            m_LastPosition = transform.position;
        }

        if (AttractionTimer > 0)
        {
            AttractionTimer -= Time.deltaTime;
            m_LastMagnetizedObjectPosition = MagnetizedObject.position;
        }
    }

    private void FixedUpdate()
    {
        if (TravelTimer > 0)
        {
            if (!IsThrowing) // Coming Back
            {
                transform.position = Vector3.Lerp(m_LastPosition, m_Player.position, 1-TravelTimer/PullTime);
            }
            else // Throwing
            {
                m_Rigidbody.AddForce(Aim * ThrowForce, ForceMode.Acceleration);
            }
        }
        else //if (HoverTimer > 0)
        {
            m_Rigidbody.velocity = Vector3.zero;
        }

        if (HasMagnetizedObject)
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
                // no idea yet lol
            }

            GetComponent<SphereCollider>().enabled = false;

        }
    }
}
