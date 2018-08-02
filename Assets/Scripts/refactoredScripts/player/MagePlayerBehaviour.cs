using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagePlayerBehaviour : PlayerBehaviour
{
    private List<MageAbilityInstantiateType> abilitiesTypes;
    private List<string> abilitiesInstatiateTrTags;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        abilitiesTypes = MagePlayerGameData2.Instance.AbilitiesTypes;
        abilitiesInstatiateTrTags = MagePlayerGameData2.Instance.AbilitiesInstatiateTrTags;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isRepositioningFromHit && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            //anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
        }
    }

    public override void StartAttack(int attackNr)
    {
        if (currentMana - abilitiesManaCosts[attackNr] <= 0)
        {
            return;
        }
        BattleManager2.Instance.ActivateGrayButtons();
        this.attackNr = attackNr;
        isAttacking = true;

        currentMana -= abilitiesManaCosts[attackNr];
        manaBar.UpdateBar(currentMana, charInfo.MaxMana);

        anim.Play(abilitiesAnimNames[attackNr]);
    }

    public override void Hit(int abilityPower)
    {
        GameObject instatiatedAbility;
        
        switch(abilitiesTypes[attackNr])
        {
            case MageAbilityInstantiateType.DmgIntantiateOnEnemy:
                Transform enemyInstatiateTr = getChildWithTag(currentEnemy.transform, abilitiesInstatiateTrTags[attackNr]).transform;
                instatiatedAbility = Instantiate(abilitiesPrefabs[attackNr], enemyInstatiateTr.position, enemyInstatiateTr.rotation);
                StartCoroutine(ProcessAbility(instatiatedAbility, abilityPower, 1.2f, 2.1f, MageAbilityType.Dmg)); // hardcoded numbers and some bellow aswell
                break;

            case MageAbilityInstantiateType.DmgProjectileWithCollider:
                Vector3 relativePos = currentEnemyTransform.position - abilitiesInstatiateTr[attackNr].position;
                abilitiesInstatiateTr[attackNr].rotation = Quaternion.LookRotation(relativePos);
                abilitiesInstatiateTr[attackNr].eulerAngles = new Vector3(0, abilitiesInstatiateTr[attackNr].eulerAngles.y, abilitiesInstatiateTr[attackNr].eulerAngles.z);

                instatiatedAbility = Instantiate(abilitiesPrefabs[attackNr], abilitiesInstatiateTr[attackNr].position, abilitiesInstatiateTr[attackNr].rotation);
                StartCoroutine(ProcessAbility(instatiatedAbility, 0, 0, 1.5f, MageAbilityType.NoAbility));
                break;

            case MageAbilityInstantiateType.HealingInstatiateOnSelf:
                instatiatedAbility = Instantiate(abilitiesPrefabs[attackNr], abilitiesInstatiateTr[attackNr].position, abilitiesInstatiateTr[attackNr].rotation);
                StartCoroutine(ProcessAbility(instatiatedAbility, abilityPower, 1.2f, 2.1f, MageAbilityType.Healing));
                break;

            case MageAbilityInstantiateType.DmgAOEInstatiateOnEnemies:
                instatiatedAbility = Instantiate(abilitiesPrefabs[attackNr], abilitiesInstatiateTr[attackNr].position, abilitiesInstatiateTr[attackNr].rotation);
                StartCoroutine(ProcessAbility(instatiatedAbility, abilityPower, 1.2f, 2.1f, MageAbilityType.AOEDmg));
                break;
        }
    }

    GameObject getChildWithTag(Transform parentTr, string childTag)
    {
        foreach(Transform child in parentTr)
        {
            if(child.tag == childTag)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    IEnumerator DestroyWithDelay(GameObject objectToDestroy, float timeBe4Destroy)
    {
        yield return new WaitForSeconds(timeBe4Destroy);
        Destroy(objectToDestroy);
    }

    IEnumerator ApplyDmgWithDelay(GameObject instatiatedAbility, int dmg, float timeBe4Attack)
    {
        yield return new WaitForSeconds(timeBe4Attack);
        EnemyBehaviour enemyScript = currentEnemy.GetComponent<EnemyBehaviour>();
        enemyScript.GetHit(dmg, 6000); // !! hardcoded number!!
    }

    IEnumerator ApplyAOEDmgWithDelay(GameObject instatiatedAbility, int dmg, float timeBe4Attack)
    {
        yield return new WaitForSeconds(timeBe4Attack);
        List<GameObject> enemies = new List<GameObject>(BattleManager2.Instance.Enemies);
        foreach (GameObject enemy in enemies)
        {
            EnemyBehaviour enemyScript = currentEnemy.GetComponent<EnemyBehaviour>();
            enemyScript.GetHit(dmg, 6000); // !! hardcoded number!!
        }
    }

    IEnumerator ApplyHealingWithDelay(GameObject instatiatedAbility, int healing, float timeBe4Healing)
    {
        yield return new WaitForSeconds(timeBe4Healing);

        this.currentHealth += healing;
        if (currentHealth > charInfo.MaxHP)
        {
            currentHealth = charInfo.MaxHP;
        }
        healthBar.UpdateBar(currentHealth, charInfo.MaxHP);
    }

    IEnumerator ProcessAbility(GameObject instatiatedAbility, int abilityPower, float timeBe4Attack, float timeBe4Destroy, MageAbilityType mageAbilityType)
    {
        switch (mageAbilityType)
        {
            case MageAbilityType.AOEDmg:
                yield return ApplyAOEDmgWithDelay(instatiatedAbility, abilityPower, timeBe4Attack);
                break;
            case MageAbilityType.Dmg:
                yield return ApplyDmgWithDelay(instatiatedAbility, abilityPower, timeBe4Attack);
                break;
            case MageAbilityType.Healing:
                yield return ApplyHealingWithDelay(instatiatedAbility, abilityPower, timeBe4Attack);
                break;
        }
        
        yield return DestroyWithDelay(instatiatedAbility, timeBe4Destroy - timeBe4Attack);
        yield return new WaitForSeconds(1); // !! hardcoded number!!
        BattleManager2.Instance.EnemiesAttacksOnGoing = true;
    }

    //    IEnumerator AttackCompleted()
    //    {
    //        yield return new WaitForSeconds(1); //hardcoded number -> expose it to the editor
    //                                            // add an array of floats with time to complete for each ability
    //        isAttacking = false;
    //    }

}
