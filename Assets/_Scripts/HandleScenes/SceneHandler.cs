using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Debug.Log("Scene Loaded: " + sceneName);
    }
}
