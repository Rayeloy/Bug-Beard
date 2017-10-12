using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public PlayerMovement instance;

    public Transform spriteTransf;
    public Rigidbody2D myRB;

    public bool accVersionOn;
    public bool accOnAir;
    public bool stopJumpOnCollision;
    public float acceleration;
    public float deacceleration;
    public float MaxHorizontalSpeed;
    float finalAcc;


    private float v;
    private string axis = "Horizontal";
    private float HorzSpeed;
    private int deAccDir;

    public float gravity;
    public float jumpForce;
    private float distToGround;
    private bool jumping;
    private float fallSpeed;
    public float maxJumpSpeed;
    public float maxFallSpeed;
    private float jumpingTime;
    public float maxTimeJumping;

    public Transform groundcheck1;
    public Transform groundcheck2;
    //float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool IsGrounded;
    





    // Use this for initialization
    void Awake()
    {
        instance = this;
        HorzSpeed = 0;
        deAccDir = 0;
        jumping = false;
    }

    // Update is called once per frame
    void Update()
    {

        HorizontalMovement();
        //Debug.Log("GRAVEDAD= " + IsGrounded + ", " + jumping);
        if (!IsGrounded && !jumping)//------------------- LA GRAVEDAD NO PERDONA ---------------------------------
        {

            if (myRB.velocity.y > maxFallSpeed)
            {
                fallSpeed = Mathf.Clamp(myRB.velocity.y + (gravity * Time.deltaTime * -1), maxFallSpeed, maxJumpSpeed);
            }
            myRB.velocity = new Vector2(myRB.velocity.x, fallSpeed);
        }
        else
        {
            myRB.velocity = new Vector2(myRB.velocity.x, 0);
        }
        if (Input.GetButtonDown("Jump") && IsGrounded)//Nos abre las puertas para saltar
        {
            jumping = true;
            jumpingTime = 0;
            Debug.Log("comenzamos salto; jumping= " + jumping);
        }
        if (Input.GetButton("Jump") && jumping)//saltando
        {
            //contador de tiempo limite de manterner jump       
            myRB.velocity = new Vector2(myRB.velocity.x, Mathf.Clamp(jumpForce * 1, maxFallSpeed, maxJumpSpeed));
            jumpingTime += Time.deltaTime;
            if (jumpingTime >= maxTimeJumping)
                jumping = false;
            Debug.Log("saltando...; jumping= " + jumping);
        }
        if (Input.GetButtonUp("Jump") && jumping)//no podemos seguir aumentando el salto al soltar el boton
        {
            jumping = false;
            Debug.Log("salto terminado; jumping= " + jumping);
        }

    }
    void HorizontalMovement()
    {
        v = Input.GetAxisRaw(axis);
        //Orientación del transform(sprite)
        if (myRB.velocity.x > 0) spriteTransf.rotation = new Quaternion(0, 180, 0, 1);
        else
        {
            if (myRB.velocity.x < 0) spriteTransf.rotation = Quaternion.identity;
        }

        if (accVersionOn || (accOnAir&&!IsGrounded))
        {
            HorzSpeed = myRB.velocity.x;
            if (v != 0)//si hay input de movimiento
            {

                Debug.Log("me muevo");
                if (Mathf.Abs(HorzSpeed) < MaxHorizontalSpeed)
                {
                    if (Mathf.Sign(HorzSpeed) != Mathf.Sign(v))
                    {
                        finalAcc = deacceleration + acceleration / 2;
                    }
                    else
                    {
                        finalAcc = acceleration;
                    }

                    HorzSpeed = Mathf.Clamp(HorzSpeed + finalAcc * v * Time.deltaTime, -MaxHorizontalSpeed, MaxHorizontalSpeed);

                }/*else if(HorzSpeed >= MaxHorizontalSpeed)
            {
                HorzSpeed=MaxHorizontalSpeed;
            }*/
            }
            else//no hay input
            {
                Debug.Log("no me muevo");
                if (HorzSpeed > 0)
                {
                    HorzSpeed -= deacceleration * Time.deltaTime;
                    deAccDir = 1;
                }
                else if (HorzSpeed < 0)
                {
                    HorzSpeed += deacceleration * Time.deltaTime;
                    deAccDir = -1;
                }
                else
                {
                    deAccDir = 0;
                }

                switch (deAccDir)
                {
                    case 0:
                        break;
                    case 1:
                        HorzSpeed = Mathf.Clamp(HorzSpeed, 0, MaxHorizontalSpeed);
                        break;
                    case -1:
                        HorzSpeed = Mathf.Clamp(HorzSpeed, -MaxHorizontalSpeed, 0);
                        break;

                }
            }

            //finalmente actualizamos la velocidad
            myRB.velocity = new Vector2(1 * HorzSpeed, myRB.velocity.y);
        }
        else
        {
                myRB.velocity = new Vector2(1 * MaxHorizontalSpeed * v, myRB.velocity.y);
        }

    }
    /*private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "ground" && col.gameObject.transform.position.y <= groundcheck1.position.y)
        {
            IsGrounded = true;
        }
    }*/
    private void OnCollisionEnter2D(Collision2D col)
    {

        if(stopJumpOnCollision && jumping && (groundcheck2.position.y<=col.gameObject.transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().bounds.extents.y))
        {
            jumping = false;
        }
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "ground")
        {
            IsGrounded = false;
            //Debug.Log("Dist a ground= " + distToGround);
        }
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        //distToGround = col.collider.bounds.extents.y;
        float top = (col.gameObject.transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().bounds.extents.y);
        float feet = groundcheck1.position.y;//(transform.position.y - GetComponent<SpriteRenderer>().bounds.extents.y);
        if (col.gameObject.tag == "ground" && top<=feet)
        {
            IsGrounded = true;    
            //Debug.Log("Dist a ground= " + distToGround);
        }
        //Debug.Log("Grounded = " + IsGrounded+"; top= "+top+" y feet = "+feet);
    }
    /*bool IsGrounded()
    {
        Debug.Log("Im grounded= " + Physics2D.OverlapArea(groundcheck1.position, groundcheck2.position, whatIsGround));
        return Physics2D.OverlapArea(groundcheck1.position, groundcheck2.position, whatIsGround);
    }*/
}
