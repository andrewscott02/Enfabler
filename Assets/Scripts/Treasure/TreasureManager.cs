using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public static int goldCount = 0;

    public static TreasureManager instance;

    private void Start()
    {
        instance = this;
        D_GiveGold += GiveGoldFunc;
    }

    public delegate void GiveGoldDelegate(int amount);
    public GiveGoldDelegate D_GiveGold;

    void GiveGoldFunc(int amount)
    {
        goldCount += amount;
        Debug.Log("GOLD COUNT: " + goldCount + " | " + amount);
    }
}
