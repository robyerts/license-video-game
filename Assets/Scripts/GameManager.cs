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

    public List<string> enemyProjectilesTags;
    public List<int> enemyProjectilesDmg;
    public bool enemyFinishedAttack = true;
    public bool enemiesAttacksStarted;
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
        player.GetComponent<Player>().currentEnemy = enemies[0];
    }

    void Update()
    {
        if (enemiesAttacksStarted)
        {
            if (enemyFinishedAttack)
            {
                indexCurrentAttackingEnemy++;
                if(indexCurrentAttackingEnemy < enemies.Count)
                {
                    GameObject enemy = enemies[indexCurrentAttackingEnemy];
                    MeleeEnemy meleeEnemy = enemy.GetComponent<MeleeEnemy>();
                    RangedEnemy mageEnemy = enemy.GetComponent<RangedEnemy>();
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
                    enemiesAttacksStarted = false;
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
        Player playerScript = player.GetComponent<Player>();
        if (playerScript.currentEnemy == deadEnemy)
        {
            playerScript.currentEnemy = enemies[0];
        }
    }
    public void enableUpgradeMenu()
    {
        int missionsCompleted = player.GetComponent<Player>().Character.MissionsCompleted;
        if (player.GetComponent<Player>().Character.MissionsCompleted < missionNumber)
        {
            Transform upgradeAbilityTr = upgradePanel.transform.Find("Ability");
            upgradeAbilityTr.Find("NewAbilityImg").gameObject.GetComponent<Image>().sprite = player.GetComponent<Player>().AbilitiesIcons[newAbilityIndex];
            upgradeAbilityTr.Find("NewAbilityName").gameObject.GetComponent<Text>().text = player.GetComponent<Player>().AbilitiesNames[newAbilityIndex];
            upgradeAbilityTr.Find("ManaCost").gameObject.GetComponent<Text>().text = player.GetComponent<Player>().AbilitiesManaCosts[newAbilityIndex].ToString();
            upgradeAbilityTr.Find("Dmg").gameObject.GetComponent<Text>().text = player.GetComponent<Player>().AbilitiesDmg[newAbilityIndex].ToString();

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
        Character character = player.GetComponent<Player>().Character;
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
        character.Save();
        SceneLoader.instance.Character = character;
        loadMenuScene();
    }
    public void loadMenuScene()
    {
        SceneLoader.instance.loadMenuScene();
    }

}
