using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDungeon : Interactable, IInteractable
{
    public E_Scenes mainMenu;

    public override void Interacted(BaseCharacterController interactCharacter)
    {
        base.Interacted(interactCharacter);

        GameCanvasManager.instance.nextLevelUI.SetActive(true);
        StartCoroutine(IMainMenu(5f));
    }

    IEnumerator IMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(mainMenu.ToString());
    }
}