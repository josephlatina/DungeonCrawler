using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    private void Start()
    {
        // Initially hide pause menu
        gameObject.SetActive(false);
    }

    // When script enabled, freeze game
    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    // Unfreeze game
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    // Quite game and load main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Home");
    }
}
