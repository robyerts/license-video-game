using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager2 : MonoBehaviour
{
    public static BattleManager2 Instance = null;

    [SerializeField] private GameObject player; // remove player instance; use prefab to INSTANTIATE
    private MagePlayer playerScript;
    [SerializeField] private Button endGameBtn;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private List<Button> grayButtons; // use RPG-CharacterUI
    private int indexCurrentAttackingEnemy = -1;

    public bool GameOver = false;
    public List<GameObject> Enemies;
    public bool EnemyFinishedAttack = true;
    public bool EnemiesAttacksOnGoing = false;
    public bool MissionSucceeded = false;

    // public accesor mandatory
    public int NewAbilityIndex = -1;
    public int IncreseHPBy = 20;
    public int IncreseManaBy = 15;
    public int MissionNumber = 1;

    public List<string> EnemiesProjectileTags;
    public List<int> EnemiesProjectileDmgs;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        EnemiesProjectileTags = new List<string>();
        EnemiesProjectileDmgs = new List<int>();

        playerScript = player.GetComponent<MagePlayer>();
        //REFACTOR using base-class
        MeleePlayer mlPlayer = player.GetComponent<MeleePlayer>();
        MagePlayer magePlayer = player.GetComponent<MagePlayer>();

        if (mlPlayer)
        {
            mlPlayer.currentEnemy = Enemies[0];
        }
        else
        {
            magePlayer.currentEnemy = Enemies[0];
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
                    MeleeEnemy meleeEnemy = enemy.GetComponent<MeleeEnemy>();
                    MageEnemy mageEnemy = enemy.GetComponent<MageEnemy>();
                    EnemyFinishedAttack = false;
                    if (meleeEnemy != null)
                    {
                        meleeEnemy.StartBasicAttack();
                    }
                    else
                    {
                        mageEnemy.StartBasicAttack();
                    }
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
        MagePlayer playerScript = player.GetComponent<MagePlayer>();
        if (playerScript.currentEnemy == deadEnemy)
        {
            playerScript.currentEnemy = Enemies[0];
            playerScript.currentEnemyTransform = Enemies[0].transform;
        }
    }
    public void enableUpgradeMenu()
    {
        MagePlayer playerScript = player.GetComponent<MagePlayer>();
        int missionsCompleted = playerScript.CharInfo.MissionsCompleted;

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
        return playerScript.AbilitiesIcons[abilityIndex];
    }

    private string getAbilityName(int abilityIndex)
    {
        return playerScript.AbilitiesNames[abilityIndex];
    }

    private int getAbilityManaCost(int abilityIndex)
    {
        return playerScript.AbilitiesManaCosts[abilityIndex];
    }

    private int getAbilityDmg(int abilityIndex)
    {
        return playerScript.AbilitiesDmg[abilityIndex];
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
        PlayerCharInfo character = player.GetComponent<MagePlayer>().CharInfo;
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
        character.Save();
        PlayerGameData2.Instance.CharInfo = character; // GetComponent<Player>().Character doesn't return the same instance 
        loadMenuScene();
    }
    public void loadMenuScene()
    {
        SceneLoader.instance.loadMenuScene();
    }

}
