using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_AI : EnemyAI{

    protected Vector2 CurrentDir;
    protected Vector2 FinalDir;
    protected override void gravityFalls()
    {
        base.gravityFalls();
    }
    public override void Update()
    {
        BouncingBack();
        if (!stopEnemy)
        {
            HorizontalMovement();
        }
    }

    public override void HorizontalMovement()
    {
        if (!stoppu)
        {
            if (!stopIfNoPlayerDetected && eState == enemyState.stop)
            {
                eState = currentDirection;//mueve hacia la direccion que iba
            }
            Pursue();//check if stopIfNoPlayerDetected, if it is detected, where is it and where to go
        }
        else
        {
            eState = enemyState.stop;
        }
        switch (eState)
        {
            default:
                weakBoxOrient();
                myRB.velocity = CurrentDir.normalized * speed;
                if (!moved)
                {
                    moved = true;
                    RespawnControler.instance.AddReposEnemy(myRespEnemy);
                }
                break;
            case enemyState.stop:
                myRB.velocity = Vector2.zero;
                break;
        }
    }
    public float smoothRotTime;
    float smoothRotSpeedX;
    float smoothRotSpeedY;
    public override void Pursue()
    {
        if (doesPursue)
        {
            if (playerDetected == 0 && stopIfNoPlayerDetected)
            {
                eState = enemyState.stop;
            }
            else if (playerDetected > 0)
            {
                FinalDir= playerGO.transform.position - transform.position;
                CurrentDir.x = Mathf.SmoothDamp(CurrentDir.x,FinalDir.x,ref smoothRotSpeedX, smoothRotTime);
                CurrentDir.y = Mathf.SmoothDamp(CurrentDir.y, FinalDir.y, ref smoothRotSpeedY, smoothRotTime);
                float dist = Mathf.Abs(CurrentDir.magnitude);
                if (dist < olgura)
                {
                    eState = enemyState.stop;
                }
                else if (CurrentDir.x>=0)
                {
                    eState = enemyState.wRight;
                }
                else if (CurrentDir.x < 0)
                {
                    eState = enemyState.wLeft;
                }
            }
        }
    }
    protected override void weakBoxOrient()
    {
        float dotProduct = Vector2.Dot(CurrentDir.normalized, Vector2.right);
        float magnitudesProduct = CurrentDir.normalized.magnitude * Vector2.right.magnitude;
        float cos = dotProduct / magnitudesProduct;
        float angleDif = Mathf.Acos(cos) * Mathf.Rad2Deg;
        if (CurrentDir.y <= 0)
        {
            angleDif = -angleDif;
        }
        weakBox.transform.GetComponentInParent<EnemyHP>().gameObject.transform.rotation = Quaternion.Euler(0, 0, angleDif);
    }
}

