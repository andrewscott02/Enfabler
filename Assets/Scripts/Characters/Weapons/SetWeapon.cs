using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWeapon : MonoBehaviour
{
    public OverrideWeapon overrideWeapon;

    public Transform[] handTransforms;
    public Object[] weapons, offhandWeapons;

    public int currentWeapon = -1;

    [ContextMenu("CreateWeapon")]
    public Weapon CreateWeapon(int weaponIndex, int hand, Object[] weaponSelection)
    {
        if (handTransforms[hand] == null)
        {
            return null;
        }

        for (int i = 0; i < handTransforms[hand].childCount; i++)
        {
            StartCoroutine(IDelayDestroy(handTransforms[hand].GetChild(i).gameObject, 0.001f));
        }

        if (weaponIndex >= 0 && weaponIndex < weaponSelection.Length)
        {
            currentWeapon = weaponIndex;

            //Debug.Log("Valid weapon, choose model " + currentWeapon);
            GameObject currentWeaponObj = Instantiate(weaponSelection[weaponIndex], new Vector3(-0.0259000007f, 0.000500000024f, 0), new Quaternion(0, 0, 0, 0), handTransforms[hand]) as GameObject;

            currentWeaponObj.transform.localPosition = new Vector3(-0.0259000007f, 0.000500000024f, 0);
            currentWeaponObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            currentWeaponObj.transform.localScale = new Vector3(1, 1, 1);

            Weapon weaponScript = currentWeaponObj.GetComponent<Weapon>();
            return weaponScript;
        }

        return null;
    }

    public void EquipMoveset(WeaponMoveset weapon)
    {
        if (overrideWeapon == null)
            return;

        if (weapon == null)
            return;

        weapons = weapon.weapons;
        offhandWeapons = weapon.offhandWeapons;

        overrideWeapon.EquipWeapon();

        CreateWeapon(0, 0, weapons);
        CreateWeapon(0, 1, offhandWeapons);
    }

    private void Start()
    {
        Invoke("Setup", 0.1f);
    }

    void Setup()
    {
        WeaponManager.instance.equipDelegate += EquipMoveset;
        WeaponManager.instance.equipDelegate(WeaponManager.equippedWeapon);
    }

    IEnumerator IDelayDestroy(GameObject obj, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        DestroyImmediate(obj);
    }
}