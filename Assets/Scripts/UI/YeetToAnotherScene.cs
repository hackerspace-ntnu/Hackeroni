using UnityEngine;
using UnityEngine.SceneManagement;

public class YeetToAnotherScene: MonoBehaviour
{
    public void YeetToSceneWithIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }
    public void YeetToSceneWithName(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
