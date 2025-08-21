using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loadingscene : MonoBehaviour
{
    public float time;
    public AsyncOperation load;


    private void Awake()
    {
        StartCoroutine(LoadScene());
        Debug.Log("Start");
    }
    IEnumerator LoadScene()
    {
        Time.timeScale = 1.0f;
        yield return null;
        load = SceneManager.LoadSceneAsync(SceneNumber.Number);
        load.allowSceneActivation = false;
        while (!load.isDone)
        {
            time += Time.deltaTime;
            yield return null;
            if (load.progress >= 0.9f && time >= 3)
            {
                load.allowSceneActivation = true;
            }
        }
    }
}
