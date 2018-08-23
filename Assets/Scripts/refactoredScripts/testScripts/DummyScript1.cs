using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DummyScript1 : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene0()
    {
        Destroy(GameObject.Find("Dummy0"));
        SceneManager.LoadScene("TestScene0");
    }
}
