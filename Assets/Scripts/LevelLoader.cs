using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Image loading;

    public void LoadLevel (string sceneName, float delay = 0)
    {
        StartCoroutine(LoadAsynchronously(sceneName, delay));
    }

    // Start is called before the first frame update
    IEnumerator LoadAsynchronously (string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            if (!loading?.enabled ?? false)
                Loading();
            
            yield return null;
        }
    }

    void Loading()
    {
        loading.GetComponent<Animator>().enabled = true;
        loading.enabled = true;
    }
}
