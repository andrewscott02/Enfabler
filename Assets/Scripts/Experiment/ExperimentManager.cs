using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager instance;

    bool adaptiveFirst;

    private void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

            if (Random.Range(0f, 1f) > 0.5f)
            {
                adaptiveFirst = true;
                realScenes[0] = tutorialScene;
                realScenes[1] = levela;
                realScenes[2] = leveli;
                realScenes[3] = endScene;
            }
            else
            {
                adaptiveFirst = false;
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

    public string GetAIMessage()
    {
        if (adaptiveFirst)
        {
            return "The first companion you played against was companion A, the second was companion I";
        }

        return "The first companion you played against was companion I, the second was companion A";
    }
}

public enum E_Scenes
{
    MainMenu, Controls, Adaptive_Experiment, Interval_Experiment, EndMenu
}