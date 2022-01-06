using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    public void LoadNextScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex + 1);
    }

    public void loadPvP()
    {
        SceneManager.LoadScene(2);
    }

    public void loadRandom()
    {
        SceneManager.LoadScene(3);
    }

    public void loadMCTS()
    {
        SceneManager.LoadScene(4);
    }

    public void loadRandomSim()
    {
        SceneManager.LoadScene(5);
    }

}
