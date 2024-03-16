using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyMenuManager : MonoBehaviour
{
    public static DifficultyMenuManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
}
