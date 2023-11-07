using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCapacityUI : MonoBehaviour
{
    public GameObject[] arrows;

    public void SetArrows(int arrowCount)
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].SetActive(i < arrowCount);
        }
    }
}
