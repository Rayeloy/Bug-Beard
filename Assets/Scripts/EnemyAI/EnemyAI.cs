using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public bool StartFacingRight;
    public bool stopIfNoPlayerDetected;
    public bool doesPursue;
    [HideInInspector]
    public int playerDetected;
    private GameObject playerGO;
    public float speed;
    public float maxFallSpeed;
    public float gravity;
    public Rigidbody2D myRB;
    public SpriteRenderer sprite;
    public GameObject weakBox;
    public bool weakBoxOrientation;

    private float fallSpeed;
    [HideInInspector]
    public bool stoppu;

    [HideInInspector]
    public enemyState eState;
    [HideInInspector]
    public enemyState currentDirection;
    [HideInInspector]
    public AttackState AState;

    public enum enemyState
    {
        stop,
        wRight,
        wLeft
    }
    public enum AttackState
    {
        ready,
        preparing,
        damaging,
        recovering
    }

    public virtual void Awake()
    {
        fallSpeed = 0;
        playerDetected = 0;
        playerGO = null;
        stoppu = false;
        if (stopIfNoPlayerDetected)
        {
            eState = enemyState.stop;
        }
        else
        {
            if (StartFacingRight)
            {
                sprite.flipX = false;
                eState = enemyState.wRight;
                currentDirection = enemyState.wRight;

            }
            else
            {
                sprite.flipX = true;
                eState = enemyState.wLeft;
                currentDirection = enemyState.wLeft;
            }
        }

    }

    // Update is called once per frame
    public virtual void Update()
    {
        gravityFalls();
        HorizontalMovement();
    }

    void gravityFalls()
    {

        if (myRB.velocity.y > maxFallSpeed)
        {
            fallSpeed = myRB.velocity.y + (gravity * Time.deltaTime);
        }
        else
        {
            fallSpeed = maxFallSpeed;
        }
        fallSpeed = Mathf.Clamp(fallSpeed, maxFallSpeed, 10000);//poner valores grandes que no opriman la parabola
        myRB.velocity = new Vector2(myRB.velocity.x, fallSpeed);
    }

    public virtual void HorizontalMovement()
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
            case enemyState.wRight:
                if (myRB.velocity.x != speed)
                {
                    if (sprite.flipX)
                        sprite.flipX = false;
                    weakBoxOrient();
                    myRB.velocity = new Vector2(speed, myRB.velocity.y);
                }
                break;
            case enemyState.wLeft:
                if (myRB.velocity.x != -speed)
                {
                    if (!sprite.flipX)
                        sprite.flipX = true;
                    weakBoxOrient();
                    myRB.velocity = new Vector2(-speed, myRB.velocity.y);
                }
                break;
            case enemyState.stop:
                myRB.velocity = new Vector2(0, myRB.velocity.y); ;
                break;
        }
    }
    private float olgura = 0.3f;
    public virtual void Pursue()//check if stopIfNoPlayerDetected, if it is detected, where is it and where to go
    {
        if (doesPursue)
        {
            if (playerDetected == 0 && stopIfNoPlayerDetected)
            {
                eState = enemyState.stop;
            }
            else if (playerDetected > 0)
            {
                if (playerGO.transform.position.x > transform.position.x + olgura)
                {
                    eState = enemyState.wRight;
                }
                else if (playerGO.transform.position.x < transform.position.x - olgura)
                {
                    eState = enemyState.wLeft;
                }
            }
        }
    }
    public virtual void StartPursue(GameObject player)
    {
        playerDetected += 1;
        playerGO = player;
    }
    public virtual void StopPursue(GameObject player)
    {
        playerDetected -= 1;
        if (playerGO == player && playerDetected == 0)
        {
            playerGO = null;
        }
    }

    public virtual void Attack()//a implementar por hijos
    {

    }

    public virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "wall" || col.gameObject.tag == "enemy")
        {
            switch (eState)
            {
                case enemyState.wRight:
                    eState = enemyState.wLeft;
                    currentDirection = enemyState.wLeft;
                    break;
                case enemyState.wLeft:
                    eState = enemyState.wRight;
                    currentDirection = enemyState.wRight;
                    break;
            }
        }
    }
    void weakBoxOrient()
    {
        if (weakBoxOrientation)//facing right?
        {
            if (eState == enemyState.wRight)
                weakBox.transform.parent.rotation = new Quaternion(0, 0, 0, 1);
            else if (eState == enemyState.wLeft)
                weakBox.transform.parent.rotation = new Quaternion(0, 180, 0, 1);
        }
        else//facing left?
        {
            if (eState == enemyState.wLeft)
                weakBox.transform.parent.rotation = new Quaternion(0, 0, 0, 1);
            else if (eState == enemyState.wRight)
                weakBox.transform.parent.rotation = new Quaternion(0, 180, 0, 1);
        }
    }
}
