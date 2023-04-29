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

<<<<<<< HEAD

=======
            m_KillzoneWire.Play();
>>>>>>> 4154e4c29a60c47f951311d4c98cf571f799ae93
            // ms : son barbele 

        }
    }
}
