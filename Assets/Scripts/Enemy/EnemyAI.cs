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
    protected bool stoppu;
    public bool stopEnemy;

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
        ready=0,
        preparing=1,
        damaging=2,
        recovering=3
    }

    public virtual void Awake()
    {
        fallSpeed = 0;
        playerDetected = 0;
        playerGO = null;
        if (stopIfNoPlayerDetected)
        {
            eState = enemyState.stop;
        }
        else
        {
            if (StartFacingRight)
            {
                eState = enemyState.wRight;
                currentDirection = enemyState.wRight;

            }
            else
            {
                eState = enemyState.wLeft;
                currentDirection = enemyState.wLeft;
            }
        }

    }

    // Update is called once per frame
    public virtual void Update()
    {
        gravityFalls();
        if (!stopEnemy)
        {
            CheckFall();
            CheckCollisionFoward();
            HorizontalMovement();
        }
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
                    weakBoxOrient();
                    myRB.velocity = new Vector2(speed, myRB.velocity.y);
                }
                break;
            case enemyState.wLeft:
                if (myRB.velocity.x != -speed)
                {
                    weakBoxOrient();
                    myRB.velocity = new Vector2(-speed, myRB.velocity.y);
                }
                break;
            case enemyState.stop:
                myRB.velocity = new Vector2(0, myRB.velocity.y); ;
                break;
        }
    }
    public float olgura;
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
                if (playerGO.transform.position.x < transform.position.x + olgura && playerGO.transform.position.x > transform.position.x - olgura)
                {
                    eState = enemyState.stop;
                }
                else if (playerGO.transform.position.x > transform.position.x)
                {
                    eState = enemyState.wRight;
                }
                else if (playerGO.transform.position.x < transform.position.x)
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
    Vector3 enemyCentre;
    public Collider2D enemyCol;
    public virtual void CheckFall()
    {
        RaycastHit2D hit;
        enemyCentre = enemyCol.bounds.center;
        Vector2 rayDir = Vector2.down;
        if (eState == enemyState.wLeft)
        {
            rayDir = new Vector2(-0.5f, -1);
        }
        else if(eState == enemyState.wRight)
        {
            rayDir = new Vector2(0.5f, -1);
        }
        float dist = enemyCol.bounds.extents.y + enemyCol.bounds.extents.y / 3;
        int layerMask = LayerMask.GetMask("Scenary");
        hit = Physics2D.Raycast(enemyCentre,rayDir, dist,layerMask);
        Debug.DrawRay(enemyCentre, rayDir * dist, Color.green);
        if (!hit)
        {
            if (eState == enemyState.wLeft)
            {
                eState = enemyState.wRight;
            }
            else if (eState == enemyState.wRight)
            {
                eState = enemyState.wLeft;
            }
        }
    }

    public virtual void CheckCollisionFoward()//mejorar con que los raycasts tengan una distancia acorde al ancho en cada altura del collider
    {
        RaycastHit2D[] hit;
        enemyCentre = enemyCol.bounds.center;
        Vector2 rayDir = Vector2.down;
        if (eState == enemyState.wLeft)
        {
            rayDir = Vector2.left;
            enemyCentre = new Vector2(enemyCentre.x - enemyCol.bounds.extents.x - 0.01f,enemyCentre.y);
        }
        else if (eState == enemyState.wRight)
        {
            rayDir = Vector2.right;
            enemyCentre = new Vector2(enemyCentre.x + enemyCol.bounds.extents.x + 0.01f, enemyCentre.y);
        }
        float dist = 0.19f;
        int layerMask = LayerMask.GetMask("Scenary","Enemy");
        for(float i=enemyCentre.y-enemyCol.bounds.extents.y/1.2f;i<= enemyCentre.y + enemyCol.bounds.extents.y; i += 0.5f)
        {
            Vector2 newCentre = new Vector2(enemyCentre.x, i);
            hit = Physics2D.RaycastAll(newCentre, rayDir, dist, layerMask);
            Debug.DrawRay(newCentre, rayDir * dist, Color.green);
            //Debug.Log(gameObject + " COLLISION WITH " + hit.collider.gameObject);
            bool hitBool = false;
            for(int k = 0; k < hit.Length; k++)
            {
                if(hit[k].collider.gameObject.tag=="wall" || hit[k].collider.gameObject.tag == "ground" || hit[k].collider.gameObject.tag == "platform")
                {
                    hitBool = true;
                }
            }
            if (hitBool)
            {
                if (eState == enemyState.wLeft)
                {
                    eState = enemyState.wRight;
                }
                else if (eState == enemyState.wRight)
                {
                    eState = enemyState.wLeft;
                }
                break;
            }
        }
    }
    /*public virtual void OnCollisionEnter2D(Collision2D col)
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
    }*/
    void weakBoxOrient()
    {
        if (weakBoxOrientation)//facing right?
        {
            if (eState == enemyState.wRight)
                weakBox.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            else if (eState == enemyState.wLeft)
                weakBox.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
        }
        else//facing left?
        {
            if (eState == enemyState.wLeft)
                weakBox.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            else if (eState == enemyState.wRight)
                weakBox.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
