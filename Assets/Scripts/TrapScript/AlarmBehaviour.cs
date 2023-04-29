using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform m_AreaToDisable;

    [SerializeField]
    private AudioSource m_AlarmSound;

    private bool m_IsEnable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapBox(m_AreaToDisable.position,new Vector3(1.0f, 4.0f, 1.0f), new(0,0,0,0), 1 << 6);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                m_IsEnable = false;
                break;
            }
        }

        if (m_IsEnable) 
        {
            m_AlarmSound.Play(); //Pour les MS : jouer le son de l'alarme ici en loop
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(m_AreaToDisable.position,new Vector3(1.0f, 4.0f, 1.0f));
    }
}
