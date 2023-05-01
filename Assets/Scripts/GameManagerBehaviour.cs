using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerBehaviour : MonoBehaviour
{
    public bool InPause;

    public bool NeedChangeScene;

    private string CurrentGameSceneName;

    // Start is called before the first frame update
    void Start()
    {
        CurrentGameSceneName = "MainMenu";
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(CurrentGameSceneName);
        if (InPause)
        {
            if (NeedChangeScene)
            {
                SceneManager.LoadScene("GameManager", LoadSceneMode.Single);
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
                NeedChangeScene = false;
            }
        }
      
    }

    public void OnOptionPress(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            Debug.Log("Before" + InPause);
            NeedChangeScene = true;
            InPause = true;
            Debug.Log("After" + InPause);
        }
    }
}
