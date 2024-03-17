using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    public DifficultyData difficulty;

    private void Start()
    {
        if (instance != null)
        {
            //Debug.Log("Difficulty singleton exists - Destroying");
            DestroyImmediate(this.gameObject);
        }
        else
        {
            StartCoroutine(IDelaySetup(0.5f));
        }
    }

    IEnumerator IDelaySetup(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Difficulty - Adding delegate function");
        TreasureManager.D_GetGoldMultiplier += GetGoldReward;

        //Debug.Log("Difficulty - Setting up difficulty singleton");
        gameObject.transform.parent = null;
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }

    float GetGoldReward()
    {
        Debug.Log("Difficulty - Get Treasure Multiplier");
        return difficulty.treasureMultiplier;
    }
}
