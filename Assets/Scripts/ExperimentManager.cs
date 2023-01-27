using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager instance;

    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            if (Random.Range(0f, 1f) > 0.5f)
            {
                realScenes[0] = tutorialScene;
                realScenes[1] = levela;
                realScenes[2] = leveli;
                realScenes[3] = endScene;
            }
            else
            {
                realScenes[0] = tutorialScene;
                realScenes[1] = leveli;
                realScenes[2] = levela;
                realScenes[3] = endScene;
            }
        }
        else { Destroy(this.gameObject); }
    }

    public E_Scenes tutorialScene;
    public E_Scenes levela, leveli;
    public E_Scenes endScene;

    E_Scenes[] realScenes = new E_Scenes[4];
    int currentScene = 0;

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(realScenes[currentScene].ToString(), LoadSceneMode.Single);
        currentScene++;
    }
}

public enum E_Scenes
{
    MainMenu, Controls, Adaptive_Experiment, Interval_Experiment
}