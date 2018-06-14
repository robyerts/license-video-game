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
    public List<GameObject> enemies;
    [SerializeField] private List<Button> grayButtons;
    private int indexCurrentAttackingEnemy = -1;
    public bool gameOver = false;

    public List<string> enemyProjectilesTags;
    public List<int> enemyProjectilesDmg;
    public bool enemyFinishedAttack = true;
    public bool enemiesAttacksStarted;
   
  

    void Awake()
    {
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
            ActivateGrayButtons();
            return;
        }
        Player playerScript = player.GetComponent<Player>();
        if (playerScript.currentEnemy == deadEnemy)
        {
            playerScript.currentEnemy = enemies[0];
        }
    }
    public void setGameOver()
    {
        gameOver = true;
        endGameBtn.gameObject.SetActive(true);
        endGameBtn.GetComponentInChildren<Text>().text = "mission failed";
    }

    public void loadMenuScene()
    {
        StartCoroutine(LoadYourAsyncScene("MainMenuScene"));
    }

    IEnumerator LoadYourAsyncScene(string name)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        MenuManager.instance.disableNewCharNamePanel();
    }
}
