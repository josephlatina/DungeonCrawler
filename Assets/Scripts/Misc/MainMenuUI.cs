using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    /// <summary>
    /// Called from the 'Start' button
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

     /// <summary>
    /// Called from the 'Exit' button
    /// </summary>
    public void Exit()
    {
        Application.Quit();
    }
}
