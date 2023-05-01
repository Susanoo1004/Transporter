using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button levelButton;
    public Button optionsButton;
    public Button quitButton;
    public Image image;

    [SerializeField]
    private AudioSource m_Click;
    [SerializeField]
    private AudioClip[] m_ClickList;

    void Start()
    {
        // Add click listeners to the buttons
        playButton.onClick.AddListener(OnPlayButtonClick);
        levelButton.onClick.AddListener(OnLevelButtonClick);
        optionsButton.onClick.AddListener(OnOptionsButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);

        playButton.Select();        
    }

    public void OnButtonSelected(GameObject button)
    {
        // Set the image position and activation status based on the selected button        
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        RectTransform imageRect = image.GetComponent<RectTransform>();
        imageRect.position = buttonRect.position;
        image.gameObject.SetActive(true);
        Debug.Log("Le caca c'est bon");        
    }

    public void OnSelect(BaseEventData eventData)
    {        
        Debug.Log("Menu button selected!");
        // Add your custom code here for when the button is selected
       
    }

    void OnPlayButtonClick()
    {
        play_Click();
        Debug.Log("Play!");
        // Load the play scene
        //SceneManager.LoadScene("PlayScene");
    }

    void OnLevelButtonClick()
    {
        play_Click();
        Debug.Log("Levels!");
        // Load the level scene
        //SceneManager.LoadScene("LevelScene");
    }

    void OnOptionsButtonClick()
    {
        play_Click();
        Debug.Log("Options!");
        // Load the options scene
        //SceneManager.LoadScene("OptionsScene");
    }

    void OnQuitButtonClick()
    {
        play_Click();
        Debug.Log("Quit");
        // Quit the application
        //Application.Quit();
    }

    public void play_Click()
    {

        int index = Random.Range(0, m_ClickList.Length);
        m_Click.PlayOneShot(m_ClickList[index]);
    }
}

