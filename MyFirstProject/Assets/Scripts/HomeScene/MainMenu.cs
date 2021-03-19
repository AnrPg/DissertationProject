using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame()
    {
        SceneLoader sceneLoader = transform.gameObject.transform.parent.Find("LoadingScreen")?.GetComponent<SceneLoader>();
        if (sceneLoader != null)
        {
            transform.gameObject.SetActive(false);
            sceneLoader.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
    }

    public void QuitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}
