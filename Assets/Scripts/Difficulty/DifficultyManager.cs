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
            Debug.Log("Difficulty singleton exists - Destroying");

            Destroy(this.gameObject);
        }

        Debug.Log("Setting up difficulty singleton");

        instance = this;
        gameObject.transform.parent = null;
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
