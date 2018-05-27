using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [SerializeField] private GameObject player;
    [SerializeField] private Button endGameBtn;
    public List<GameObject> enemies;
    [SerializeField] private List<Button> grayButtons;
    private int indexCurrentAttackingEnemy = -1;

    public bool enemyFinishedAttack = true;
    public bool enemiesAttacksStarted;
  

    void Awake()
    {
        Time.timeScale = 2f;

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        player.GetComponent<Player2>().currentEnemy = enemies[0];
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
                    GetCloseAttack2 meleeEnemy = enemy.GetComponent<GetCloseAttack2>();
                    SorceressAttack mageEnemy = enemy.GetComponent<SorceressAttack>();
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
            return;
        }
        Player2 playerScript = player.GetComponent<Player2>();
        if (playerScript.currentEnemy == deadEnemy)
        {
            playerScript.currentEnemy = enemies[0];
        }
    }
    public void setGameOver()
    {
        endGameBtn.gameObject.SetActive(true);
        endGameBtn.GetComponentInChildren<Text>().text = "mission failed";
    }
}
