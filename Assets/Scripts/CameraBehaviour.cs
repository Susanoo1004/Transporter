using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform m_Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = m_Target.position;
        cameraPos.y += 2.0f;
        cameraPos.z -= 20.0f;
        transform.position = cameraPos;
        transform.LookAt(m_Target.position);


    }
}
