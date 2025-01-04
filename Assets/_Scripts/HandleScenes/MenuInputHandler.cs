using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInputHandler : MonoBehaviour
{
    [SerializeField] private string _MenuSceneName;
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (_MenuSceneName != SceneManager.GetActiveScene().name)
            {
                SceneManager.LoadScene(_MenuSceneName);
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
