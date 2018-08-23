using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private GameObject player;
    [SerializeField] private Button endGameBtn;
    [SerializeField] private GameObject upgradePanel;
    public List<GameObject> enemies;
    [SerializeField] private List<Button> grayButtons;
    private int indexCurrentAttackingEnemy = -1;
    public bool gameOver = false;

    public List<string> enemyProjectilesTags;// should move them to PlayerGameSettings probably
    public List<int> enemyProjectilesDmg;   //
    public bool enemyFinishedAttack = true;
    public bool enemiesAttacksOnGoing;
    public bool missionSucceeded = false;

    public int newAbilityIndex = -1;
    public int increseHPBy = 20;
    public int increseManaBy = 15;
    public int missionNumber = 1;

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

    void Start()
    {
        MeleePlayer mlPlayer = player.GetComponent<MeleePlayer>();
        MagePlayer magePlayer = player.GetComponent<MagePlayer>();

        if (mlPlayer)
        {
            mlPlayer.currentEnemy = enemies[0];
        } else
        {
            magePlayer.currentEnemy = enemies[0];
        }
        
    }

    void Update()
    {
        if (enemiesAttacksOnGoing)
        {
            if (enemyFinishedAttack)
            {
                indexCurrentAttackingEnemy++;
                if(indexCurrentAttackingEnemy < enemies.Count)
                {
                    GameObject enemy = enemies[indexCurrentAttackingEnemy];
                    MeleeEnemy meleeEnemy = enemy.GetComponent<MeleeEnemy>();
                    MageEnemy mageEnemy = enemy.GetComponent<MageEnemy>();
                    enemyFinishedAttack = false;
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
                    enemiesAttacksOnGoing = false;
                    indexCurrentAttackingEnemy = -1;
                    if(!gameOver && enemies.Count != 0)
                        DeativateGrayButtons();
                }
            }
        }
    }

    public void ActivateGrayButtons()
    {
        foreach(Button b in grayButtons)
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
        enemies.Remove(deadEnemy);
        if(enemies.Count == 0)
        {
            endGameBtn.gameObject.SetActive(true);
            endGameBtn.GetComponentInChildren<Text>().text = "mission succeeded";
            endGameBtn.onClick.AddListener(delegate { enableUpgradeMenu(); });
            missionSucceeded = true;
            ActivateGrayButtons();
            return;
        }
        MeleePlayer playerScript = player.GetComponent<MeleePlayer>();
        if (playerScript.currentEnemy == deadEnemy)
        {
            playerScript.currentEnemy = enemies[0];
            playerScript.currentEnemyTransform = enemies[0].transform;
        }
    }
    public void enableUpgradeMenu()
    {
        int missionsCompleted = player.GetComponent<MeleePlayer>().CharInfo.MissionsCompleted;
        if (player.GetComponent<MeleePlayer>().CharInfo.MissionsCompleted < missionNumber)
        {
            Transform upgradeAbilityTr = upgradePanel.transform.Find("Ability");
            upgradeAbilityTr.Find("NewAbilityImg").GetComponent<Image>().sprite = player.GetComponent<MeleePlayer>().AbilitiesIcons[newAbilityIndex];
            upgradeAbilityTr.Find("NewAbilityName").GetComponent<Text>().text = player.GetComponent<MeleePlayer>().AbilitiesNames[newAbilityIndex];
            upgradeAbilityTr.Find("ManaCost").GetComponent<Text>().text = player.GetComponent<MeleePlayer>().AbilitiesManaCosts[newAbilityIndex].ToString();
            upgradeAbilityTr.Find("Dmg").GetComponent<Text>().text = player.GetComponent<MeleePlayer>().AbilitiesDmg[newAbilityIndex].ToString();

            Transform increaseHPTr = upgradePanel.transform.Find("HP");
            increaseHPTr.Find("Amount").GetComponent<Text>().text = "+" + increseHPBy.ToString();

            Transform increaseManaTr = upgradePanel.transform.Find("Mana");
            increaseManaTr.Find("Amount").GetComponent<Text>().text = "+" + increseManaBy.ToString();

            endGameBtn.gameObject.SetActive(false);
            upgradePanel.SetActive(true);
        } else
        {
            loadMenuScene();
        }
       
    }
    public void setGameOver()
    {
        gameOver = true;
        endGameBtn.gameObject.SetActive(true);
        endGameBtn.GetComponentInChildren<Text>().text = "mission failed";
        endGameBtn.onClick.AddListener(delegate { loadMenuScene(); }); //should be be default, but just in case
        missionSucceeded = false;
    }
    
    public void applyUpgrade(int choice)
    {
        //0 - ability
        //1 - HP
        //2 - Manass
        PlayerCharInfo character = player.GetComponent<MeleePlayer>().CharInfo;
        switch (choice)
        {
            case 0:
                character.Abilities[newAbilityIndex] = 1;
                break;
            case 1:
                character.MaxHP += increseHPBy;
                break;
            case 2:
                character.MaxMana += increseManaBy;
                break;
        }
        character.MissionsCompleted++;
        //character.Save();
        PlayerGameSettings.instance.CharInfo = character; // it is not the same instance? - GetComponent<Player>().Character doesn't return the same instance?
        loadMenuScene();
    }
    public void loadMenuScene()
    {
        SceneLoader.instance.loadMenuScene();
    }

}
