using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenUI : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;
    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(() => { SceneManager.LoadScene(1, LoadSceneMode.Single); });
        quitButton.onClick.AddListener(() => { Application.Quit(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
