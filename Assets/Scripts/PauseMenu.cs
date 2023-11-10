using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public bool paused = false;

    E_Scenes mainMenu = E_Scenes.MainMenu;
    public GameObject pauseMenu, controls;
    public GameObject[] howToPlayPages;

    float unpausedTimeScale = 1;

    private void Start()
    {
        instance = this;
        unpausedTimeScale = Time.timeScale;

        Resume();
    }

    public void PauseGame()
    {
        Debug.Log("Pause game input");
        ShowMouse(true);
        paused = true;
        pauseMenu.SetActive(true);
        controls.SetActive(false);

        Time.timeScale = 0;
    }

    public void Resume()
    {
        ShowMouse(false);
        paused = false;
        pauseMenu.SetActive(false);
        controls.SetActive(false);

        Time.timeScale = unpausedTimeScale;
    }

    public void ShowControls(bool show)
    {
        pauseMenu.SetActive(!show);
        controls.SetActive(show);
        ShowHowToPlayPage(0);
    }

    public void MainMenu()
    {
        Resume();
        SceneManager.LoadScene(mainMenu.ToString());
    }

    void ShowMouse(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    int currentPage = 0;

    void ShowHowToPlayPage(int index)
    {
        currentPage = Mathf.Clamp(index, 0, howToPlayPages.Length - 1);

        for (int i = 0; i < howToPlayPages.Length; i++)
        {
            howToPlayPages[i].SetActive(i == currentPage);
        }
    }

    public void ChangePage(bool next)
    {
        int nextPage = currentPage + (next ? 1 : -1);
        ShowHowToPlayPage(nextPage);
    }
}
