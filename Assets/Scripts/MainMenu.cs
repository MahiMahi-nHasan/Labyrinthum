using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class  MainMenu : MonoBehaviour
{
    public void EnterGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
    }
    public void Instructions()
    {
        SceneManager.LoadScene("Instructions");
    }
    public void BackMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void EndGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
        {
            
        }
    }
}
