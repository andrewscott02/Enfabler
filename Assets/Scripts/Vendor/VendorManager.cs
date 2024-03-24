using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class VendorManager : MonoBehaviour
{
    public static VendorManager instance;

    private void Start()
    {
        instance = this;

        OpenVendorMenu(true);
        OpenVendorMenu(false);
    }

    public GameObject vendorMenu;
    public List<GameObject> disable;

    public GameObject weaponContainer;

    public GoldUI goldUI;

    public void OpenVendorMenu(bool open)
    {
        foreach(var item in disable)
        {
            item.SetActive(!open);
        }

        vendorMenu.SetActive(open);

        if (open)
            CheckWeapons();

        if (open)
            goldUI.Setup();
        else
            goldUI.RemoveDelegate();
    }

    public void CloseVendorMenu()
    {
        PauseMenu.instance.ShowVendorMenu(false);
    }

    void CheckWeapons()
    {
        WeaponSelector[] weaponSelectors = weaponContainer.GetComponentsInChildren<WeaponSelector>();

        foreach(var item in weaponSelectors)
        {
            item.Setup(this);
        }

        EventSystem.current.SetSelectedGameObject(weaponSelectors[0].button.gameObject);
    }

    public TextMeshProUGUI weaponName, weaponAttack, weaponDesc;

    public void ShowWeaponDescription(WeaponMoveset weaponDisplay)
    {
        weaponName.text = weaponDisplay.weaponName;
        weaponAttack.text = weaponDisplay.baseAttackPwr.ToString();
        weaponDesc.text = weaponDisplay.weaponDescription;
    }
}