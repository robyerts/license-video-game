using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
#if UNITY_EDITOR 
using UnityEditor.Presets;
#endif

public class PlayerGameData2 : MonoBehaviour
{
    public static PlayerGameData2 Instance = null;
    public static int GameObjectInstances = 0;

    public List<GameObject> AbilitiesPrefabs; // in case of ranged attack from mage or buff/heal ability from any type of player character
    public List<string> AbilitiesAnimNames;
    public List<string> AbilitiesNames;
    public List<int> AbilitiesManaCosts;
    public List<int> AbilitiesDmg;  
    public List<int> AbilitiesForces;
    public List<Sprite> AbilitiesIcons;
    public List<float> abilitiesTimeBe4Attack;
    public List<float> abilitiesTimeBe4Destroy;

    public PlayerCharInfo CharInfo;

    // Use this for initialization
    protected virtual void Start()
    {
        if (GameObjectInstances == 1 && Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

}
