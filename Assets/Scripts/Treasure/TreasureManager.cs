using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    public static int goldCount = 0;

    public static TreasureManager instance;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        D_GiveGold += GiveGoldFunc;
        D_GetGoldMultiplier += GoldMultiplierDelegateCheck;
    }

    public delegate void GiveGoldDelegate(int amount);
    public GiveGoldDelegate D_GiveGold;

    void GiveGoldFunc(int amount)
    {
        goldCount += amount;
        //Debug.Log("GOLD COUNT: " + goldCount + " | " + amount);
    }

    public delegate float GetGoldMultiplierDelegate();
    public GetGoldMultiplierDelegate D_GetGoldMultiplier;

    public Vector2Int GetGoldReward(Vector2Int initialGoldYield)
    {
        var invocations = D_GetGoldMultiplier.GetInvocationList();

        Vector2 goldYield = initialGoldYield;

        for (int i = 0; i < invocations.Length; i++)
        {
            float current = ((GetGoldMultiplierDelegate)invocations[i]).Invoke();

            //Debug.Log("Invocation + " + i + " is " + current);

            goldYield *= current;
        }

        Vector2Int goldYieldInt = new Vector2Int((int)goldYield.x, (int)goldYield.y);
        return goldYieldInt;
    }

    public float GoldMultiplierDelegateCheck()
    {
        return 1;
    }
}
