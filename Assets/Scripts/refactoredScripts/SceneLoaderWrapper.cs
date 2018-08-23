using UnityEngine;
using System.Collections;

public class SceneLoaderWrapper : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void loadMissionScene(int missionNr)
    {
        if(PlayerGameData2.Instance.CharInfo.MissionsCompleted >= missionNr - 1)
        {
            SceneLoader.instance.loadMissionScene(missionNr);
        }
    }
}
