using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyBehaviour : EnemyBehaviour {
    public List<GameObject> abilitiesPrefabs;
    public List<Transform> abilitiesInstatiateTr; // assuming there is only one player
    public List<MageAbilityInstantiateType> abilitiesTypes;
    public List<float> abilitiesTimeBe4Attack;
    public List<float> abilitiesTimeBe4Destroy;

    public List<string> ProjectilesTags;
    public List<int> ProjectilesDmgs; // double check if it's really nedeed or could use abilities dmg
                                      // needed to have in battle manager an array of projectiles

    public float delayAfterAbility = 0.5f;

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        //Debug.Log(Vector3.Distance(transform.position, initialTransform.position));
        if (isRepositioningFromHit && (Vector3.Distance(transform.position, initialTransform.position) < 0.1f))
        {
            //Debug.Log("reposition finished - MageEnemyBehaviour");
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
            hasToRotateForwards = true;
            //GameManager.instance.enemiesAttacksOnGoing = true; // no idea why this line was actually for
        }
        if (hasToRotateForwards)
        {
            //Debug.Log(Time.deltaTime);
            Vector3 targetDir = idleViewOrientation.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            // Move our position a step closer to the target.
            if (newDir == transform.forward)
            {
                hasToRotateForwards = false;
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    public void Hit() // assuming there is only one attack anim;
    {
        GameObject instatiatedAbility = Instantiate(abilitiesPrefabs[attackNr], abilitiesInstatiateTr[attackNr].position, abilitiesInstatiateTr[attackNr].rotation);

        switch (abilitiesTypes[attackNr])
        {
            case MageAbilityInstantiateType.DmgIntantiateOnEnemy:
            case MageAbilityInstantiateType.DmgAOEInstatiateOnEnemies:
                //for flamethrower: 1, 2.1
                StartCoroutine(ProcessAbility(instatiatedAbility, abilitiesPowers[attackNr], abilitiesTimeBe4Attack[attackNr], abilitiesTimeBe4Destroy[attackNr], MageAbilityType.Dmg));
                break;

            case MageAbilityInstantiateType.DmgProjectileWithCollider:
                //for firebolt: 0(dmg is applied by the OnCollisionEnter function), 1.5
                StartCoroutine(ProcessAbility(instatiatedAbility, 0, 0, abilitiesTimeBe4Destroy[attackNr], MageAbilityType.NoAbility));
                break;

            case MageAbilityInstantiateType.HealingInstatiateOnSelf:
                //1.2, 2.1
                StartCoroutine(ProcessAbility(instatiatedAbility, abilitiesPowers[attackNr], abilitiesTimeBe4Attack[attackNr], abilitiesTimeBe4Destroy[attackNr], MageAbilityType.Healing));
                break;
        }
    }

    #region mage common functions declarations(and 2 implementations)
    IEnumerator ApplyDmgWithDelay(GameObject instatiatedAbility, int dmg, float timeBe4Attack)
    {
        yield return new WaitForSeconds(timeBe4Attack);
        Player.GetComponent<PlayerBehaviour>().GetHit(dmg);
    }

    IEnumerator DestroyWithDelay(GameObject objectToDestroy, float timeBe4Destroy)
    {
        yield return new WaitForSeconds(timeBe4Destroy);
        Destroy(objectToDestroy);
    }

    IEnumerator ApplyHealingWithDelay(GameObject instatiatedAbility, int healing, float timeBe4Healing)
    {
        yield return new WaitForSeconds(timeBe4Healing);

        this.currentHealth += healing;
        if (currentHealth > MaxHealth)
        {
            currentHealth = MaxHealth;
        }
        healthBar.UpdateBar(currentHealth, MaxHealth);
    }

    //only the last line is diff from mage player.. also has fewer options in the switch case
    IEnumerator ProcessAbility(GameObject instatiatedAbility, int abilityPower, float timeBe4Attack, float timeBe4Destroy, MageAbilityType mageAbilityType)
    {
        switch (mageAbilityType)
        {
            case MageAbilityType.Dmg:
                yield return ApplyDmgWithDelay(instatiatedAbility, abilityPower, timeBe4Attack);
                break;
            case MageAbilityType.Healing:
                yield return ApplyHealingWithDelay(instatiatedAbility, abilityPower, timeBe4Attack);
                break;
        }

        yield return DestroyWithDelay(instatiatedAbility, timeBe4Destroy - timeBe4Attack);
        yield return new WaitForSeconds(delayAfterAbility);
        BattleManager2.Instance.EnemyFinishedAttack = true;
    }
    #endregion

    public override void StartAttack()
    {
        if (BattleManager2.Instance.GameOver)
        {
            BattleManager2.Instance.EnemyFinishedAttack = true;
            return;
        }

        attackNr = selectRandomAbilityIndex();
        //Time.timeScale = 0.1f;
        anim.Play(abilitiesAnimNames[attackNr]);
    }
}
