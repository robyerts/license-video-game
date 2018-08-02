using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public static SceneLoader instance = null;

    // Use this for initialization
    void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadMission1Scene()
    {
        StartCoroutine(LoadSceneAsync("Mission1"));
    }

    public void loadMenuScene()
    {
        StartCoroutine(LoadMainMenuSceneAfterCharSelected());
    }

    IEnumerator LoadSceneAsync(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadMainMenuSceneAfterCharSelected()
    {
        yield return StartCoroutine(LoadSceneAsync("MainMenuScene"));
        MenuManager.instance.disableCharsPanel();
    }
}
