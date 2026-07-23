using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnResume : MonoBehaviour
{
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) // Quando o aplicativo volta do background
        {
            RestartScene();
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
