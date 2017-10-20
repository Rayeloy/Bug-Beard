﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAI : EnemyAI
{

    public float timeToAttack;
    public float timeDamaging;
    public float timeRecovering;
    private float attackTimeline;
    public EnemyHP eHP;
    public EnemyAttackRange EARange;
    private bool damaged;
    public GameObject garrote;


    public override void Awake()
    {
        base.Awake();
        attackTimeline = 0;
        AState = AttackState.ready;
        damaged = false;
    }

    public override void Update()
    {
        base.Update();
        Attack();
    }


    public override void Attack()
    {
        if (AState == AttackState.ready && EARange.thereIsPlayer)
        {
            attackTimeline = 0;
            AState = AttackState.preparing;
            stoppu = true;
            garrote.SetActive(true);
            garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y + 3);
        }
        else
        {
            if (attackTimeline > timeToAttack && AState == AttackState.preparing)
            {
                if (!damaged && EARange.thereIsPlayer)
                {
                    damaged = true;
                    PlayerHP.instance.TakeDamage(eHP.damage);
                }
                AState = AttackState.damaging;
                garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 3);
            }
            else if (attackTimeline > timeDamaging + timeToAttack && AState == AttackState.damaging)
            {
                damaged = false;
                AState = AttackState.recovering;
                garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 1);
            }
            else if (attackTimeline > timeDamaging + timeToAttack + timeRecovering && AState == AttackState.recovering)
            {
                AState = AttackState.ready;
                stoppu = false;
                garrote.SetActive(false);
                garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y + 1);
            }
        }
        if (AState != AttackState.ready)
        {
            attackTimeline += Time.deltaTime;
        }
    }
}
