using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public E_Scenes mainMenu, arena;

    private void Start()
    {
        ShowMouse(true);
    }

    public void StartGame()
    {
        ShowMouse(false);
        SceneManager.LoadScene(arena.ToString());
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
