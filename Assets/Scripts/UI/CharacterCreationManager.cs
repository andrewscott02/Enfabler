using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CartoonHeroes;

public class CharacterCreationManager : MonoBehaviour
{
    public static bool male = true;
    public static int[] pieces = new int[4]
    {
        2, 2, 0, 3
    };

    public static int[] femalePieceOffset = new int[4]
    {
        4, 4, 3, 4
    };

    public SetCharacter setCharacter;
    Animator animator;
    public Avatar maleAvatar, femaleAvatar;
    public Color selectedColour, notSelectedColour;
    public Button maleButton, femaleButton;
    public GameObject legButtonObj, torsoButtonObj, faceButtonObj, hairButtonObj;
    Button[] legButtons, torsoButtons, faceButtons, hairButtons; 

    // Start is called before the first frame update
    void Start()
    {
        animator = setCharacter.GetComponent<Animator>();
        oldPieces = new List<GameObject>();

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

        UpdatePreview();
    }

    List<GameObject> oldPieces;

    void UpdatePreview()
    {
        for (int i = 0; i < oldPieces.Count; i++)
        {
            Destroy(oldPieces[i].gameObject);
        }

        oldPieces.Clear();

        animator.avatar = male ? maleAvatar : femaleAvatar;

        for (int i = 0; i < setCharacter.itemGroups.Length; i++)
        {
            int piece = pieces[i];

            if (!male)
            {
                piece += femalePieceOffset[i];
            }

            Debug.Log("changing piece " + setCharacter.itemGroups[i].name);

            oldPieces.Add(setCharacter.AddItem(setCharacter.itemGroups[i], piece));

            Debug.Log("Successfully changed piece " + setCharacter.itemGroups[i].name);
        }

        Vector3 rot = setCharacter.gameObject.transform.rotation.eulerAngles;
        rot.y += 180f;

        setCharacter.gameObject.transform.rotation = Quaternion.Euler(rot);
    }

    #region Buttons

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

    #endregion

    public void LoadMainMenu()
    {
        StartCoroutine(IDelayLoad());
    }

    IEnumerator IDelayLoad()
    {
        LoadingScreen.instance.StartLoadingScreen();
        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(E_Scenes.MainMenu.ToString());
    }
}
