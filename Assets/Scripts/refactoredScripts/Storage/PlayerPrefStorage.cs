using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlayerPrefStorage : PlayerCharInfoStorage
{
    public override bool DeleteChar(int index)
    {
        PlayerPrefs.DeleteKey("Character_" + index + "_name");
        PlayerPrefs.DeleteKey("Character_" + index + "_maxHP");
        PlayerPrefs.DeleteKey("Character_" + index + "_maxMana");
        PlayerPrefs.DeleteKey("Character_" + index + "_currentXP");
        PlayerPrefs.DeleteKey("Character_" + index + "_missionsCompleted");
        PlayerPrefs.DeleteKey("Character_" + index + "_abilities");

        return true;
    }

    private List<int> convertStringToAbilities(string abilitiesString)
    {
        List<int> abilities = new List<int>();
        string[] booleanChars = abilitiesString.Split(',');
        foreach (string boolChar in booleanChars)
        {
            abilities.Add(Int32.Parse(boolChar));
        }
        return abilities;
        //Debug.Log("abilities deserialized " + string.Join(",", abilities.Select(x => x.ToString()).ToArray()));
    }

    public override PlayerCharInfo LoadChar(int index)
    {
        List<int> abilities = new List<int>();
        CharacterType charType = (CharacterType)Enum.Parse(typeof(CharacterType), PlayerPrefs.GetString("Character_" + index + "_charType"));
        string name = PlayerPrefs.GetString("Character_" + index + "_name");
        int maxHP = PlayerPrefs.GetInt("Character_" + index + "_maxHP");
        int maxMana = PlayerPrefs.GetInt("Character_" + index + "_maxMana");
        int missionsCompleted = PlayerPrefs.GetInt("Character_" + index + "_missionsCompleted");
        string abilitiesString;
        abilitiesString = PlayerPrefs.GetString("Character_" + index + "_abilities");
        abilities = convertStringToAbilities(abilitiesString);

        PlayerCharInfo charInfo = new PlayerCharInfo(index, name, maxHP, maxMana, missionsCompleted, abilities, charType);
        return charInfo;
    }

    public override bool SaveChar(PlayerCharInfo charInfo)
    {
        PlayerPrefs.SetString("Character_" + charInfo.Index + "_name", charInfo.Name);
        PlayerPrefs.SetString("Character_" + charInfo.Index + "_charType", charInfo.CharType.ToString());
        PlayerPrefs.SetInt("Character_" + charInfo.Index + "_maxHP", charInfo.MaxHP);
        PlayerPrefs.SetInt("Character_" + charInfo.Index + "_maxMana", charInfo.MaxMana);
        PlayerPrefs.SetInt("Character_" + charInfo.Index + "_missionsCompleted", charInfo.MissionsCompleted);
        //Debug.Log(string.Join(",", abilities.Select(x => x.ToString()).ToArray()));
        PlayerPrefs.SetString("Character_" + charInfo.Index + "_abilities", string.Join(",", charInfo.Abilities.Select(x => x.ToString()).ToArray()));

        return true;
    }
}
