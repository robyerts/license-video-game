using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Ability
{
    private string animName;
    private float rangeFromEnemy;
    private int manaCost;

    public string AnimName
    {
        get
        {
            return animName;
        }

        set
        {
            animName = value;
        }
    }

    public float RangeFromEnemy
    {
        get
        {
            return rangeFromEnemy;
        }

        set
        {
            rangeFromEnemy = value;
        }
    }

    public int ManaCost
    {
        get
        {
            return manaCost;
        }

        set
        {
            manaCost = value;
        }
    }
}

