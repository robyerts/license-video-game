using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager2 : MonoBehaviour
{
    public static BattleManager2 Instance = null;

    [SerializeField] private GameObject magePlayerChar;
    [SerializeField] private GameObject meleePlayerChar;
    private GameObject playerChar; 
    private PlayerBehaviour playerScript;
    [SerializeField] private Button endGameBtn;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private List<Button> grayButtons; // use RPG-CharacterUI
    private int indexCurrentAttackingEnemy = -1;
    private PlayerCharInfoStorage charStorage;

    public bool GameOver { get; set; }
    public List<GameObject> Enemies;
    public bool EnemyFinishedAttack { get; set; }
    public bool EnemiesAttacksOnGoing { get; set; }
    public bool MissionSucceeded { get; set; }

    // public accesor mandatory
    public int NewAbilityIndex = -1;
    public int IncreseHPBy = 20;
    public int IncreseManaBy = 15;
    public int MissionNumber = 1;

    public List<string> EnemiesProjectileTags { get; set; }
    public List<int> EnemiesProjectileDmgs { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        GameOver = false;
        EnemyFinishedAttack = true;
        EnemiesAttacksOnGoing = false;
        MissionSucceeded = false;

        charStorage = new PlayerPrefStorage();

        if(PlayerGameData2.Instance.CharInfo.CharType == CharacterType.MeleeCharacter)
        {
            meleePlayerChar.SetActive(true);
            playerChar = meleePlayerChar;
        } else
        {
            magePlayerChar.SetActive(true);
            playerChar = magePlayerChar;
        }

        EnemiesProjectileTags = new List<string>();
        EnemiesProjectileDmgs = new List<int>();

        playerScript = playerChar.GetComponent<PlayerBehaviour>();
        playerScript.currentEnemy = Enemies[0];

        initializeEnemiesPlayerField();

        populateEnemiesProjectiles();
    }

    private void initializeEnemiesPlayerField()
    {
        foreach(GameObject enemy in Enemies)
        {
            enemy.GetComponent<EnemyBehaviour>().Player = playerChar;
        }
    }
    
    private void populateEnemiesProjectiles()
    {
        foreach(GameObject enemy in Enemies)
        {
            MageEnemyBehaviour enemySorceressScript = enemy.GetComponent<MageEnemyBehaviour>();
            if(enemySorceressScript != null)
            {
                EnemiesProjectileTags.AddRange(enemySorceressScript.ProjectilesTags);
                EnemiesProjectileDmgs.AddRange(enemySorceressScript.ProjectilesDmgs);
            }
        }
    }

    void Update()
    {
        if (EnemiesAttacksOnGoing)
        {
            if (EnemyFinishedAttack)
            {
                indexCurrentAttackingEnemy++;
                if (indexCurrentAttackingEnemy < Enemies.Count)
                {
                    GameObject enemy = Enemies[indexCurrentAttackingEnemy];
                    EnemyBehaviour enemyScript = enemy.GetComponent<EnemyBehaviour>();
                    
                    EnemyFinishedAttack = false;

                    enemyScript.StartAttack();
                }
                else
                {
                    EnemiesAttacksOnGoing = false;
                    indexCurrentAttackingEnemy = -1;
                    if (!GameOver && Enemies.Count != 0)
                        DeativateGrayButtons();
                }
            }
        }
    }

    public void ActivateGrayButtons()
    {
        foreach (Button b in grayButtons)
        {
            b.gameObject.SetActive(true);
        }
    }

    public void DeativateGrayButtons()
    {
        foreach (Button b in grayButtons)
        {
            b.gameObject.SetActive(false);
        }
    }

    public void RemoveDeadEnemy(GameObject deadEnemy)
    {
        Enemies.Remove(deadEnemy);
        if (Enemies.Count == 0)
        {
            endGameBtn.gameObject.SetActive(true);
            endGameBtn.GetComponentInChildren<Text>().text = "mission succeeded";
            endGameBtn.onClick.AddListener(delegate { enableUpgradeMenu(); });
            MissionSucceeded = true;
            ActivateGrayButtons();
            return;
        }
        PlayerBehaviour playerScript = playerChar.GetComponent<PlayerBehaviour>();
        if (playerScript.currentEnemy == deadEnemy)
        {
            playerScript.currentEnemy = Enemies[0];
            playerScript.currentEnemyTransform = Enemies[0].transform;
        }
    }
    public void enableUpgradeMenu()
    {
        int missionsCompleted = PlayerGameData2.Instance.CharInfo.MissionsCompleted;

        if (missionsCompleted < MissionNumber)
        {
            Transform upgradeAbilityTr = upgradePanel.transform.Find("Ability");

            upgradeAbilityTr.Find("NewAbilityImg").GetComponent<Image>().sprite = getAbilityIcon(NewAbilityIndex);
            upgradeAbilityTr.Find("NewAbilityName").GetComponent<Text>().text = getAbilityName(NewAbilityIndex);
            upgradeAbilityTr.Find("ManaCost").GetComponent<Text>().text = getAbilityManaCost(NewAbilityIndex).ToString();
            upgradeAbilityTr.Find("Dmg").GetComponent<Text>().text = getAbilityDmg(NewAbilityIndex).ToString();

            Transform increaseHPTr = upgradePanel.transform.Find("HP");
            increaseHPTr.Find("Amount").GetComponent<Text>().text = "+" + IncreseHPBy.ToString();

            Transform increaseManaTr = upgradePanel.transform.Find("Mana");
            increaseManaTr.Find("Amount").GetComponent<Text>().text = "+" + IncreseManaBy.ToString();

            endGameBtn.gameObject.SetActive(false);
            upgradePanel.SetActive(true);
        }
        else
        {
            loadMenuScene();
        }

    }
    
    private GameObject getGameObjectByName(Transform parentTransform, string name)
    {
        return parentTransform.Find(name).gameObject;
    }

    private Sprite getAbilityIcon(int abilityIndex)
    {
        return PlayerGameData2.Instance.AbilitiesIcons[abilityIndex];
    }

    private string getAbilityName(int abilityIndex)
    {
        return PlayerGameData2.Instance.AbilitiesNames[abilityIndex];
    }

    private int getAbilityManaCost(int abilityIndex)
    {
        return PlayerGameData2.Instance.AbilitiesManaCosts[abilityIndex];
    }

    private int getAbilityDmg(int abilityIndex)
    {
        return PlayerGameData2.Instance.AbilitiesDmg[abilityIndex];
    }

    public void setGameOver()
    {
        GameOver = true;
        endGameBtn.gameObject.SetActive(true);
        endGameBtn.GetComponentInChildren<Text>().text = "mission failed";
        endGameBtn.onClick.AddListener(delegate { loadMenuScene(); }); //should be by default, but just in case
        MissionSucceeded = false;
    }

    public void applyUpgrade(int choice)
    {
        //0 - ability
        //1 - HP
        //2 - Manass
        PlayerCharInfo character = PlayerGameData2.Instance.CharInfo;
        switch (choice)
        {
            case 0:
                character.Abilities[NewAbilityIndex] = 1;
                break;
            case 1:
                character.MaxHP += IncreseHPBy;
                break;
            case 2:
                character.MaxMana += IncreseManaBy;
                break;
        }
        character.MissionsCompleted++;
        charStorage.SaveChar(character);
        PlayerGameData2.Instance.CharInfo = character; // GetComponent<Player>().Character doesn't return the same instance 
        loadMenuScene();
    }
    public void loadMenuScene()
    {
       
        SceneLoader.instance.loadMenuScene();
    }

}
