using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BearTrapBehaviour : MonoBehaviour
{

    [SerializeField]
    private float m_TrapFrequency;
    private float m_TrapCD;

    private bool IsTrapped;

    private GameObject m_Target;

    private string m_OldActionMap;

    // Start is called before the first frame update
    void Start()
    {
        m_TrapCD = m_TrapFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsTrapped) 
        {
            m_TrapCD -= Time.deltaTime;
            if(m_TrapCD > 0 ) 
            {
                return;
            }

            m_Target.GetComponent<PlayerInput>().SwitchCurrentActionMap(m_OldActionMap);
            GetComponent<BearTrapBehaviour>().enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IsTrapped = true;
            m_Target = other.gameObject;
            m_OldActionMap = m_Target.GetComponent<PlayerInput>().currentActionMap.name;
            m_Target.GetComponent<PlayerInput>().SwitchCurrentActionMap("Trap");
        }
    }
}
