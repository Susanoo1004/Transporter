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

    public float HoverTime;
    public float HoverTimer;
    public float TravelTime;
    public float TravelTimer;
    public bool IsThrowing;

    public Vector3 Target;

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
            HoverTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (TravelTimer > 0)
        {
            if (!IsThrowing) // Coming Back
            {
                // a deplacer avec un transform et un lerp

                Vector3 distance = m_Player.position - transform.position;

                m_Rigidbody.velocity = distance.normalized * distance.magnitude / TravelTime;
            }
            else // Throwing
            {
                Vector3 distance = Target - transform.position;

                m_Rigidbody.velocity = distance.normalized * distance.magnitude / TravelTime;
            }
        }
        else //if (HoverTimer > 0)
        {
            m_Rigidbody.velocity = Vector3.zero;
        }
    }
}
