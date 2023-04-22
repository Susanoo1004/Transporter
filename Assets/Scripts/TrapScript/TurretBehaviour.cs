using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{

    [SerializeField]
    private float m_AttractionForce;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        // other.GetComponent<Rigidbody>().velocity = (transform.position - other.transform.position).normalized * m_AttractionForce;
        other.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position).normalized * m_AttractionForce);
    }
}
