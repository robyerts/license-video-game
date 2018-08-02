using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour {
    public static MenuManager instance = null;

    [SerializeField] private Text charName;
    [SerializeField] private GameObject newCharNamePanel;
    [SerializeField] private GameObject charsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private List<Text> emptySlotLabels;
    [SerializeField] private List<GameObject> characterSlots;

    private static int maxNrChars = 4;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        //shouldn't be needed
        //else if (instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
    }

    // Use this for initialization
    void Start () {
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("maxNrCharacters", maxNrChars); //hardcoded & UI aswell
        //modifyCharTest();
        populateCharSlots();
        activateEmptySlotLabels();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void populateCharSlots()
    {
        for(int index = 0; index < maxNrChars; index++)
        {
            if (!isSlotEmpty(index))
            {
                PlayerCharInfo character = new PlayerCharInfo(index);
                character.Load();
                populateCharSlot(character);
            }
        }
    }

    public void activateEmptySlotLabels()
    {
        for (int index = 0; index < maxNrChars; index++)
        {
            if (isSlotEmpty(index))
            {
                emptySlotLabels[index].gameObject.SetActive(true);
                characterSlots[index].SetActive(false);
            }
        }
    }

    public void populateCharSlot(PlayerCharInfo character)
    {
        int index = character.Index;
        Text name = characterSlots[index].transform.Find("CharName").gameObject.GetComponent<Text>();
        name.text = character.Name;

        Text maxHP = characterSlots[index].transform.Find("MaxHP").gameObject.GetComponent<Text>();
        maxHP.text = character.MaxHP.ToString();

        Text maxMana = characterSlots[index].transform.Find("MaxMana").gameObject.GetComponent<Text>();
        maxMana.text = character.MaxMana.ToString();

        Text unlockedAbilities = characterSlots[index].transform.Find("NrUnlockedAbilities").gameObject.GetComponent<Text>();
        unlockedAbilities.text = character.Abilities.Where(x => x == 1).Count().ToString();
    }

    public void confirmName()
    {
        //PlayerPrefs.SetInt("currentCharIndex", indexNewChar);
        int indexAvailableSlot = getAvailableSlotIndex();
        if (indexAvailableSlot == -1)
        {
            return;
            //error window
        }
        PlayerCharInfo character = new PlayerCharInfo(indexAvailableSlot, charName.text, CharacterType.MeleeCharacter);
        character.Save();
        //SceneLoader.instance.Character = character;
        PlayerGameSettings.instance.CharInfo = character;

        PlayerPrefs.SetInt("nrChars", PlayerPrefs.GetInt("nrChars", 0) + 1);
        disableCharsPanel();
    }

    private int getAvailableSlotIndex()
    {
        for(int index = 0; index < maxNrChars; index++)
        {
            string charName = PlayerPrefs.GetString("Character_" + index + "_name", "");
            if (isSlotEmpty(index))
            {
                return index;
            }
        }
        return -1;
    }

    private bool isSlotEmpty(int index)
    {
        string charName = PlayerPrefs.GetString("Character_" + index + "_name", "");
        if (charName.CompareTo("") == 0)
        {
            return true;
        }
        return false;
    }

    //TB later removed
    private void modifyCharTest()
    {
        PlayerCharInfo character = new PlayerCharInfo(0, "fufu", CharacterType.MeleeCharacter);
        character.MaxHP = 120;
        character.MaxMana = 130;
        character.Save();
    }

    public void deleteCharacter(int index)
    {
        //ask confirmation windows
        PlayerCharInfo character = new PlayerCharInfo(index);
        character.Delete();
        emptySlotLabels[index].gameObject.SetActive(true);
        characterSlots[index].SetActive(false);
    }

    public void loadCharacter(int index)
    {
        PlayerCharInfo character = new PlayerCharInfo(index);
        character.Load();
        PlayerGameSettings.instance.CharInfo = character;
        disableCharsPanel();
    }

    public void disableCharsPanel()
    {
        newCharNamePanel.SetActive(false);
        charsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void loadMission1Scene()
    {
        SceneLoader.instance.loadMission1Scene();
    }

}
