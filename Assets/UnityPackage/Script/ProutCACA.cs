using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProutCACA : MonoBehaviour
{
    public RectTransform image;
    public Vector2 Posimage;
    public RectTransform test;
    public Button caca;

    private void Awake()
    {
        test = GetComponent<RectTransform>();
        Posimage = new Vector2(-314f, 0f);
        caca = GetComponent<Button>();
    }

    public void Update()
    {
        // Check if the current selected game object is a button
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>() == caca)
            {
                Debug.Log("Button is currently selected!" + this.name);
                image.anchoredPosition = Posimage + test.anchoredPosition;
                
            }
        }
    }
}
