using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DummyScript0 : MonoBehaviour
{
    public static DummyScript0 Instance = null;

    // Use this for initialization
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene1()
    {
        SceneManager.LoadScene("TestScene1");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
