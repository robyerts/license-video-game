using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//[System.Serializable]
public class PlayerCharInfo
{
    private CharacterType charType;
    private int index;
    private string name;
    private int maxHP;
    private int maxMana;
    private int currentXP;// not in use
    private int missionsCompleted;
    private List<int> abilities;

    public PlayerCharInfo(int index, string name, CharacterType charType)
    {
        //hardcoded values
        //create static fields for initial hp, mana,.. values
        this.charType = charType;
        this.index = index;
        this.name = name;
        this.maxHP = 100;
        this.maxMana = 60;
        this.currentXP = 0;
        this.missionsCompleted = 0;
        this.abilities = new List<int>(new int[] { 1,1,0,0 });
    }

    public PlayerCharInfo(int index)
    {
        this.Index = index;
        this.abilities = new List<int>();
        
    }

    public void Save()
    {
        PlayerPrefs.SetString("Character_" + Index + "_name", name);
        PlayerPrefs.SetString("Character_" + Index + "_charType", charType.ToString());
        PlayerPrefs.SetInt("Character_" + Index + "_maxHP", maxHP);
        PlayerPrefs.SetInt("Character_" + Index + "_maxMana", maxMana);
        PlayerPrefs.SetInt("Character_" + Index + "_currentXP", CurrentXP);
        PlayerPrefs.SetInt("Character_" + Index + "_missionsCompleted", missionsCompleted);
        //Debug.Log(string.Join(",", abilities.Select(x => x.ToString()).ToArray()));
        PlayerPrefs.SetString("Character_" + Index + "_abilities", string.Join(",", abilities.Select(x => x.ToString()).ToArray()));
    }
    public void Load()
    {
        this.abilities = new List<int>();
        //needs to be tested
        //charType = (CharacterType)Enum.Parse(typeof(CharacterType), PlayerPrefs.GetString("Character_" + Index + "_charType"));
        name = PlayerPrefs.GetString("Character_" + Index + "_name");
        maxHP = PlayerPrefs.GetInt("Character_" + Index + "_maxHP");
        maxMana =  PlayerPrefs.GetInt("Character_" + Index + "_maxMana");
        currentXP = PlayerPrefs.GetInt("Character_" + Index + "_currentXP");
        missionsCompleted = PlayerPrefs.GetInt("Character_" + Index + "_missionsCompleted");
        //Debug.Log(string.Join(",", abilities.Select(x => x.ToString()).ToArray()));
        string abilitiesString;
        abilitiesString = PlayerPrefs.GetString("Character_" + Index + "_abilities");
        convertStringToAbilities(abilitiesString);
    }

    public void Delete()
    {
        PlayerPrefs.DeleteKey("Character_" + Index + "_name");
        PlayerPrefs.DeleteKey("Character_" + Index + "_maxHP");
        PlayerPrefs.DeleteKey("Character_" + Index + "_maxMana");
        PlayerPrefs.DeleteKey("Character_" + Index + "_currentXP");
        PlayerPrefs.DeleteKey("Character_" + Index + "_missionsCompleted");
        PlayerPrefs.DeleteKey("Character_" + Index + "_abilities");
    }

    private void convertStringToAbilities(string abilitiesString)
    {
        string[] booleanChars = abilitiesString.Split(',');
        foreach(string boolChar in booleanChars)
        {
            abilities.Add(Int32.Parse(boolChar));
        }
        //Debug.Log("abilities deserialized " + string.Join(",", abilities.Select(x => x.ToString()).ToArray()));
    }

    public string Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
        }
    }

    public int MaxHP
    {
        get
        {
            return maxHP;
        }

        set
        {
            maxHP = value;
        }
    }

    public int MaxMana
    {
        get
        {
            return maxMana;
        }

        set
        {
            maxMana = value;
        }
    }

    public List<int> Abilities
    {
        get
        {
            return abilities;
        }

        set
        {
            abilities = value;
        }
    }

    public int Index
    {
        get
        {
            return index;
        }

        set
        {
            index = value;
        }
    }

    public int CurrentXP
    {
        get
        {
            return currentXP;
        }

        set
        {
            currentXP = value;
        }
    }

    public int MissionsCompleted
    {
        get
        {
            return missionsCompleted;
        }

        set
        {
            missionsCompleted = value;
        }
    }

    internal CharacterType CharType
    {
        get
        {
            return charType;
        }

        set
        {
            charType = value;
        }
    }
}

