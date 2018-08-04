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
    public List<int> AbilitiesForces;
    public List<Sprite> AbilitiesIcons;

    public PlayerCharInfo CharInfo;

    // Use this for initialization
    protected virtual void Start()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //}
        //else if (Instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
        //DontDestroyOnLoad(gameObject);

    }

    protected virtual void OnEnable()
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

    protected virtual void OnDisable()
    {
        Instance = null;
    }

    // Update is called once per frame
    void Update()
    {

    }
    //public void DestroyItself()
    //{
    //    Destroy(this.gameObject);
    //}
}
