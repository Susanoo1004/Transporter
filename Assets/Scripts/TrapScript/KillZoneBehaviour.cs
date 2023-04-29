using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZoneBehaviour : MonoBehaviour
{

    [SerializeField]
    private AudioSource m_KillzoneWire;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<PlayerBehaviour>().PlayerLife = 0;

            m_KillzoneWire.Play();
            // ms : son barbele 

        }
    }
}
