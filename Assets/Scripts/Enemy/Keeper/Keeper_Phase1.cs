using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_Phase1 : EnemyAI
{

    public float timeToAttack;
    public float timeDamaging;
    public float timeRecovering;
    private float attackTimeline;

    public float maxTimeVulnerable;

    public EnemyHP eHP;
    public CheckHitBox EARange;
    public CheckHitBox EAHitBox;
    private bool damaged;
    public GameObject garrote;
    public GameObject espada;
    [HideInInspector]
    public bool vulnareble = false;



    [HideInInspector]
    public KeeperP1 KP1 = KeeperP1.espazado;
    public enum KeeperP1
    {
        espazado=0,
        excalibur=1

    }


    public override void Awake()
    {
        base.Awake();
        attackTimeline = 0;
        AState = AttackState.ready;
        damaged = false;
    }

    public override void Update()
    {
        switch(KP1){
            case KeeperP1.espazado:
                base.Update();
                if (!stopEnemy)
                {
                    if (AState!=AttackState.vulnerable)
                    {
                        Attack();
                    }
                    else
                    {
                        attackTimeline += Time.deltaTime;
                        if (attackTimeline > maxTimeVulnerable)
                        {
                            AState = AttackState.ready;
                        }
                    }

                }
                break;
            case KeeperP1.excalibur:
                doExcalibur();
                break;
        }

    }

    void doExcalibur()
    {

    }


    public override void Attack()
    {
        if (AState == AttackState.ready && EARange.CheckFor("Player"))
        {
            attackTimeline = 0;
            AState = AttackState.preparing;
            stoppu = true;
            garrote.SetActive(true);
            garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y + 3);
        }
        else
        {
            //PREPARING ATTACK
            if (attackTimeline > timeToAttack && attackTimeline < timeToAttack + timeDamaging)//quito la condicion  && AState == AttackState.preparing, la pongo abajo vv
            {
                weakBox.SetActive(true);
                vulnareble = true;

                if (!damaged && EAHitBox.CheckFor("Player"))
                {
                    damaged = true;
                    PlayerHP.instance.TakeDamage(eHP.damage);
                }
                if (AState == AttackState.preparing)//separo la condición aqui abajo para poder comprobar si hace daño constantemente y no una sola vez
                {
                    AState = AttackState.damaging;
                    garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 3);
                }
            }
            //ATTACKING
            else if (attackTimeline > timeDamaging + timeToAttack && AState == AttackState.damaging)
            {
                vulnareble = false;
                weakBox.SetActive(false);

                damaged = false;
                AState = AttackState.recovering;
                garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 1);
            }
            //RECOVERING
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

    public void BecomeVulnarable()
    {
        weakBox.SetActive(false);
        vulnareble = false;
        AState = AttackState.vulnerable;
        //animación a vulnerable
        //espada vulnerable 
        espada.SetActive(true);
        attackTimeline = 0;
    }
}
