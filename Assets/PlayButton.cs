using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public AsyncOperation asyncOperation;

    public void Start()
    {
        asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;
    }

    public void Play()
    {
        if (asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = true;
        }
    }
}
