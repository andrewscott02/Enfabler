using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSelector : MonoBehaviour
{
    public WeaponMoveset weapon;

    public Button button;
    ButtonHover buttonHover;
    public TextMeshProUGUI nameText, buttonText;

    public string equipped, owned, purchase;

    public Texture2D cursorEquip, cursorEquipped;
    public Texture2D cursorBuy, cursorCannotBuy;

    E_WeaponStates weaponState = E_WeaponStates.purchase;

    VendorManager vendorManager;

    public void Setup(VendorManager vendorManager)
    {
        this.vendorManager = vendorManager;

        buttonHover = button.GetComponent<ButtonHover>();

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

    public void CheckWeapon()
    {
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
                message = equipped;
                button.interactable = true;
                buttonHover.overideCursorTexture = cursorEquipped;
                break;
            case E_WeaponStates.owned:
                message = owned;
                button.interactable = true;
                buttonHover.overideCursorTexture = cursorEquip;
                break;
            case E_WeaponStates.purchase:
                message = purchase + weapon.goldCost + "G";
                button.interactable = true;
                buttonHover.overideCursorTexture = CanBuy() ? cursorBuy : cursorCannotBuy;
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
                return;
            case E_WeaponStates.owned:
                WeaponManager.instance.equipDelegate(weapon);
                weaponState = E_WeaponStates.equipped;
                break;
            case E_WeaponStates.purchase:
                if (CanBuy())
                {
                    TreasureManager.instance.D_GiveGold(-weapon.goldCost);
                    WeaponManager.instance.ownedWeapons.Add(weapon);
                    weaponState = E_WeaponStates.owned;
                }
                break;
            default:
                break;
        }

        vendorManager.CheckWeapons();
    }

    public bool CanBuy()
    {
        return TreasureManager.goldCount >= weapon.goldCost;
    }

    public void OnHover()
    {
        vendorManager.ShowWeaponDescription(weapon);
    }
}
