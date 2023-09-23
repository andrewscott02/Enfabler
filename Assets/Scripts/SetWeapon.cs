using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWeapon : MonoBehaviour
{
    public Transform handTransform;
    public Object[] weapons;

    public int currentWeapon = -1;

    [ContextMenu("CreateWeapon")]
    public Weapon CreateWeapon()
    {
        if (handTransform == null)
        {
            return null;
        }

        for (int i = 0; i < handTransform.childCount; i++)
        {
            StartCoroutine(IDelayDestroy(handTransform.GetChild(i).gameObject, 0.001f));
        }

        if (currentWeapon >= 0 && currentWeapon < weapons.Length)
        {
            //Debug.Log("Valid weapon, choose model " + currentWeapon);
            GameObject currentWeaponObj = Instantiate(weapons[currentWeapon], new Vector3(-0.0259000007f, 0.000500000024f, 0), new Quaternion(0, 0, 0, 0), handTransform) as GameObject;

            currentWeaponObj.transform.localPosition = new Vector3(-0.0259000007f, 0.000500000024f, 0);
            currentWeaponObj.transform.localRotation = new Quaternion(0, 0, 0, 0);
            currentWeaponObj.transform.localScale = new Vector3(1, 1, 1);

            Weapon weaponScript = currentWeaponObj.GetComponent<Weapon>();
            return weaponScript;
        }

        return null;
    }

    IEnumerator IDelayDestroy(GameObject obj, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        DestroyImmediate(obj);
    }
}