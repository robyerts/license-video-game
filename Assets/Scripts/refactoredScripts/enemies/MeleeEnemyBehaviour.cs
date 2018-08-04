using UnityEngine;
using System.Collections;

public class MeleeEnemyBehaviour : EnemyBehaviour
{
    private Transform playerTransform;
    private bool isAtacking;
    private bool attackAnimPlayed;
    private bool hasToRotateBackwards;
    private bool isReturning;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        playerTransform = Player.transform;
        isAtacking = false;
        attackAnimPlayed = false;
        hasToRotateBackwards = false;
        isReturning = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isAtacking && !attackAnimPlayed && Vector3.Distance(transform.position, playerTransform.position) < 4) // hardcoded ability attack range
        {
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
            //Debug.Log(Time.deltaTime);
            Vector3 targetDir = idleViewOrientation.position - transform.position;
            // The step size is equal to speed times frame time.
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            // Move our position a step closer to the target.
            if (newDir == transform.forward)
            {
                hasToRotateForwards = false;
                attackAnimPlayed = false;
                BattleManager2.Instance.EnemyFinishedAttack = true;
            }
            transform.rotation = Quaternion.LookRotation(newDir);
        }
        if (isRepositioningFromHit && Vector3.Distance(transform.position, initialTransform.position) < 0.2)
        {
            anim.SetBool("isRunning", false);
            navMeshAgent.isStopped = true;
            isRepositioningFromHit = false;
            //GameManager.instance.enemiesAttacksOnGoing = true;  // // no idea why this line was actually for
        }
    }

    public override void StartAttack()
    {
        isAtacking = true;
        anim.SetBool("isRunning", true);
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(playerTransform.position);

        attackNr = selectRandomAbilityIndex();
    }

    public void Hit()
    {
        Player.GetComponent<PlayerBehaviour>().GetHit(abilitiesPowers[attackNr]);
    }

    public void EndHit()
    {
        hasToRotateBackwards = true;
        isAtacking = false;
    }
}
