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
                Character character = new Character(index);
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

    public void populateCharSlot(Character character)
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
        Character character = new Character(indexAvailableSlot, charName.text);
        character.Save();
        SceneLoader.instance.Character = character;
   
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

    private void modifyCharTest()
    {
        Character character = new Character(0, "fufu");
        character.MaxHP = 120;
        character.MaxMana = 130;
        character.Save();
    }

    public void deleteCharacter(int index)
    {
        //ask confirmation window
        Character character = new Character(index);
        character.Delete();
        emptySlotLabels[index].gameObject.SetActive(true);
        characterSlots[index].SetActive(false);
    }

    public void loadCharacter(int index)
    {
        Character character = new Character(index);
        character.Load();
        SceneLoader.instance.Character = character;
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
