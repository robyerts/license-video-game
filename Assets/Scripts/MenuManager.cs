using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class MenuManager : MonoBehaviour {
    public static MenuManager instance = null;

    [SerializeField] private Text charName;
    [SerializeField] private Dropdown charTypeDropdown;
    [SerializeField] private GameObject newCharNamePanel;
    [SerializeField] private GameObject charsPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private List<Text> emptySlotLabels;
    [SerializeField] private List<GameObject> characterSlots;
    [SerializeField] private GameObject playerGameDataObj;
    private PlayerCharInfoStorage charStorage;

    private static int maxNrChars = 4; // UI hardcoded // equals to charSlots.size
    private static int charIndex;
    private static PlayerCharInfo charInfo;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start () {
        charStorage = new PlayerPrefStorage();
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
                PlayerCharInfo character = charStorage.LoadChar(index);
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

        Text charType = characterSlots[index].transform.Find("CharType").gameObject.GetComponent<Text>();

        if(character.CharType == CharacterType.MeleeCharacter)
        {
            charType.text = "Melee";
        } else
        {
            charType.text = "Ranged";
        }

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
        
        if(charTypeDropdown.value == 0)
        {
            charInfo = new PlayerCharInfo(indexAvailableSlot, charName.text, CharacterType.RangedCharacter);
        } else
        {
            charInfo = new PlayerCharInfo(indexAvailableSlot, charName.text, CharacterType.MeleeCharacter);
        }
        charStorage.SaveChar(charInfo);

        setUpPlayerGameData();

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
        //character.Save();
    }

    public void deleteCharacter(int index)
    {
        //ask confirmation windows
        charStorage.DeleteChar(index);
        emptySlotLabels[index].gameObject.SetActive(true);
        characterSlots[index].SetActive(false);
    }

    public void loadCharacter(int index)
    {
        charIndex = index;
        setUpPlayerGameData();

        disableCharsPanel();
    }

    public void setUpPlayerGameData()
    {
        charInfo = charStorage.LoadChar(charIndex);
        if (charInfo.CharType == CharacterType.MeleeCharacter)
        {
            MeleePlayerGameData2 meleePlayerGameData2Component = playerGameDataObj.GetComponent<MeleePlayerGameData2>();
            MeleePlayerGameData2.Instance = meleePlayerGameData2Component;
            PlayerGameData2.Instance = meleePlayerGameData2Component;
        }
        else
        {
            MagePlayerGameData2 magePlayerGameData2Component = playerGameDataObj.GetComponent<MagePlayerGameData2>();
            MagePlayerGameData2.Instance = magePlayerGameData2Component;
            PlayerGameData2.Instance = magePlayerGameData2Component;
        }
        PlayerGameData2.GameObjectInstances = 1;
        PlayerGameData2.Instance.CharInfo = charInfo;
    }

    public void disableCharsPanel()
    {
        newCharNamePanel.SetActive(false);
        charsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void enableCharsPanel()
    {
        newCharNamePanel.SetActive(true);
        charsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        //updatechar you played with

    }

}
