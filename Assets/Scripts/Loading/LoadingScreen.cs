using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;

    public bool endLoadingScreenOnStart;
    public float loadDelay = 2f;

    public void Start()
    {
        instance = this;

        if (loading)
            animator.SetTrigger("Loading");

        if (endLoadingScreenOnStart)
            StartCoroutine(IDelayStopLoad());
    }

    IEnumerator IDelayStopLoad()
    {
        yield return new WaitForSecondsRealtime(loadDelay);
        EndLoadingScreen();
    }

    public Animator animator;
    public Slider slider;
    public TextMeshProUGUI text;
    static bool loading = false;

    public void StartLoadingScreen()
    {
        //Debug.Log("Start loading");

        loading = true;
        animator.SetTrigger("StartLoading");

        LoadProgress(0);
    }

    public void LoadProgress(float progress, string loadMessage = "Loading...")
    {
        if (!loading)
            return;

        //Debug.Log("Loading " + progress);

        text.text = loadMessage;
        slider.value = Mathf.Clamp(progress, 0f, 1f);

        if (progress >= 1f)
            EndLoadingScreen();
    }

    public void EndLoadingScreen()
    {
        //Debug.Log("End loading");

        loading = false;
        animator.SetTrigger("EndLoading");
    }
}