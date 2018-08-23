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
    private int missionsCompleted;
    private List<int> abilities;

    public static int startingHP = 100;
    public static int startingMana = 60;
    public static int nrAbilities = 6;

    public PlayerCharInfo(int index, string name, CharacterType charType)
    {
        this.charType = charType;
        this.index = index;
        this.name = name;
        this.maxHP = startingHP;
        this.maxMana = startingMana;
        this.missionsCompleted = 0;

        SetAbilities(2);
    }

    public PlayerCharInfo(int index, string name, int maxHP, int maxMana, int missionsCompleted, List<int> abilities, CharacterType charType)
    {
        this.charType = charType;
        this.index = index;
        this.name = name;
        this.maxHP = maxHP;
        this.maxMana = maxMana;
        this.missionsCompleted = missionsCompleted;
        this.abilities = abilities;

    }

    public void SetAbilities(int startingAbilities)
    {
        this.abilities = new List<int>();
        for(int i = 0; i < nrAbilities; i++)
        {
            if(i < startingAbilities)
            {
                abilities.Add(1);
            } else
            {
                abilities.Add(0);
            }
        }
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

    public CharacterType CharType
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

