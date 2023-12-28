using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreationManager : MonoBehaviour
{
    public static bool male = true;
    public static int[] pieces = new int[4]
    {
        2, 2, 1, 3
    };

    public static int[] femalePieceOffset = new int[4]
    {
        4, 4, 3, 4
    };

    public Color selectedColour, notSelectedColour;
    public Button maleButton, femaleButton;
    public GameObject legButtonObj, torsoButtonObj, faceButtonObj, hairButtonObj;
    Button[] legButtons, torsoButtons, faceButtons, hairButtons; 

    // Start is called before the first frame update
    void Start()
    {
        SetupButtonObjects();
        SetValues();
    }

    void SetupButtonObjects()
    {
        legButtons = legButtonObj.GetComponentsInChildren<Button>();
        torsoButtons = torsoButtonObj.GetComponentsInChildren<Button>();
        faceButtons = faceButtonObj.GetComponentsInChildren<Button>();
        hairButtons = hairButtonObj.GetComponentsInChildren<Button>();
    }

    void SetValues()
    {
        maleButton.image.color = male ? selectedColour : notSelectedColour;
        femaleButton.image.color = !male ? selectedColour : notSelectedColour;

        for (int i = 0; i < legButtons.Length; i++)
        {
            legButtons[i].image.color = (i == pieces[0]) ? selectedColour : notSelectedColour;
        }

        for (int i = 0; i < torsoButtons.Length; i++)
        {
            torsoButtons[i].image.color = (i == pieces[1]) ? selectedColour : notSelectedColour;
        }

        for (int i = 0; i < faceButtons.Length; i++)
        {
            faceButtons[i].image.color = (i == pieces[2]) ? selectedColour : notSelectedColour;
        }

        for (int i = 0; i < hairButtons.Length; i++)
        {
            hairButtons[i].image.color = (i == pieces[3]) ? selectedColour : notSelectedColour;
        }
    }

    public void SetGender(bool setMale)
    {
        male = setMale;
        SetValues();
    }

    public void SetLegs(int piece)
    {
        pieces[0] = piece;
        SetValues();
    }

    public void SetTorso(int piece)
    {
        pieces[1] = piece;
        SetValues();
    }

    public void SetFace(int piece)
    {
        pieces[2] = piece;
        SetValues();
    }

    public void SetHair(int piece)
    {
        pieces[3] = piece;
        SetValues();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(E_Scenes.MainMenu.ToString());
    }
}
