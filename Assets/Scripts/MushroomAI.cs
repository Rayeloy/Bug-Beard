using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomAI : MonoBehaviour {

    public bool StartFacingRight;
    public float speed;
    public float maxFallSpeed;
    public float gravity;
    public Rigidbody2D myRB;
    public SpriteRenderer sprite;

    private float fallSpeed;

    [HideInInspector]
    public mushroomState mState;

    public enum mushroomState
    {
        wRight,
        wLeft
    }

    private void Awake()
    {
        fallSpeed = 0;

        if (StartFacingRight)
        {
            sprite.flipX = false;
            mState = mushroomState.wRight;

        }
        else
        {
            sprite.flipX = true;
            mState = mushroomState.wLeft;
        }

    }

    // Update is called once per frame
    void Update () {
        gravityFalls();
        HorizontalMovement();

    }

    void gravityFalls()
    {

        if (myRB.velocity.y > maxFallSpeed)
        {
            fallSpeed = myRB.velocity.y + (gravity * Time.deltaTime);
            Debug.Log("++++++++++++++++++++++++++++++++++++++++GRAVEDAD A: fallSpeed=" + fallSpeed);
        }
        else
        {
            fallSpeed = maxFallSpeed;
            Debug.Log("++++++++++++++++++++++++++++++++++++++++GRAVEDAD B: fallSpeed=" + fallSpeed);
        }
        fallSpeed = Mathf.Clamp(fallSpeed, maxFallSpeed, 10000);//poner valores grandes que no opriman la parabola
        Debug.Log("++++++++++++++++++++++++++++++++++++++++GRAVEDAD C: fallSpeed=" + fallSpeed);
        myRB.velocity = new Vector2(myRB.velocity.x, fallSpeed);
    }

    void HorizontalMovement()
    {
        switch (mState)
        {
            case mushroomState.wRight:
                if(myRB.velocity.x != speed)
                {
                    if (sprite.flipX)
                        sprite.flipX = false;
                    myRB.velocity = new Vector2(speed, myRB.velocity.y);
                }
                break;
            case mushroomState.wLeft:
                if(myRB.velocity.x != -speed)
                {
                    if(!sprite.flipX)
                    sprite.flipX = true;
                    myRB.velocity = new Vector2(-speed, myRB.velocity.y);
                }
                break;
        }
    }
}
