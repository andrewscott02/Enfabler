using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject trail;
    public GameObject weaponBase, weaponTip;
    public GameObject weaponBaseHit, weaponTipHit;

    // Start is called before the first frame update
    void Start()
    {
        trail.SetActive(false);
    }
}
