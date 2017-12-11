using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    static public PlayerMovement instance;

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

    public LayerMask whatIsGround;
    private bool IsGrounded;

    public float bounceForce;
    [HideInInspector]
    public bool bouncing;
    private float bounceTime;
    public float MaxBounceTime;

    public enum jumpphase
    {
        none,//no gravedad
        normal,//gravedad normal
        rise,//gravedad muy aumentada
        stop,//parar en shorthops
        fall//gravedad aumentada
    };
    public bool startFacingRight;
    public pmoveState pmState;
    public enum pmoveState
    {

        stopRight,
        stopLeft,
        wRight,
        wLeft
    }

    public bool stopPlayer;
    bool playerStopped;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        playerCentre = transform.position;
        HorzSpeed = 0;
        deAccDir = 0;
        jumping = false;
        phase = jumpphase.normal;
        gravity = (-2 * maxHeight) / Mathf.Pow(timeToReach, 2); Debug.Log("la gravedad es de " + gravity);
        initialHeight = 0;
        bouncing = false;
        bounceTime = 0;
        stopPlayer = false;
        playerStopped = false;
        if (startFacingRight)
        {
            pmState = pmoveState.stopRight;
        }
        else
        {
            pmState = pmoveState.stopLeft;
        }

    }
    private void Start()
    {
        if (jumpWithHeight) jumpWithAcc = false;
        if (accVersionOn) accOnAir = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopPlayer)
        {
            if (playerStopped) playerStopped = false;
            HorizontalMovement();
            if (!jumpWithHeight)
            {
                Jump();
            }
            else
            {
                JumpWithHeight();
            }
            LookDownUp();
        }
        else
        {
            if (!playerStopped)
            {
                playerStopped = true;
                myRB.velocity = Vector3.zero;
                phase = jumpphase.fall;
            }
        }

        CheckGrounded();
        CheckCollisionHead();
        gravityFalls();
        if (bouncing)//tiempo rebotando (perdemos el control del jugador)
        {
            bounceTime += Time.deltaTime;
            if (bounceTime > MaxBounceTime)
            {
                bouncing = false;
            }
        }
    }
    void HorizontalMovement()
    {
#if DEBUG_LOG
        Debug.Log("SLASHING STATE= " + PlayerSlash.instance.slashSt);
#endif
        if (PlayerSlash.instance.slashSt != PlayerSlash.SlashState.crystal && !bouncing)
        {
            v = Input.GetAxisRaw(axis);
            //Orientación del transform(sprite)
            if (myRB.velocity.x > 0 && v > 0)
                spriteTransf.rotation = new Quaternion(0, 180, 0, 1);
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

                    }
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
                //this is to let player stop de slash with movement or when hitting ground
                if ((v != 0 && PlayerSlash.instance.stopOnMove && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing) ||
                    (PlayerSlash.instance.stopOnGround && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing && IsGrounded && PlayerSlash.instance.timeSlashing > 0.1))//if we detect some movement
                {
                    //Debug.Log("ISGROUNDED= " + IsGrounded);
                    PlayerSlash.instance.StopSlash();
                }
                //--------------MOVER JUGADOR SEGUN CONTROLES---------------
                if (PlayerSlash.instance.slashSt != PlayerSlash.SlashState.slashing)
                {
                    myRB.velocity = new Vector2(1 * MaxHorizontalSpeed * v, myRB.velocity.y);
                    orientPlayer();
                }
            }
        }
    }
    void gravityFalls()
    {
#if DEBUG_LOG
        Debug.Log("EN FASE " + phase.ToString());
#endif
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
        if (IsGrounded)
        {
            gravityAct = gravity;
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
#if DEBUG_LOG
                Debug.Log("CASO ESPECIAL SHORT HOPS");
#endif
            fallSpeed = 0;
            phase = jumpphase.fall;
        }
        fallSpeed = Mathf.Clamp(fallSpeed, maxFallSpeed, maxJumpSpeed);//poner valores grandes que no opriman la parabola
        myRB.velocity = new Vector2(myRB.velocity.x, fallSpeed);
    }
    void Jump()//Mecanica de salto antigua
    {
#if DEBUG_LOG
        Debug.Log("isGrounded= " + IsGrounded);
#endif
        if (Input.GetButtonDown("Jump") && IsGrounded)//Nos abre las puertas para saltar
        {
            jumping = true;
            jumpingTime = 0;
#if DEBUG_LOG
            Debug.Log("comenzamos salto; jumping= " + jumping);
#endif
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
#if DEBUG_LOG
            Debug.Log("saltando...; jumping= " + jumping);
#endif
        }
        if (Input.GetButtonUp("Jump") && jumping)//no podemos seguir aumentando el salto al soltar el boton
        {
            jumping = false;
#if DEBUG_LOG
            Debug.Log("salto terminado; jumping= " + jumping);
#endif
        }
    }
    void JumpWithHeight()//Mecanica de salto chachi piruli
    {

        if (Input.GetButtonDown("Jump") && IsGrounded)//Nos abre las puertas para saltar
        {
            phase = jumpphase.rise;
            jumpingTime = 0;
            initialHeight = transform.position.y;
            myRB.velocity = new Vector2(myRB.velocity.x, 2 * maxHeight / timeToReach);
        }
        if (Input.GetButton("Jump") && phase == jumpphase.rise)//saltando
        {
            jumpingTime += Time.deltaTime;
            if (jumpingTime >= timeToReach)
                phase = jumpphase.fall;
        }
        if (Input.GetButtonUp("Jump") && phase == jumpphase.rise)//no podemos seguir aumentando el salto al soltar el boton
        {
            phase = jumpphase.stop;
        }
        if (phase == jumpphase.stop && (myRB.velocity.y <= 0))
        {
            phase = jumpphase.fall;
        }
        if (phase == jumpphase.fall && IsGrounded)
        {
            phase = jumpphase.normal;
            //newOcurrence = true;
        }

    }
    public void BounceBack()
    {
        //Debug.Log("BOUNCE BACK!! dir= " + (PlayerSlash.instance.lastSlashDir * -1));
        bouncing = true;
        bounceTime = 0;
        myRB.velocity = PlayerSlash.instance.lastSlashDir * -1 * bounceForce;
        instance.phase = jumpphase.fall;
    }

    private void orientPlayer()
    {
        if (v > 0)
        {
            pmState = pmoveState.wRight;
        }
        else if (v < 0)
        {
            pmState = pmoveState.wLeft;
        }
        else if (pmState == pmoveState.wRight)
        {
            pmState = pmoveState.stopRight;
        }
        else if (pmState == pmoveState.wLeft)
        {
            pmState = pmoveState.stopLeft;
        }
    }

    public void attachToCrystal(GameObject crystal)
    {
        phase = jumpphase.none;
        PlayerSlash.instance.slashSt = PlayerSlash.SlashState.crystal;
        myRB.velocity = Vector2.zero;
        GameObject crystalPos = GameController.instance.GetChild(crystal, "playerPos");
        transform.position = crystalPos.transform.position;
        Pointer.instance.attackHBnormal();
    }

    private void CheckCollisionHead()
    {
        Vector3 playerCentre = playerCollider.bounds.center;
        float maxDist = (playerCollider.bounds.extents.y + 0.05f);
        int layerMask = LayerMask.GetMask("Scenary");
        hit = Physics2D.Raycast(playerCollider.bounds.center, Vector2.up, maxDist, layerMask);
        Vector3 up = Vector3.up * maxDist;
        Debug.DrawRay(playerCollider.bounds.center, up, Color.red);
        if (hit)
        {
            //Debug.Log("Hit.Collider.gameObject.tag= " + hit.collider.gameObject.tag);
            if (hit.collider.gameObject.tag == "ground" || hit.collider.gameObject.tag == "platform" || hit.collider.gameObject.tag == "wall")
            {
                if (stopJumpOnCollision && (jumping || phase == jumpphase.rise || phase == jumpphase.stop))
                {
                    jumping = false;
                    phase = jumpphase.fall;
                }
                if (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
                {
                    PlayerSlash.instance.StopSlash();
                }
            }
        }
    }
    RaycastHit2D hit;
    float distToHit;
    public Collider2D playerCollider;
    Vector3 playerCentre;
    private void CheckGrounded()
    {
        Vector3 playerCentre = playerCollider.bounds.center;
        float maxDist = (playerCollider.bounds.extents.y) + 0.75f;
        int layerMask = LayerMask.GetMask("Scenary");
        Vector3 down = Vector3.down;
        bool hasHit = false;
        for (float i = playerCentre.x - playerCollider.bounds.extents.x / 1.2f; i <= playerCentre.x + playerCollider.bounds.extents.x; i += 0.45f)
        {
            Vector2 newCentre = new Vector2(i, playerCentre.y);
            hit = Physics2D.Raycast(newCentre, down, maxDist, layerMask);
            Debug.DrawRay(newCentre, down * maxDist, Color.green);
            if (hit) hasHit = true;

        }
        if (hasHit)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "wall" || (col.gameObject.tag == "ground" && isAtRight(playerCollider, col)))
        {
            if (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
            {
                PlayerSlash.instance.StopSlash();
            }
        }
    }
    [Tooltip("Time pressing w/up or s/down needed to look up of down (when not moving)")]
    public float maxTimePressing2Look;
    bool pressingUp = false, pressingDown=false;
    float timePressingUp = 0, timePressingDown = 0;
    void LookDownUp()
    {
        float h = Input.GetAxisRaw(axis);
        float v = Input.GetAxisRaw("Vertical");
        Vector2 velOlgura = new Vector2(0.1f, 0.1f);
        //Debug.Log("VELOCITY=" + myRB.velocity);
        if (h == 0 && ((myRB.velocity.x < velOlgura.x && myRB.velocity.y < velOlgura.y) && (myRB.velocity.x > -velOlgura.x && myRB.velocity.y > -4f)))
        {
            if (v>0)
            {
                //Debug.Log("PRESSING UP");
                    pressingUp = true;
                    timePressingUp += Time.deltaTime;
                    if (timePressingUp >= maxTimePressing2Look)
                    {
                   // Debug.Log("LOOKING UP");
                    CameraMovement.instance.LookUp = true;
                    }
            }else if (v<0)
            {
               // Debug.Log("PRESSING DOWN");
                pressingDown = true;
                    timePressingDown += Time.deltaTime;
                    if (timePressingDown >= maxTimePressing2Look)
                    {
                   // Debug.Log("LOOKING DOWN");
                    CameraMovement.instance.LookDown = true;
                }
            }
            else
            {
                //Debug.Log("STOP LOOK UP/DOWN");
                timePressingUp = 0;
                timePressingDown = 0;
                pressingUp = pressingDown = false;
                CameraMovement.instance.LookUp = CameraMovement.instance.LookDown=false;
            }
        }
        else if(pressingUp || pressingDown)
        {
            //Debug.Log("STOP LOOK UP/DOWN");
            timePressingUp = 0;
            timePressingDown = 0;
            pressingUp = pressingDown = false;
            CameraMovement.instance.LookUp = CameraMovement.instance.LookDown = false;
        }

    }

    bool isAtRight(Collider2D player, Collider2D Obstacle)
    {
        bool res = false;
        float limitXdcha = Obstacle.bounds.center.x + Obstacle.bounds.extents.x;
        float limitXizda = Obstacle.bounds.center.x - Obstacle.bounds.extents.x;
        float limitYtop= Obstacle.bounds.center.y + Obstacle.bounds.extents.y;
        if ((player.bounds.center.x > limitXdcha || player.bounds.center.x < limitXizda) && player.bounds.center.y<limitYtop)
        {
            res = true;
        }
        return res;
    }
}
