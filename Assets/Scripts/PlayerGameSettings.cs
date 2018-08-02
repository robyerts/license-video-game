using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class PlayerGameSettings : MonoBehaviour {
    public static PlayerGameSettings instance = null;

    public List<string> abilitiesAnimNames;
    public List<string> abilitiesNames;
    public List<float> abilitiesAttackRanges;
    public List<int> abilitiesManaCosts;
    public List<int> abilitiesDmg;
    public List<Sprite> abilitiesIcons;
    public List<int> abilitiesXpCosts; // currently not used

    public int maxNrAbilitiesInBattle = 4; // not sure how it affects changing this; maybe some for loops, since UI is not updated with this number

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
}
