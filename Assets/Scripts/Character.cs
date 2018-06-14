using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Character
{
    private string name;
    private int maxHP;
    private int maxMana;
    //private int xp;
    private List<int> abilities;

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
}

