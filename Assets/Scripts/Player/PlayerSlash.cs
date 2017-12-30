using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlash : MonoBehaviour
{
    public static PlayerSlash instance;
    
    public bool stopOnGround;
    public bool gravityOn;
    public bool stopOnMove;
    public bool abruptEnd;
    public bool jumpOutOfCrystal;

    private Vector3 mousePosition;
    public float InitialSpeed;
    public float cdTime;
    [HideInInspector]
    public float cd;
    public float MaxDistSlash;
    public float slashDamage;
    //public float MaxTimeSlashing;
    public new Camera camera;
    public Rigidbody2D myRB;
    private Vector2 myPos;//my position - Origen del slash
    public PlayerMovement playerM;

    private float slashDist;
    [HideInInspector]
    public float timeSlashing;
    [HideInInspector]
    public Vector2 lastSlashDir;
    [HideInInspector]
    public SlashState slashSt;

    public enum SlashState
    {
        ready,
        slashing,
        cooldown,
        crystal
    }
    [HideInInspector]
    public AttachedCrystal atCrystal;
    public struct AttachedCrystal
    {
        public GameObject Crystal;
        public bool attachReady;
        public AttachedCrystal(GameObject _Crystal, bool _attachReady = false)
        {
            Crystal = _Crystal;
            attachReady = _attachReady;
        }
    }

    private void Awake()
    {
        instance = this;
        atCrystal = new AttachedCrystal(null, true);
        slashSt = SlashState.ready;
        slashDist = 0;
        timeSlashing = 0;
        lastSlashDir = Vector2.zero;
        cd = cdTime;//para la barra de progreso, si no empieza en cdTime, empieza en 0
    }

    private void FixedUpdate()
    {
        switch (slashSt)
        {
            case SlashState.slashing:
                slashDist = new Vector2(transform.position.x - myPos.x, transform.position.y - myPos.y).magnitude;
                if (slashDist >= MaxDistSlash)
                {
                    StopSlash();
                }
                timeSlashing += Time.deltaTime;
                /*if (timeSlashing >= MaxTimeSlashing)
                {
                    StopSlash();
                }*/
                break;
            case SlashState.cooldown:
                cd += Time.deltaTime;
                if (cd >= cdTime)
                {
                    slashSt = SlashState.ready;
                    cd = cdTime;
                }
                break;


        }
    }


    void Update()
    {
        if (!PlayerMovement.instance.stopPlayer)
        {
            mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - camera.transform.position.z));
            if (Input.GetButtonDown("Slash") && (slashSt == SlashState.ready || slashSt == SlashState.crystal))
            {
                //Debug.Log("SLASH!");
                slash();
            }
            if (jumpOutOfCrystal && Input.GetButtonDown("Jump") && slashSt == SlashState.crystal)
            {
                ExitJumpCrystal();
            }
        }
        else
        {
            if (slashSt == SlashState.slashing)//para parar un slash al llegar a un event
            {
                StopSlash();
            }
        }
    }

    void slash()
    {
        if (gameObject.isStatic)
        {
            gameObject.isStatic = false;
        }
        if (gravityOn)
            playerM.phase = PlayerMovement.jumpphase.normal;
        else
            playerM.phase = PlayerMovement.jumpphase.none;
        slashSt = SlashState.slashing;
        timeSlashing = 0;
        myPos = transform.position;
        //Debug.Log("myPos=" + myPos.x + ", " + myPos.y);
        //Vector2 vel = DecomposeSpeed(InitialSpeed, mousePosition, myPos);
        lastSlashDir = new Vector2((mousePosition.x - myPos.x), (mousePosition.y - myPos.y)).normalized;
        myRB.velocity = new Vector2(lastSlashDir.x * InitialSpeed, lastSlashDir.y * InitialSpeed);
        Pointer.instance.attackHBslash();
        //myRB.velocity = new Vector2(80f, 20f);
        //Debug.Log("mouse pos=" + mousePosition.x + ", " + mousePosition.y + "; vx=" + vel.x + ";vy=" + vel.y);
    }
    public void StopSlash()
    {
        //Debug.Log("STOP SLASH");
        if (abruptEnd)
            myRB.velocity = Vector2.zero;
        slashSt = SlashState.cooldown;
        timeSlashing = 0;
        slashDist = 0;
        playerM.phase = PlayerMovement.jumpphase.normal;
        cd = 0;
        Pointer.instance.attackHBnormal();
        PlayerAnimations.instance.ResetSpriteRotation();
        PlayerAnimations.instance.PAnim = PlayerAnimations.PlayerAnims.Jumping;
        PlayerAnimations.instance.SetPose();
#if DEBUG_LOG
        Debug.Log("STOP SLASH");
#endif
    }
    
    public void ResetSlash()
    {
        cd = cdTime;
        timeSlashing = 0;
        slashSt = SlashState.ready;
    }

    public void EnterCrystal(GameObject crystal)
    {
        PlayerMovement.instance.attachToCrystal(crystal);
        atCrystal = new AttachedCrystal(crystal);
    }
    public void ExitCrystal(GameObject crystal)
    {
        if (crystal == instance.atCrystal.Crystal)
        {
            instance.atCrystal.Crystal = null;
            instance.atCrystal.attachReady = true;
        }
    }

    public void ExitJumpCrystal()
    {
        if (gameObject.isStatic)
        {
            gameObject.isStatic = false;
        }
        playerM.phase = PlayerMovement.jumpphase.normal;
        slashSt = PlayerSlash.SlashState.ready;
    }

    private Vector2 DecomposeSpeed(float speed, Vector3 posB, Vector3 posA)
    {
        float angle = Mathf.Atan2((posB.x - posA.x), (posB.y - posA.y)) * Mathf.Rad2Deg;
        return new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
    }
}
