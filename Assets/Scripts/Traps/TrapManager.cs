using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour
{
    public static TrapManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        TrapActivation = ActivateTraps;
        StartCoroutine(IIntervalTraps());
    }

    int m_trapTime = 0;
    public int trapTime
    {
        get
        {
            return m_trapTime;
        }
        set
        {
            m_trapTime = value;
            TrapActivation();
        }
    }

    public delegate void TrapActivationDel();
    public TrapActivationDel TrapActivation;

    IEnumerator IIntervalTraps()
    {
        yield return new WaitForSeconds(1);
        trapTime++;
        StartCoroutine(IIntervalTraps());
    }

    void ActivateTraps()
    {

    }
}
