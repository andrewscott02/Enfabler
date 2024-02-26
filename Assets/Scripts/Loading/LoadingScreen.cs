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
        yield return new WaitForSeconds(loadDelay);
        EndLoadingScreen();
    }

    public Animator animator;
    public Slider slider;
    public TextMeshProUGUI text;
    static bool loading = false;
    static float loadProgress = 0;

    public void StartLoadingScreen()
    {
        Debug.Log("Start loading");

        loading = true;
        loadProgress = 0;
        animator.SetTrigger("StartLoading");

        LoadProgress(loadProgress);
    }

    public void LoadProgress(float progress, string loadMessage = "Loading...")
    {
        Debug.Log("Loading " + loadProgress);
        loadProgress = progress;

        text.text = loadMessage;
        slider.value = Mathf.Clamp(loadProgress, 0f, 1f);

        if (loadProgress >= 1f)
            EndLoadingScreen();
    }

    public void EndLoadingScreen()
    {
        Debug.Log("End loading");

        loading = false;
        animator.SetBool("Loading", loading);
        loadProgress = 0;
        animator.SetTrigger("EndLoading");
    }
}