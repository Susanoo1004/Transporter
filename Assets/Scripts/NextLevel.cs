using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private float m_TimeBeforeNextLevel; 
    private float m_TimerBeforeNextLevel;

    private bool m_IsLoadingScene;

    private void Awake()
    {
        m_TimerBeforeNextLevel = m_TimeBeforeNextLevel;
    }

    private void Update()
    {
        if (m_IsLoadingScene)
        {
            m_Image.enabled = true;

            m_TimerBeforeNextLevel -= Time.deltaTime;

            if (m_TimerBeforeNextLevel > 0)
                return;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            m_IsLoadingScene = true;
        }
    }
}
