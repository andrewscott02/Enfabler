using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public bool paused = false;

    E_Scenes mainMenu = E_Scenes.MainMenu;
    public GameObject pauseMenu, controls;
    public GameObject pauseMenuDefaultButton, controlsDefaultButton;
    GameObject currentPageDefault;
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
        StartCoroutine(IDelayPause(0.15f));
    }

    IEnumerator IDelayPause(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        ShowMouse(true);
        paused = true;
        ShowControls(false);

        Time.timeScale = 0;
    }

    public void Resume()
    {
        StartCoroutine(IDelayResume(0.15f));
    }

    IEnumerator IDelayResume(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        ShowMouse(false);
        paused = false;
        ShowControls(false);
        pauseMenu.SetActive(false);
        controls.SetActive(false);

        Time.timeScale = unpausedTimeScale;
    }

    public void ShowControls(bool show)
    {
        pauseMenu.SetActive(!show);
        controls.SetActive(show);

        currentPageDefault = show ? controlsDefaultButton : pauseMenuDefaultButton;
        EventSystem.current.SetSelectedGameObject(currentPageDefault);

        ShowHowToPlayPage(0);
    }

    public void MainMenu()
    {
        Time.timeScale = unpausedTimeScale;
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

    public void OnControlsChange(PlayerInput input)
    {
        bool usingGamepad = input.currentControlScheme == "Gamepad";

        if (paused)
        {
            ShowMouse(!usingGamepad);
        }
        else
        {
            ShowMouse(false);
        }

        if (!usingGamepad) return;

        EventSystem.current.SetSelectedGameObject(currentPageDefault);
    }
}
