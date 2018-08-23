using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleePlayerBehaviour : PlayerBehaviour
{
    private List<float> abilitiesAttackRanges;
    private List<MeleeAbilityInstantiateType> abilitiesTypes;

    private bool attackAnimPlayed;
    private bool hasToRotateBackwards;
    private bool isReturning;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        abilitiesAttackRanges = MeleePlayerGameData2.Instance.AbilitiesAttackRanges;
        abilitiesTypes = MeleePlayerGameData2.Instance.AbilitiesTypes;
        attackAnimPlayed = false;
        hasToRotateBackwards = false;
        isReturning = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isAttacking && !attackAnimPlayed && Vector3.Distance(transform.position, currentEnemyTransform.position) < abilitiesAttackRanges[attackNr])
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
                BattleManager2.Instance.EnemiesAttacksOnGoing = true;
                // attack finished - back to idle position; let enemies start their attack aswell
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

    public override void StartAttack(int attackNr)
    {
        if (currentMana - abilitiesManaCosts[attackNr] < 0)
        {
            return;
        }
        BattleManager2.Instance.ActivateGrayButtons();
        this.attackNr = attackNr;

        currentMana -= abilitiesManaCosts[attackNr];
        manaBar.UpdateBar(currentMana, charInfo.MaxMana);

        switch (abilitiesTypes[attackNr])
        {
            case MeleeAbilityInstantiateType.NoInstatiate:
                isAttacking = true;
                anim.SetBool("isRunning", true);
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(currentEnemyTransform.position);
                break;
            case MeleeAbilityInstantiateType.HealingInstatiateOnSelf:
                anim.Play(abilitiesAnimNames[attackNr]);
                break;
        }
       
       
       
    }

    // parameter probably not needed since can there is a list for it
    public override void Hit()
    {
        switch (abilitiesTypes[attackNr])
        {
            case MeleeAbilityInstantiateType.NoInstatiate:
                EnemyBehaviour enemyScript = currentEnemy.GetComponent<EnemyBehaviour>();
                enemyScript.GetHit(abilitiesDmgs[attackNr], abilitiesAttackForces[attackNr]);
                break;
            case MeleeAbilityInstantiateType.HealingInstatiateOnSelf:
                GameObject instatiatedAbility = Instantiate(abilitiesPrefabs[attackNr], abilitiesInstatiateTr[attackNr].position, abilitiesInstatiateTr[attackNr].rotation);
                StartCoroutine(ProcessAbility(instatiatedAbility, abilitiesDmgs[attackNr], abilitiesTimeBe4Attack[attackNr], abilitiesTimeBe4Destroy[attackNr], abilitiesTypes[attackNr]));
                break;
        }
      
    }

    IEnumerator ProcessAbility(GameObject instatiatedAbility, int abilityPower, float timeBe4Attack, float timeBe4Destroy, MeleeAbilityInstantiateType meleeAbilityInstantiateType)
    {
        switch (meleeAbilityInstantiateType)
        {

            case MeleeAbilityInstantiateType.HealingInstatiateOnSelf:
                yield return ApplyHealingWithDelay(instatiatedAbility, abilityPower, timeBe4Attack);
                break;
        }

        yield return DestroyWithDelay(instatiatedAbility, timeBe4Destroy - timeBe4Attack);
        yield return new WaitForSeconds(1); // !! hardcoded number!!
        BattleManager2.Instance.EnemiesAttacksOnGoing = true;
    }

    IEnumerator DestroyWithDelay(GameObject objectToDestroy, float timeBe4Destroy)
    {
        yield return new WaitForSeconds(timeBe4Destroy);
        Destroy(objectToDestroy);
    }

    public override void EndHit()
    {
        hasToRotateBackwards = true;
        isAttacking = false;
    }
}
