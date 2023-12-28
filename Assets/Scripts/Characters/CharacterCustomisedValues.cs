using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CartoonHeroes;

public class CharacterCustomisedValues : MonoBehaviour
{
    SetCharacter setCharacter;
    Animator animator;
    public Avatar maleAvatar, femaleAvatar;

    // Start is called before the first frame update
    void Start()
    {
        setCharacter = GetComponent<SetCharacter>();
        animator = GetComponentInParent<Animator>();

        SetValues();
    }

    void SetValues()
    {
        //TODO : Get values from player customise settings instance
        animator.avatar = CharacterCreationManager.male ? maleAvatar : femaleAvatar;

        for (int i = 0; i < setCharacter.itemGroups.Length; i++)
        {
            int piece = CharacterCreationManager.pieces[i];

            if (!CharacterCreationManager.male)
            {
                piece += CharacterCreationManager.femalePieceOffset[i];
            }

            setCharacter.AddItem(setCharacter.itemGroups[i], piece);
        }
    }
}
