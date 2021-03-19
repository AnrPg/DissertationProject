using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    //public SceneManager loadingScreen;
    public Slider slider;

    public void LoadLevel(int sceneIndex)
    {
        transform.gameObject.SetActive(true);
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
               
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;

            yield return null;
        }
    }
}
