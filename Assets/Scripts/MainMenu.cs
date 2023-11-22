using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    #region Setup

    public E_Scenes pcgDungeon, arena;
    public GameObject mainMenu, controls;
    public GameObject mainMenuDefaultButton, controlsDefaultButton;
    GameObject currentPageDefault;
    public GameObject[] howToPlayPages;

    private void Start()
    {
        ShowMouse(true);
        ShowControls(false);
    }

    void ShowMouse(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    #endregion

    #region Buttons

    public void StartGame()
    {
        ShowMouse(false);
        SceneManager.LoadScene(pcgDungeon.ToString());
    }

    public void ArenaMode()
    {
        SceneManager.LoadScene(arena.ToString());
    }

    public void ShowControls(bool show)
    {
        mainMenu.SetActive(!show);
        controls.SetActive(show);

        currentPageDefault = show ? controlsDefaultButton : mainMenuDefaultButton;
        EventSystem.current.SetSelectedGameObject(currentPageDefault);

        ShowHowToPlayPage(0);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
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

    #endregion

    #region Inputs

    public void Close(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ShowControls(false);
    }

    public void NextPage(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ChangePage(true);
    }

    public void PreviousPage(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ChangePage(false);
    }

    public void OnControlsChange(PlayerInput input)
    {
        bool usingGamepad = input.currentControlScheme == "Gamepad";
        ShowMouse(!usingGamepad);

        if (!usingGamepad) return;

        EventSystem.current.SetSelectedGameObject(currentPageDefault);
    }

    #endregion
}