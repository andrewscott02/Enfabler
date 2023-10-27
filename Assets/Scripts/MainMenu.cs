using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public E_Scenes arena;
    public GameObject mainMenu, controls;
    public GameObject[] howToPlayPages;

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
        ShowHowToPlayPage(0);
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

    int currentPage = 0;

    void ShowHowToPlayPage(int index)
    {
        currentPage = Mathf.Clamp(index, 0, howToPlayPages.Length - 1);

        for(int i = 0; i < howToPlayPages.Length; i++)
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
