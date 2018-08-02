using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MagePlayer : MonoBehaviour
{
    private PlayerCharInfo charInfo;

    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    public Transform currentEnemyTransform;
    public GameObject currentEnemy;
    private bool hasToRotateForwards;
    private int rotationSpeed = 10;
    private bool isAtacking;
    private bool isRepositioningFromHit;
    [SerializeField] private Transform initialTransform;
    [SerializeField] private Transform idleViewOrientation;
    [SerializeField] private Text nameLabel;
    private int attackNr = 0;

    [SerializeField] private SimpleHealthBar healthBar;
    private int currentHealth;
    [SerializeField] private SimpleHealthBar manaBar;
    private int currentMana;

    private List<string> abilitiesAnimNames;
    private List<string> abilitiesNames;
    private List<int> abilitiesManaCosts;
    private List<int> abilitiesDmg; 
    private List<Sprite> abilitiesIcons;
    private int maxNrAbilitiesUI = 4;

    [SerializeField] private GameObject ability1Prefab;
    [SerializeField] private Transform ability1InstantiateTransform1;
    [SerializeField] private GameObject ability2Prefab;
    [SerializeField] private Transform ability2InstantiateTransform1;
    [SerializeField] private GameObject ability3Prefab;
    [SerializeField] private Transform ability3InstantiateTransform1;
    [SerializeField] private GameObject ability4Prefab;
    [SerializeField] private Transform ability4InstantiateTransform1;

    [SerializeField] private GameObject RPGCharUI;
    private List<GameObject> abilitiesButtons;

    #region public properties -- useless since i take them from PlayerGameSettings public members
    public PlayerCharInfo CharInfo
    {
        get
        {
            return charInfo;
        }

        set
        {
            charInfo = value;
        }
    }

    public List<string> AbilitiesAnimNames
    {
        get
        {
            return abilitiesAnimNames;
        }

        set
        {
            abilitiesAnimNames = value;
        }
    }

    public List<string> AbilitiesNames
    {
        get
        {
            return abilitiesNames;
        }

        set
        {
            abilitiesNames = value;
        }
    }

    public List<int> AbilitiesManaCosts
    {
        get
        {
            return abilitiesManaCosts;
        }

        set
        {
            abilitiesManaCosts = value;
        }
    }

    public List<Sprite> AbilitiesIcons
    {
        get
        {
            return abilitiesIcons;
        }

        set
        {
            abilitiesIcons = value;
        }
    }

    public List<int> AbilitiesDmg
    {
        get
        {
            return abilitiesDmg;
        }

        set
        {
            abilitiesDmg = value;
        }
    }
    #endregion

    // 0 - basic attack
    // 1 - ability1
    // 2 - ability2 - not yet implemented
    //...

    // Use this for initialization
    void Start()
    {
        //int indexChar = PlayerPrefs.GetInt("currentCharIndex", 0);
        //character = new Character(indexChar);
        //character.Load();
        charInfo = PlayerGameSettings.instance.CharInfo;
        nameLabel.text = charInfo.Name;
        currentEnemyTransform = currentEnemy.transform;
        rb = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        abilitiesAnimNames = PlayerGameSettings.instance.abilitiesAnimNames;
        abilitiesNames = PlayerGameSettings.instance.abilitiesNames;
        abilitiesDmg = PlayerGameSettings.instance.abilitiesDmg;
        abilitiesIcons = PlayerGameSettings.instance.abilitiesIcons;
        abilitiesManaCosts = PlayerGameSettings.instance.abilitiesManaCosts;
        maxNrAbilitiesUI = PlayerGameSettings.instance.maxNrAbilitiesInBattle;

        hasToRotateForwards = false;
        isAtacking = false;
        isRepositioningFromHit = false;
        currentHealth = charInfo.MaxHP;
        currentMana = charInfo.MaxMana;
        healthBar.UpdateBar(currentHealth, currentHealth);
        manaBar.UpdateBar(currentMana, currentMana);
        abilitiesButtons = new List<GameObject>();

        for (int i = 0; i < abilitiesAnimNames.Count; i++)
        {
            abilitiesButtons.Add(RPGCharUI.transform.Find("Ability" + i).gameObject);
        }

        populateUIButtons();
    }

    public void populateUIButtons()
    {
        int btnIndex = 0;
        for (int i = 0; i < charInfo.Abilities.Count; i++)
        {
            if (charInfo.Abilities[i] == 1)
            {
                abilitiesButtons[btnIndex].transform.Find("Mana Cost").gameObject.GetComponent<Text>().text = abilitiesManaCosts[i].ToString();
                abilitiesButtons[btnIndex].GetComponent<Image>().sprite = abilitiesIcons[i];
                Button btn = abilitiesButtons[btnIndex].GetComponent<Button>();
                int a = i;
                abilitiesButtons[btnIndex].GetComponent<Button>().onClick.AddListener(delegate { StartAttack(a); });
                btnIndex++;
                //it shouldn't get here
                // don't allow more than maxNrAbilitiesUI to be stored!
                if (btnIndex >= maxNrAbilitiesUI)
                    break;
            }
        }
        for (int i = btnIndex; i < maxNrAbilitiesUI; i++)
        {
            abilitiesButtons[i].SetActive(false);
        }

    }

    public void StartAttack(int attackNr)
    {
        if(currentMana - abilitiesManaCosts[attackNr] <= 0)
        {
            return;
        }
        GameManager.instance.ActivateGrayButtons();
        this.attackNr = attackNr;
        isAtacking = true;

        currentMana -= abilitiesManaCosts[attackNr];
        manaBar.UpdateBar(currentMana, charInfo.MaxMana);

        anim.Play(abilitiesAnimNames[attackNr]);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRepositioningFromHit && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            //anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
        }
    }

    //for sorceress attack
    void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside MagePlayer OnCollisionEnter");
        int projectileIndex = GameManager.instance.enemyProjectilesTags.FindIndex(proj => proj.CompareTo(c.gameObject.tag) == 0);
        if (projectileIndex != -1)
        {
            currentHealth -= GameManager.instance.enemyProjectilesDmg[projectileIndex];
            healthBar.UpdateBar(currentHealth, charInfo.MaxHP);
            reactToCurrentHP();
        }
    }
    public void reactToCurrentHP()
    {
        if (currentHealth <= 0)
        {
            //anim.enabled = false;
            anim.Play("FallingBackDeath"); // hardcoded
            GameManager.instance.setGameOver();
        }
        else
        {
            anim.Play("BigStomachHit"); // hardcoded
            StartCoroutine(RepositionFromHit());
        }
    }

    public void Hit(int dmg)
    {
        GameObject instatiatedProjectile;
        switch (attackNr)
        {
            case 0:
                instatiatedProjectile = Instantiate(ability1Prefab, ability1InstantiateTransform1.position, ability1InstantiateTransform1.rotation);
                StartCoroutine(ApplyDmgEndAbility(instatiatedProjectile, 100, 1.2f, 2.1f));
                break;
            case 1:
                instatiatedProjectile = Instantiate(ability2Prefab, ability2InstantiateTransform1.position, ability2InstantiateTransform1.rotation);
                StartCoroutine(ApplyDmgEndAbility(instatiatedProjectile, dmg, 1.2f, 2.1f));
                break;
            case 2:
                instatiatedProjectile = Instantiate(ability3Prefab, ability3InstantiateTransform1.position, ability3InstantiateTransform1.rotation);
                StartCoroutine(ApplyHealingEndAbility(instatiatedProjectile, dmg, 1.2f, 2.1f));
                break;
            case 3:
                Vector3 relativePos = currentEnemyTransform.position - ability4InstantiateTransform1.position;
                ability4InstantiateTransform1.rotation = Quaternion.LookRotation(relativePos);
                ability4InstantiateTransform1.eulerAngles = new Vector3(0, ability4InstantiateTransform1.eulerAngles.y, ability4InstantiateTransform1.eulerAngles.z);

                //Time.timeScale = 0.05f;
                instatiatedProjectile = Instantiate(ability4Prefab, ability4InstantiateTransform1.position, ability4InstantiateTransform1.rotation);
                StartCoroutine(DestroyProjectile(instatiatedProjectile, 1.5f));


                //StartCoroutine(ApplyAOEDmgEndAbility(instatiatedAbility, dmg, 1.2f, 2.1f)); // for tornado
                break;
        }  
    }

    IEnumerator DestroyProjectile(GameObject instatiatedProjectile, float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(instatiatedProjectile);
    }

    IEnumerator ApplyDmgEndAbility(GameObject instatiatedAbility, int dmg, float timeToAttack, float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToAttack);
        MageEnemy sorceressScript = currentEnemy.GetComponent<MageEnemy>();
        MeleeEnemy bruteWarriorScript = currentEnemy.GetComponent<MeleeEnemy>();

        if (sorceressScript)
        {
            sorceressScript.GetHit(dmg, 6000);
        }
        else
        {
            bruteWarriorScript.GetHit(dmg, 6000);
        }
        yield return new WaitForSeconds(timeToDestroy - timeToAttack);
        Destroy(instatiatedAbility);
        yield return new WaitForSeconds(1);
        GameManager.instance.enemiesAttacksOnGoing = true;
    }

    IEnumerator ApplyAOEDmgEndAbility(GameObject instatiatedAbility, int dmg, float timeToAttack, float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToAttack);
        List<GameObject> enemies = new List<GameObject>(GameManager.instance.enemies);
        foreach (GameObject enemy in enemies)
        {
            MageEnemy sorceressScript = enemy.GetComponent<MageEnemy>();
            MeleeEnemy bruteWarriorScript = enemy.GetComponent<MeleeEnemy>();

            if (sorceressScript)
            {
                sorceressScript.GetHit(dmg, 6000);
            }
            else
            {
                bruteWarriorScript.GetHit(dmg, 6000);
            }
        }
       
        yield return new WaitForSeconds(timeToDestroy - timeToAttack);
        Destroy(instatiatedAbility);
        yield return new WaitForSeconds(1);
        GameManager.instance.enemiesAttacksOnGoing = true;
    }

    IEnumerator ApplyHealingEndAbility(GameObject instatiatedAbility, int healing, float timeToHeal, float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToHeal); 
        this.currentHealth += healing;
        if(currentHealth > charInfo.MaxHP)
        {
            currentHealth = charInfo.MaxHP;
        }
        healthBar.UpdateBar(currentHealth, charInfo.MaxHP);
        yield return new WaitForSeconds(timeToDestroy - timeToHeal);
        Destroy(instatiatedAbility);
        yield return new WaitForSeconds(1);
        GameManager.instance.enemiesAttacksOnGoing = true;
    }

    public void EndHit()
    {
        //hasToRotateBackwards = true;
        isAtacking = false;
        // we'll see about the coroutine later
        //StartCoroutine(AttackCompleted());
    }

    public void GetHit(int dmg)
    {
        Debug.Log("Inside RPG Character GetHit");
        rb.AddForce(-transform.forward * 10000, ForceMode.Impulse);

        currentHealth -= dmg;
        healthBar.UpdateBar(currentHealth, charInfo.MaxHP);
        reactToCurrentHP();
    }

    IEnumerator AttackCompleted()
    {
        yield return new WaitForSeconds(1); //hardcoded number -> expose it to the editor
                                            // add an array of floats with time to complete for each ability
        isAtacking = false;
    }

    IEnumerator RepositionFromHit()
    {
        yield return new WaitForSeconds(0.5f);
        isRepositioningFromHit = true;
        //anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(initialTransform.position);
    }

}
