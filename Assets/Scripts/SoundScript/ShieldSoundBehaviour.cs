using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSoundBehaviour : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player Projectiles"))
        {
            //son shield damage
        }
    }

}
