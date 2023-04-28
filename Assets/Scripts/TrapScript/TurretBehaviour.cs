using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{

    [SerializeField]
    private float m_AttractionSpeed;

    MagneticObject.Polarity polarity;

    private byte TurretHP = 1;

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
        PolarityMaterial();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PolarityMaterial();
    }

    private void OnTriggerEnter(Collider other)
    {
        // other.GetComponent<Rigidbody>().velocity = (transform.position - other.transform.position).normalized * m_AttractionSpeed;
        if (!other.TryGetComponent(out PlayerBehaviour player))
            return;

        other.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position).normalized * m_AttractionSpeed);
    }


    private void PolarityMaterial()
    {
        switch (polarity)
        {
            case MagneticObject.Polarity.NEGATIVE:
                GetComponent<Renderer>().material = m_NegativeMaterial;
                break;

            case MagneticObject.Polarity.POSITIVE:
                GetComponent<Renderer>().material = m_PositiveMaterial;
                break;

            case MagneticObject.Polarity.BOTH_ATTRACTIVE:
                GetComponent<Renderer>().material = m_AttractiveMaterial;
                break;

            case MagneticObject.Polarity.BOTH_REPULSIVE:
                GetComponent<Renderer>().material = m_RepulsiveMaterial;
                break;
        }
    }
}
