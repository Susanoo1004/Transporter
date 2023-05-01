using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour m_PlayerBehaviour;

    [SerializeField]
    private Slider m_HealthBar;

    [SerializeField]
    private AudioSource m_LifeLow;


    // Start is called before the first frame update
    void Start()
    {
        m_HealthBar.value = m_PlayerBehaviour.PlayerLife;
        m_HealthBar.maxValue = m_PlayerBehaviour.PlayerLife;
        m_HealthBar.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        m_HealthBar.value = m_PlayerBehaviour.PlayerLife;
             
    }
}
