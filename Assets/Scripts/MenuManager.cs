using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public static MenuManager instance = null;

    [SerializeField] private Text charName;
    [SerializeField] private GameObject newCharNamePanel;
    [SerializeField] private GameObject charsPanel;
    [SerializeField] private GameObject mainMenuPanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        PlayerPrefs.SetString("maxNrCharacters", "5"); //hardcoded & UI aswell

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void confirmName()
    {
        PlayerPrefs.SetString("playerName", charName.text);
        disableNewCharNamePanel();
    }

    public void disableNewCharNamePanel()
    {
        newCharNamePanel.SetActive(false);
        charsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void loadMission1Scene()
    {
        SceneManager.LoadScene("removedColliderPhysics");
    }

}
