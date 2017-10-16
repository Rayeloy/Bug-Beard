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
    public bool jumpWithAcc;

    public float acceleration;
    public float deacceleration;
    public float MaxHorizontalSpeed;
    float finalAcc;


    private float v;
    private string axis = "Horizontal";
    private float HorzSpeed;
    private int deAccDir;

    public float gravityUp;
    public float gravityDown;
    private float gravityAct;
    public float jumpForce;
    private float distToGround;
    private bool jumping;
    private float fallSpeed;
    public float maxJumpSpeed;
    public float maxFallSpeed;
    private float jumpingTime;
    public float maxTimeJumping;

    public bool jumpWithHeight;
    public bool shortHops;
    public float maxHeight;//hay que darlo
    public float timeToReach;//hay que darlo
    private float gravity;//se calcula solo
    public jumpphase phase;
    public float gtimesStop;
    private float initialHeight;
    private bool newOcurrence;


    public Transform groundcheck1;
    public Transform groundcheck2;
    //float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool IsGrounded;

    public PlayerSlash pSlash;

    public enum jumpphase
    {
        none,
        normal,
        rise,
        stop,
        fall
    };



    // Use this for initialization
    void Awake()
    {
        instance = this;
        HorzSpeed = 0;
        deAccDir = 0;
        jumping = false;
        phase = jumpphase.normal;
        gravity = (-2 * maxHeight) / Mathf.Pow(timeToReach, 2); Debug.Log("la gravedad es de " + gravity);
        initialHeight = 0;
        newOcurrence = true;
    }
    private void Start()
    {
        if (jumpWithHeight) jumpWithAcc = false;
        if (accVersionOn) accOnAir = false;
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        gravityFalls();
        if (!jumpWithHeight)
        {
            Jump();
        }
        else
        {
            JumpWithHeight();
        }

    }
    void HorizontalMovement()
    {
        if (pSlash.slashSt != PlayerSlash.SlashState.crystal)
        {
            Debug.Log("SLASHING STATE= " + pSlash.slashSt);
            v = Input.GetAxisRaw(axis);
            //Orientación del transform(sprite)
            if (myRB.velocity.x > 0 && v > 0) spriteTransf.rotation = new Quaternion(0, 180, 0, 1);
            else
            {
                if (myRB.velocity.x < 0 && v < 0) spriteTransf.rotation = Quaternion.identity;
            }

            if (accVersionOn || (accOnAir && !IsGrounded))
            {
                HorzSpeed = myRB.velocity.x;
                if (v != 0)//si hay input de movimiento
                {

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
                else if (myRB.velocity.x != 0)//no hay input
                {
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
                //this is to let the slash mechanic move the character
                if ((v != 0 && pSlash.stopOnMove && pSlash.slashSt == PlayerSlash.SlashState.slashing) ||
                    (pSlash.stopOnGround && pSlash.slashSt == PlayerSlash.SlashState.slashing && IsGrounded && pSlash.timeSlashing > 0.1))//if we detect some movement
                {
                    pSlash.StopSlash();
                }
                if (pSlash.slashSt != PlayerSlash.SlashState.slashing)
                    myRB.velocity = new Vector2(1 * MaxHorizontalSpeed * v, myRB.velocity.y);
            }
        }
    }
    void gravityFalls()
    {
        Debug.Log("EN FASE " + phase.ToString());
        if (!IsGrounded)//------------------- LA GRAVEDAD NO PERDONA ---------------------------------
        {
            if (newOcurrence)
            {
                newOcurrence = false;
            }
            if (jumpWithHeight)
            {
                switch (phase)
                {
                    case jumpphase.none:
                        gravityAct = 0;
                        break;
                    case jumpphase.normal:
                        gravityAct = gravity;
                        break;
                    case jumpphase.rise:
                        gravityAct = gravity;
                        break;
                    case jumpphase.stop:
                        gravityAct = gravity * gtimesStop;
                        // if((transform.position.y - initialHeight < maxHeight/4))
                        break;
                    case jumpphase.fall:
                        gravityAct = gravity * gtimesStop / 2;
                        break;
                }
            }
            else
            {
                if (myRB.velocity.y >= 0) gravityAct = gravityUp;
                else gravityAct = gravityDown;
            }

            //CONTROLAMOS QUE NO SUPERE EL MAX DE CAIDA
            if (myRB.velocity.y > maxFallSpeed)
            {
                fallSpeed = myRB.velocity.y + (gravityAct * Time.deltaTime);
            }
            else
            {
                fallSpeed = maxFallSpeed;
            }
            if (shortHops && phase == jumpphase.stop && (transform.position.y - initialHeight < maxHeight / 2))//CASO ESPECIAL PARA DAR SHORT HOPS
            {
                Debug.Log("CASO ESPECIAL");
                fallSpeed = 0;
                phase = jumpphase.fall;
            }
            fallSpeed = Mathf.Clamp(fallSpeed, maxFallSpeed, maxJumpSpeed);//poner valores grandes que no opriman la parabola
            myRB.velocity = new Vector2(myRB.velocity.x, fallSpeed);
        }
    }
    void Jump()
    {
        Debug.Log("isGrounded= " + IsGrounded);
        if (Input.GetButtonDown("Jump") && IsGrounded)//Nos abre las puertas para saltar
        {
            jumping = true;
            jumpingTime = 0;
            Debug.Log("comenzamos salto; jumping= " + jumping);
        }
        if (Input.GetButton("Jump") && jumping)//saltando
        {
            //contador de tiempo limite de manterner jump  
            Vector2 jumpingSpeed;
            if (jumpWithAcc)
            {
                jumpingSpeed = new Vector2(myRB.velocity.x, Mathf.Clamp(jumpForce * 1 * Time.deltaTime, maxFallSpeed, maxJumpSpeed));
            }
            else
            {
                jumpingSpeed = new Vector2(myRB.velocity.x, maxJumpSpeed);
            }
            myRB.velocity = jumpingSpeed;
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
    void JumpWithHeight()
    {

        if (Input.GetButtonDown("Jump") && IsGrounded)//Nos abre las puertas para saltar
        {
            phase = jumpphase.rise;
            jumpingTime = 0;
            Debug.Log("comenzamos salto; jumping= " + phase.ToString());
            initialHeight = transform.position.y;
            myRB.velocity = new Vector2(myRB.velocity.x, 2 * maxHeight / timeToReach);
        }
        if (Input.GetButton("Jump") && phase == jumpphase.rise)//saltando
        {
            jumpingTime += Time.deltaTime;
            if (jumpingTime >= timeToReach)
                phase = jumpphase.fall;
            Debug.Log("saltando...; jumping= " + phase.ToString());
        }
        if (Input.GetButtonUp("Jump") && phase == jumpphase.rise)//no podemos seguir aumentando el salto al soltar el boton
        {
            phase = jumpphase.stop;
            Debug.Log("salto terminado; jumping= " + phase.ToString());
        }
        if (phase == jumpphase.stop && (myRB.velocity.y <= 0))
        {
            phase = jumpphase.fall;
        }
        if (phase == jumpphase.fall && IsGrounded)
        {
            phase = jumpphase.normal;
            newOcurrence = true;
        }

    }
    /*private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "ground" && col.gameObject.transform.position.y <= groundcheck1.position.y)
        {
            IsGrounded = true;
        }
    }*/
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "crystal" && pSlash.slashSt == PlayerSlash.SlashState.slashing)
        {
            phase = jumpphase.none;
            pSlash.slashSt = PlayerSlash.SlashState.crystal;
            myRB.velocity = Vector2.zero;
            transform.position = col.gameObject.transform.GetChild(0).transform.position;
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (stopJumpOnCollision && (jumping || phase == jumpphase.rise || phase == jumpphase.stop) && (groundcheck2.position.y <= col.gameObject.transform.position.y + col.gameObject.GetComponent<BoxCollider2D>().bounds.extents.y))
        {
            jumping = false;
            phase = jumpphase.fall;
        }
        if (pSlash.slashSt == PlayerSlash.SlashState.slashing)
        {
            pSlash.StopSlash();
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
        if (col.gameObject.tag == "ground" && top <= feet)
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
