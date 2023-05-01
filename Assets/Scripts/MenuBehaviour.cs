using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene("GameManager", LoadSceneMode.Single);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
