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
                SceneManager.UnloadSceneAsync(CurrentGameSceneName);
                SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
                NeedChangeScene = false;
            }
        }
        else
        {

            if (NeedChangeScene)
            {
                SceneManager.UnloadSceneAsync("Pause");
                SceneManager.LoadScene(CurrentGameSceneName, LoadSceneMode.Additive);
                NeedChangeScene = false;
                CurrentGameSceneName = SceneManager.GetSceneAt(1).name;
            }
        }
    }

    public void OnOptionPress(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            Debug.Log("Before" + InPause);
            NeedChangeScene = true;
            InPause ^= true;
            Debug.Log("After" + InPause);
        }
    }
}
