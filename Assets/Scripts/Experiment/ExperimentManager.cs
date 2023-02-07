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

            if (RandomBool())
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
        string code = GenerateReferenceCode();

        if (adaptiveFirst)
        {
            return "Your reference code is " + code + ". Please write this down or remember it as it is needed for the questionnaire, and will also be needed if you wish to withdraw from the experiment. The first companion you played against was companion AM, the second was companion IT.";
        }

        return "Your reference code is " + code + ". Please write this down or remember it as it is needed for the questionnaire, and will also be needed if you wish to withdraw from the experiment. The first companion you played against was companion IT, the second was companion AM.";
    }

    public string GenerateReferenceCode()
    {
        string code = "";

        if (adaptiveFirst)
        {
            code += "AM-IT-";
        }
        else
        {
            code += "IT-AM-";
        }

        code += (int)Random.Range((int)100, (int)999) + "-";
        code += Remap(Time.frameCount, new Vector2Int(0, 100000), new Vector2Int(10000, 99999));

        Debug.Log(code);

        return code;
    }

    int Remap(int In, Vector2Int InMinMax, Vector2Int OutMinMax)
    {
        //Remap Function: https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Remap-Node.html
        return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
    }

    #region Random

    bool RandomBool()
    {
        return Random.Range(0, 2) > 0;
    }

    #region Tests

    void TestRandom(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            if (RandomBool())
            {
                Debug.Log("interval");
            }
            else
            {
                Debug.Log("adaptive");
            }
        }
    }

    [ContextMenu("Test Random/ 1")]
    public void TestRandom1()
    {
        TestRandom(1);
    }

    [ContextMenu("Test Random/ 100")]
    public void TestRandom100()
    {
        TestRandom(100);
    }

    [ContextMenu("Test Random/ 100000")]
    public void TestRandom100000()
    {
        TestRandom(100000);
    }

    #endregion

    #endregion
}

public enum E_Scenes
{
    MainMenu, Controls, Adaptive_Experiment, Interval_Experiment, EndMenu
}