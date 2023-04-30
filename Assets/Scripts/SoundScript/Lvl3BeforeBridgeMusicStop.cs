using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lvl3BeforeBridgeMusicStop : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_BeforeBridgeMusic;

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
        if (other.gameObject.GetComponent<PlayerBehaviour>())
        {
            m_BeforeBridgeMusic.Stop();
        }
    }
}
