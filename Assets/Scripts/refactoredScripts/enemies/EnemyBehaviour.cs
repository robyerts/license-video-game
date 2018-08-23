using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : CharacterBehaviour {
    public int MaxHealth;
    public GameObject Player { get; set; }

    public List<int> abilitiesProbabilities;
    public List<string> abilitiesAnimNames;
    public List<int> abilitiesPowers;

    protected bool isDead;
    protected int attackNr;

    private System.Random randomGenerator;
    private int sumProbabilities;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        isDead = false;

        currentHealth = MaxHealth;
        healthBar.UpdateBar(currentHealth, MaxHealth);

        initializeAbilitiesRandomizer();
    }

    private void initializeAbilitiesRandomizer()
    {
        randomGenerator = new System.Random();

        sumProbabilities = 1;
        foreach (int prob in abilitiesProbabilities)
        {
            sumProbabilities += prob;
        }
    }

    // Update is called once per frame
    protected override void Update () {
		
	}

    protected int selectRandomAbilityIndex()
    {
        int selectedNr = randomGenerator.Next(1, sumProbabilities);
        int sum = 0;
        int index = 0;

        while (sum < selectedNr)
        {
            sum += abilitiesProbabilities[index++];
        }

        return index - 1;
    }

    public void GetHit(int dmg, int force)
    {
        Debug.Log("Inside Sorceress GetHit");
        rb.AddForce(-transform.forward * force, ForceMode.Impulse);

        currentHealth -= dmg;
        healthBar.UpdateBar(currentHealth, MaxHealth);

        reactToTakenDmg();
    }

    protected void OnCollisionEnter(Collision c)
    {
        Debug.Log(c.gameObject.tag + " - inside EnemyBehaviour OnCollisionEnter");
        if (MagePlayerGameData2.Instance == null)
        {
            return;
        }

        int projectileIndex = MagePlayerGameData2.Instance.ProjectilesTags.FindIndex(proj => proj.CompareTo(c.gameObject.tag) == 0);
        if (projectileIndex != -1)
        {
            currentHealth -= MagePlayerGameData2.Instance.ProjectilesDmgs[projectileIndex];
            healthBar.UpdateBar(currentHealth, MaxHealth);

            reactToTakenDmg();
        }
    }
    private void reactToTakenDmg()
    {
        if (currentHealth <= 0)
        {
            anim.enabled = false;
            BattleManager2.Instance.RemoveDeadEnemy(this.gameObject);
            isDead = true;
            StartCoroutine(SelfDisable());
        }
        else
        {
            StartCoroutine(RepositionFromHit());
        }
    }
    public virtual void StartAttack()
    {

    }
    protected IEnumerator SelfDisable()
    {
        yield return new WaitForSeconds(0.8f); // create public field to set this in editor
        //ugly code dependand on the simple Health bar framework
        healthBar.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);

    }
}
