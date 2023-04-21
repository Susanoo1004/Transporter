using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{

    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private bool m_LoopBack;

    [SerializeField]
    private float m_StopDuration;

    [SerializeField]
    private Vector2[] m_PathPositions;

    private int m_Index;

    private Rigidbody m_Rigidbody;

    private float m_StopCooldown = 0;

    private void OnValidate()
    {
        if (m_Speed < 0)
            m_Speed = 0;

        if (m_StopDuration < 0)
            m_StopDuration = 0;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < m_PathPositions.Length; ++i)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_PathPositions[i], 0.2f);
        }
    }


    private void Awake()
    {
        m_Index = 0;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (m_StopCooldown > 0)
        {
            m_StopCooldown -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (m_PathPositions.Length <= 0)
            return;

        Vector2 distance = m_PathPositions[m_Index] - (Vector2)transform.localToWorldMatrix.GetPosition();
        Vector2 direction = distance.normalized;

        if ((direction * m_Speed * Time.fixedDeltaTime).sqrMagnitude >= distance.sqrMagnitude)
        {
            m_Rigidbody.velocity = Vector3.zero;
            transform.position = m_PathPositions[m_Index];

            m_Index++;
            if (m_Index == m_PathPositions.Length)
                m_Index = 0;

            if (!m_LoopBack && m_Index == 0 && m_PathPositions.Length > 1)
            {
                System.Array.Reverse(m_PathPositions);
                m_Index++;
            }

            m_StopCooldown = m_StopDuration;
        }
        else if (m_StopCooldown <= 0)
        {
            m_Rigidbody.velocity = (Vector3)direction * m_Speed;
            //transform.position += (Vector3)direction * m_Speed * Time.fixedDeltaTime;
        }
    }
}
