using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{

    [SerializeField]
    private float m_AttractionSpeed;

    MagneticObject.Polarity polarity;

    [SerializeField]
    private Material m_PositiveMaterial;
    [SerializeField]
    private Material m_NegativeMaterial;
    [SerializeField]
    private Material m_AttractiveMaterial;
    [SerializeField]
    private Material m_RepulsiveMaterial;

    private void OnValidate()
    {
         
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // other.GetComponent<Rigidbody>().velocity = (transform.position - other.transform.position).normalized * m_AttractionSpeed;
        if (!other.TryGetComponent(out PlayerBehaviour player)) // ou magnetic object
            return;

        other.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position).normalized * m_AttractionSpeed);
    }
}
