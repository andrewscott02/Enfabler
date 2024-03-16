using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    public DifficultyData difficulty;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        StartCoroutine(IDelayAssignDelegate(2f));
    }

    IEnumerator IDelayAssignDelegate(float delay)
    {
        yield return new WaitForSeconds(delay);
        TreasureManager.instance.D_GetGoldMultiplier += GetGoldReward;
    }

    float GetGoldReward()
    {
        return difficulty.treasureMultiplier;
    }
}
