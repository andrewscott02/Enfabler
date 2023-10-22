using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public E_Scenes arena;
    public GameObject mainMenu, controls;

    private void Start()
    {
        ShowMouse(true);
        ShowControls(false);
    }

    public void StartGame()
    {
        ShowMouse(false);
        SceneManager.LoadScene(arena.ToString());
    }

    public void ShowControls(bool show)
    {
        mainMenu.SetActive(!show);
        controls.SetActive(show);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void ShowMouse(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}
