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
    public CheckHitBox EARange;
    public CheckHitBox EAHitBox;
    private bool damaged;
    public GameObject garrote;

    [Header("Sprites")]
    public Sprite[] enemySprites;
    Vector2 standBySpritePos;
    Vector2 standBySpriteProp;
    public Vector2[] spritesOffsets;//ORDEN: Standby, Anticipation, Attack,
    public Vector2[] spritesProportions;
    public Collider2D[] colliders;


    public override void Awake()
    {
        base.Awake();
        attackTimeline = 0;
        AState = AttackState.ready;
        damaged = false;
        standBySpritePos = spritesOffsets[0];
        standBySpriteProp = spritesProportions[0];
    }

    public override void Update()
    {
        base.Update();
        ManagePose();
        if (!stopEnemy)
        {
            Attack();
        }
    }


    public override void Attack()
    {
        if (AState == AttackState.ready && EARange.CheckFor("Player"))
        {
            attackTimeline = 0;
            AState = AttackState.preparing;
            Debug.Log("stoppu true");
            stoppu = true;
            poseSet = false;
            //garrote.SetActive(true);
            //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y + 3);
        }
        else
        {
            if (attackTimeline > timeToAttack && attackTimeline < timeToAttack + timeDamaging)//quito la condicion  && AState == AttackState.preparing, la pongo abajo vv
            {
                if (!damaged && EAHitBox.CheckFor("Player"))
                {
                    damaged = true;
                    PlayerHP.instance.TakeDamage(eHP.damage);
                }
                if (AState == AttackState.preparing)//separo la condición aqui abajo para poder comprobar si hace daño constantemente y no una sola vez
                {
                    AState = AttackState.damaging;
                    poseSet = false;
                    //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 3);
                }
            }
            else if (attackTimeline > timeDamaging + timeToAttack && AState == AttackState.damaging)
            {
                damaged = false;
                AState = AttackState.recovering;
                poseSet = false;
                //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 1);
            }
            else if (attackTimeline > timeDamaging + timeToAttack + timeRecovering && AState == AttackState.recovering)
            {
                AState = AttackState.ready;
                stoppu = false;
                poseSet = false;
                //garrote.SetActive(false);
                //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y + 1);
            }
        }
        if (AState != AttackState.ready)
        {
            attackTimeline += Time.deltaTime;
        }
    }

    bool poseSet = false;
    void SetPose(int poseIndex)
    {
        sprite.sprite = enemySprites[poseIndex];
        sprite.transform.localPosition = spritesOffsets[poseIndex];
        sprite.transform.localScale = new Vector2(standBySpriteProp.x * spritesProportions[poseIndex].x, standBySpriteProp.y * spritesProportions[poseIndex].y);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (i == poseIndex)
            {
                colliders[i].enabled = true;
            }
            else
            {
                colliders[i].enabled = false;
            }
        }
    }
    void ManagePose()
    {
        if (!poseSet)
        {
            switch (AState)
            {
                case AttackState.ready:
                    SetPose(0);
                    break;
                case AttackState.preparing:
                    SetPose(1);
                    break;
                case AttackState.damaging:
                    SetPose(2);
                    break;
                case AttackState.recovering:
                    SetPose(2);
                    break;
                case AttackState.damaged:
                    break;
                case AttackState.vulnerable:
                    break;
                case AttackState.damagedAfterVulnerable:
                    break;
            }
            poseSet = true;
        }
    }
}
