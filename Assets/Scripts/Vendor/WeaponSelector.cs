using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public WeaponMoveset weapon;

    public Button button;
    public TextMeshProUGUI nameText, buttonText;

    public string equipped, owned, purchase;

    E_WeaponStates weaponState = E_WeaponStates.purchase;

    public void Setup()
    {
        nameText.text = weapon.weaponName;

        weaponState = E_WeaponStates.purchase;

        if (weapon == WeaponManager.equippedWeapon)
        {
            weaponState = E_WeaponStates.equipped;
        }
        else
        {
            if (WeaponManager.instance.ownedWeapons.Contains(weapon))
                weaponState = E_WeaponStates.owned;
        }

        SetButtonInteractivity();
    }

    void SetButtonInteractivity()
    {
        string message = "";

        switch (weaponState)
        {
            case E_WeaponStates.equipped:
                message = owned;
                button.interactable = true;
                break;
            case E_WeaponStates.owned:
                message = owned;
                button.interactable = true;
                break;
            case E_WeaponStates.purchase:
                message = purchase + weapon.goldCost + "G";
                button.interactable = true;
                break;
            default:
                break;
        }

        buttonText.text = message;
    }

    enum E_WeaponStates
    {
        equipped, owned, purchase
    }

    public void ButtonClicked()
    {
        switch (weaponState)
        {
            case E_WeaponStates.equipped:
                WeaponManager.instance.equipDelegate(weapon);
                weaponState = E_WeaponStates.equipped;
                SetButtonInteractivity();
                return;
            case E_WeaponStates.owned:
                WeaponManager.instance.equipDelegate(weapon);
                weaponState = E_WeaponStates.equipped;
                SetButtonInteractivity();
                break;
            case E_WeaponStates.purchase:
                if (CanBuy())
                {
                    TreasureManager.instance.D_GiveGold(-weapon.goldCost);
                    WeaponManager.instance.ownedWeapons.Add(weapon);
                    weaponState = E_WeaponStates.owned;
                    SetButtonInteractivity();
                }
                break;
            default:
                break;
        }
    }

    public bool CanBuy()
    {
        return TreasureManager.goldCount >= weapon.goldCost;
    }
}
