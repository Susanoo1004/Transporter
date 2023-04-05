using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Vector2[] m_PathPositions;

    public int m_Index;

    private void OnDrawGizmos()
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 distance = m_PathPositions[m_Index] - (Vector2)transform.localToWorldMatrix.GetPosition();
        Vector2 direction = distance.normalized;

        if ((direction * m_Speed * Time.fixedDeltaTime).sqrMagnitude >= distance.sqrMagnitude)
        {
            transform.position = m_PathPositions[m_Index];

            m_Index++;
            if (m_Index == m_PathPositions.Length)
                m_Index = 0;

            if (m_LoopBack && m_Index == m_PathPositions.Length)
                m_Index = 0;
            if (!m_LoopBack && m_Index == 0)
                System.Array.Reverse(m_PathPositions);
        }
        else
        {
            transform.position += (Vector3)direction * m_Speed * Time.fixedDeltaTime;
        }
    }
}
