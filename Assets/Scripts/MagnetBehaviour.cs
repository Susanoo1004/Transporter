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
    public Vector3 Aim;

    private Vector3 m_LastPosition;

    private bool HasMagnet { get { return transform.parent == m_Player.transform; } }


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
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
            TravelTimer -= Time.deltaTime;
        else if (HoverTimer > 0)
        {
            HoverTimer -= Time.deltaTime;
            m_LastPosition = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (TravelTimer > 0)
        {
            if (!IsThrowing) // Coming Back
            {
                transform.position = Vector3.Lerp(m_LastPosition, m_Player.position, 1 - TravelTimer / PullTime);
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
    }
}
