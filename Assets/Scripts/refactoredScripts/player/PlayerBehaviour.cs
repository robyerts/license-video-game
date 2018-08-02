using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerBehaviour : CharacterBehaviour
{
    public string DeathAnim;
    public string GotHitAnim;
    public Transform currentEnemyTransform; // enemy has smth similar... // consider removing this
    public GameObject currentEnemy;
    public Text nameLabel;
    public SimpleHealthBar manaBar;
    public GameObject RPGCharUI;

    protected PlayerCharInfo charInfo;
    protected bool isAttacking;
    protected int attackNr = 0;
    protected int currentMana;

    public List<GameObject> abilitiesPrefabs;
    public List<Transform> abilitiesInstatiateTr;

    protected List<string> abilitiesAnimNames;
    protected List<string> abilitiesNames;
    protected List<int> abilitiesManaCosts;
    protected List<int> abilitiesDmgs;
    protected List<Sprite> abilitiesIcons;
    protected int maxNrAbilitiesUI = 4;

    protected List<GameObject> abilitiesButtons;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        charInfo = PlayerGameData2.Instance.CharInfo;
        nameLabel.text = charInfo.Name;
        currentEnemy = BattleManager2.Instance.Enemies[0];
        currentEnemyTransform = currentEnemy.transform;

        isAttacking = false;
        currentHealth = charInfo.MaxHP;
        currentMana = charInfo.MaxMana;
        healthBar.UpdateBar(currentHealth, currentHealth);
        manaBar.UpdateBar(currentMana, currentMana);

        abilitiesAnimNames = PlayerGameData2.Instance.AbilitiesAnimNames;
        abilitiesNames = PlayerGameData2.Instance.AbilitiesNames;
        abilitiesDmgs = PlayerGameData2.Instance.AbilitiesDmg;
        abilitiesIcons = PlayerGameData2.Instance.AbilitiesIcons;
        abilitiesManaCosts = PlayerGameData2.Instance.AbilitiesManaCosts;
        maxNrAbilitiesUI = GameSettings2.MaxNrAbilitiesInBattle;


        abilitiesButtons = new List<GameObject>();

        for (int i = 0; i < abilitiesNames.Count; i++)
        {
            abilitiesButtons.Add(RPGCharUI.transform.Find("Ability" + i).gameObject);
        }

        populateUIButtons();
    }

    protected void populateUIButtons()
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
                btn.onClick.AddListener(delegate { StartAttack(a); });
                btnIndex++;
                //it shouldn't get here
                // don't allow more than maxNrAbilitiesUI to be stored!
                // constraint to be added
                if (btnIndex >= maxNrAbilitiesUI)
                    break;
            }
        }
        // disable the rest of remaining buttons
        for (int i = btnIndex; i < maxNrAbilitiesUI; i++)
        {
            abilitiesButtons[i].SetActive(false);
        }

    }

    public virtual void StartAttack(int attackNr)
    {

    }

    public virtual void Hit(int dmg)
    {

    }

    public virtual void EndHit()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); // altough base function has no code in it
    }

    protected void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside PlayerBehaviour OnCollisionEnter");
        int projectileIndex = BattleManager2.Instance.EnemiesProjectileTags.FindIndex(proj => proj.CompareTo(c.gameObject.tag) == 0);
        if (projectileIndex != -1)
        {
            currentHealth -= BattleManager2.Instance.EnemiesProjectileDmgs[projectileIndex];
            healthBar.UpdateBar(currentHealth, charInfo.MaxHP);
            reactToTakenDmg();
        }
    }
    private void reactToTakenDmg()
    {
        if (currentHealth <= 0)
        {
            //anim.enabled = false;
            anim.Play(DeathAnim);
            BattleManager2.Instance.setGameOver();
        }
        else
        {
            anim.Play(GotHitAnim);
            StartCoroutine(RepositionFromHit());
        }
    }

    // add force as parameter
    public void GetHit(int dmg)
    {
        Debug.Log("Inside Player GetHit");
        rb.AddForce(-transform.forward * 10000, ForceMode.Impulse);

        currentHealth -= dmg;
        healthBar.UpdateBar(currentHealth, charInfo.MaxHP);
        reactToTakenDmg();
    }
}
