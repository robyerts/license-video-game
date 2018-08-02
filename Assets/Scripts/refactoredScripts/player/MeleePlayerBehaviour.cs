using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleePlayerBehaviour : PlayerBehaviour
{
    public List<float> abilitiesAttackRanges;
    public List<int> abilitiesAttackForces;

    private bool attackAnimPlayed;
    private bool hasToRotateBackwards;
    private bool isReturning;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        abilitiesAttackRanges = MeleePlayerGameData2.Instance.AbilitiesAttackRanges;
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
        if (currentMana - abilitiesManaCosts[attackNr] <= 0)
        {
            return;
        }
        BattleManager2.Instance.ActivateGrayButtons();
        this.attackNr = attackNr;
        isAttacking = true;

        currentMana -= abilitiesManaCosts[attackNr];
        manaBar.UpdateBar(currentMana, charInfo.MaxMana);

        anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(currentEnemyTransform.position);
    }

    public override void Hit(int dmg)
    {
        EnemyBehaviour enemyScript = currentEnemy.GetComponent<EnemyBehaviour>();
        enemyScript.GetHit(dmg, abilitiesAttackForces[attackNr]);
      
    }

    public override void EndHit()
    {
        hasToRotateBackwards = true;
        isAttacking = false;
    }
}
