using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollisons : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour m_PlayerBehaviour;

    [SerializeField]
    private Rigidbody m_PlayerRigidbody;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Pre wall" + m_PlayerRigidbody.velocity);
            m_PlayerRigidbody.velocity = new Vector3(0, -1, 0);
            // Debug.Log("Post wall" + m_PlayerRigidbody.velocity);
        }
    }
}
