using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenceManager : MonoBehaviour
{
    public Button pause;
    public Button exit;
    public Image pauseMenu;
    public Image exitMenu;

    public void Pause()
    {
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pauseMenu.gameObject.SetActive(false);
        exitMenu.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitMenu()
    {
        exitMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
