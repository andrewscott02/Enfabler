using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorManager : MonoBehaviour
{
    public static VendorManager instance;

    private void Start()
    {
        instance = this;
    }

    public GameObject vendorMenu;
    public List<GameObject> disable;

    public GameObject weaponContainer; 

    public void OpenVendorMenu(bool open)
    {
        foreach(var item in disable)
        {
            item.SetActive(!open);
        }

        vendorMenu.SetActive(open);

        if (open)
            CheckWeapons();

        ShowMouse(open);
    }

    void CheckWeapons()
    {
        WeaponSelector[] weaponSelectors = weaponContainer.GetComponentsInChildren<WeaponSelector>();

        foreach(var item in weaponSelectors)
        {
            item.Setup();
        }
    }

    void ShowMouse(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}