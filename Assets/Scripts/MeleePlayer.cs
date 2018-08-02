using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MeleePlayer : MonoBehaviour
{
    private PlayerCharInfo charInfo;

    private Animator anim;
    private NavMeshAgent navMeshAgent;
    private Rigidbody rb;
    public Transform currentEnemyTransform;
    public GameObject currentEnemy;
    private bool attackAnimPlayed;
    private bool hasToRotateBackwards;
    private bool hasToRotateForwards;
    private int rotationSpeed = 10;
    private bool isReturning;
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
    private List<float> abilitiesAttackRanges;
    private List<int> abilitiesManaCosts;
    private List<int> abilitiesDmg; // currently not used
    private List<Sprite> abilitiesIcons;
    private List<int> abilitiesXpCosts; // currently not used
    private int maxNrAbilitiesUI = 4;

    [SerializeField] private GameObject RPGCharUI;
    private List<GameObject> abilitiesButtons;

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

    public List<float> AbilitiesAttackRanges
    {
        get
        {
            return abilitiesAttackRanges;
        }

        set
        {
            abilitiesAttackRanges = value;
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
        abilitiesAttackRanges = PlayerGameSettings.instance.abilitiesAttackRanges;
        abilitiesDmg = PlayerGameSettings.instance.abilitiesDmg;
        abilitiesIcons = PlayerGameSettings.instance.abilitiesIcons;
        abilitiesManaCosts = PlayerGameSettings.instance.abilitiesManaCosts;
        abilitiesXpCosts = PlayerGameSettings.instance.abilitiesXpCosts;
        maxNrAbilitiesUI = PlayerGameSettings.instance.maxNrAbilitiesInBattle;

        isReturning = false;
        hasToRotateForwards = false;
        hasToRotateBackwards = false;
        attackAnimPlayed = false;
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
        for(int i = 0; i < charInfo.Abilities.Count; i++)
        {
            if(charInfo.Abilities[i] == 1)
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
        if (currentMana - abilitiesManaCosts[attackNr] <= 0)
        {
            return;
        }
        GameManager.instance.ActivateGrayButtons();
        this.attackNr = attackNr;
        isAtacking = true;

        currentMana -= abilitiesManaCosts[attackNr];
        manaBar.UpdateBar(currentMana, charInfo.MaxMana);
        

        anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentEnemyTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAtacking && !attackAnimPlayed && Vector3.Distance(transform.position, currentEnemyTransform.position) < abilitiesAttackRanges[attackNr])
        {
            //Time.timeScale = 0.3f;
            navMeshAgent.isStopped = true;
            anim.SetBool("isRunning", false);
            anim.Play(abilitiesAnimNames[attackNr]);
            attackAnimPlayed = true;
        }

        if (hasToRotateBackwards)
        {
            //Debug.Log(Time.deltaTime);
            Vector3 targetDir = initialTransform.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            // Move our position a step closer to the target.
            if (newDir == transform.forward)
            {
                hasToRotateBackwards = false;
                anim.SetBool("isRunning", true);
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(initialTransform.position);
                isReturning = true;
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        if (isReturning && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            hasToRotateForwards = true;
            isReturning = false;
        }
        if (hasToRotateForwards)
        {
            Vector3 targetDir = idleViewOrientation.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            // Move our position a step closer to the target.
            if (newDir == transform.forward)
            {
                hasToRotateForwards = false;
                attackAnimPlayed = false;
                GameManager.instance.enemiesAttacksOnGoing = true;
                // attack finished - back to idle position
                // reset the remaining variables
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        if (isRepositioningFromHit && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
        }
    }

    //for sorceress attack
    void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside Player OnCollisionEnter");
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
            anim.Play("Unarmed-Death1");
            GameManager.instance.setGameOver();
        }
        else
        {
            anim.Play("Unarmed-GetHit-B1");
            StartCoroutine(RepositionFromHit());
        }
    }

    public void Hit(int dmg)
    {
        MageEnemy sorceressScript = currentEnemy.GetComponent<MageEnemy>();
        MeleeEnemy bruteWarriorScript = currentEnemy.GetComponent<MeleeEnemy>();

        if (sorceressScript)
        {
            sorceressScript.GetHit(dmg, 8000);
        } else
        {
            bruteWarriorScript.GetHit(dmg, 8000);
        }
    }

    public void EndHit()
    {
        hasToRotateBackwards = true;
        isAtacking = false;
    }

    public void GetHit(int dmg)
    {
        Debug.Log("Inside RPG Character GetHit");
        rb.AddForce(-transform.forward * 10000, ForceMode.Impulse);

        currentHealth -= dmg; 
        healthBar.UpdateBar(currentHealth, charInfo.MaxHP);
        reactToCurrentHP();
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
    