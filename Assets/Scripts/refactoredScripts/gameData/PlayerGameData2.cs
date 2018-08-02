using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class PlayerGameData2 : MonoBehaviour
{
    public static PlayerGameData2 Instance = null;

    public List<GameObject> AbilitiesPrefabs; // in case of ranged attack from mage or buff/heal ability from any type of player character
    public List<string> AbilitiesAnimNames;
    public List<string> AbilitiesNames;
    public List<int> AbilitiesManaCosts;
    public List<int> AbilitiesDmg;  //try using AnimationClip.events because dmgs are set there aswell but only for Melee Player?
    public List<Sprite> AbilitiesIcons;

    private PlayerCharInfo charInfo;

    public PlayerCharInfo CharInfo
    {
        get
        {
            return charInfo;
        }

        set
        {
            charInfo = value;
        }
    }

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

    // Update is called once per frame
    void Update()
    {

    }
}
