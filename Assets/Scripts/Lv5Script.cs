using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv5Script : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameObject;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player Projectiles"))
        {
            Destroy(m_GameObject);
            Destroy(gameObject);
        }
    }
}
