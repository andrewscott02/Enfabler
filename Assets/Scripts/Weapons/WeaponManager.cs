using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    public static WeaponMoveset equippedWeapon;

    public WeaponMoveset startingWeapon;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        equippedWeapon = startingWeapon;

        DontDestroyOnLoad(gameObject);
    }
}
