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
    public bool jumpUpCrystal;
    bool jumpDownCrystal;

    public GameObject slashSplash;
    public Transform slashSplashes;
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
        crystal,
        sliding
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
        jumpUpCrystal = false;
        jumpDownCrystal = false;
    }

    private void FixedUpdate()
    {
        switch (slashSt)
        {
            case SlashState.slashing:
                slashDist = new Vector2(transform.position.x - myPos.x, transform.position.y - myPos.y).magnitude;
                lastSlashDir = new Vector2((mousePosition.x - myPos.x), (mousePosition.y - myPos.y)).normalized;
                myRB.velocity = lastSlashDir * InitialSpeed;
                timeSlashing += Time.deltaTime;
                if (slashDist >= MaxDistSlash)
                {
                    StopSlash();
                }
                //myRB.velocity = myRB.velocity.normalized * InitialSpeed;
                /*if (timeSlashing >= MaxTimeSlashing)
                {
                    StopSlash();
                }*/
                break;
            case SlashState.sliding:
                slashDist = slashDist + new Vector2(transform.position.x - slideStartPos.x, transform.position.y - slideStartPos.y).magnitude; ;
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
            if (slashSt == SlashState.slashing)
            {
                CheckSlash();
            }
            else if (Input.GetButtonDown("Slash") && (slashSt == SlashState.ready || slashSt == SlashState.crystal))
            {
                //Debug.Log("SLASH!");
                slash();
            }
            float v = Input.GetAxisRaw("Vertical");
            if(slashSt == SlashState.crystal && v < 0 && Input.GetButtonDown("Jump"))
            {
                ExitJumpCrystal();
            }
            else if (slashSt == SlashState.crystal && Input.GetButtonDown("Jump"))
            {
                JumpOutCrystal();
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
        slashDist = 0;
        timeSlashing = 0;
        myPos = transform.position;
        //Debug.Log("myPos=" + myPos.x + ", " + myPos.y);
        //Vector2 vel = DecomposeSpeed(InitialSpeed, mousePosition, myPos);
        mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z - camera.transform.position.z));
        lastSlashDir = new Vector2((mousePosition.x - myPos.x), (mousePosition.y - myPos.y)).normalized;
        //Debug.DrawLine(transform.position,mousePosition,Color.green,4f);
        //Debug.Log("MousePos= " + mousePosition+"; lastSlashDir= "+lastSlashDir);
        myRB.velocity = lastSlashDir * InitialSpeed;
        PlayerMovement.instance.pmState = lastSlashDir.x >= 0 ? PlayerMovement.pmoveState.stopRight : PlayerMovement.pmoveState.stopLeft;
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

    public void CheckSlash()
    {
        RaycastHit2D[] hit;
        Collider2D PBTrigger = GetComponentInChildren<PlayerBodyTrigger>().GetComponent<Collider2D>();
        float arrowWide = PBTrigger.bounds.size.y;
        float arrowHeadDist = arrowWide;
        Vector2 center = PBTrigger.bounds.center;

        Vector2 arrowHead = center +(lastSlashDir * arrowHeadDist);
        Vector2 baseDir = new Vector2(lastSlashDir.y, -lastSlashDir.x);
        Vector2 arrowBase = center + baseDir * arrowWide/2;
        int divisions = 40;
        float division = arrowWide / divisions;
        for(float i= 1; i <= divisions; i++)
        {
            Vector2 newBase = arrowBase + (baseDir * (-division * i));
            Vector2 rayDir = arrowHead - arrowBase;
            float dist = rayDir.magnitude;

            Debug.DrawLine(newBase, arrowHead, Color.yellow);
            string[] layers = { "hitBox", "Default", "Enemy", "GhostEnemy", "enemy_Boss", "Enemy_Projectile","Ground"};
            int layerMask = LayerMask.GetMask(layers);
            hit = Physics2D.RaycastAll(newBase, rayDir, dist,layerMask);
            //Debug.Log(gameObject + " COLLISION WITH " + hit.collider.gameObject);
            for(int j = 0; j < hit.Length; j++)
            {
                if (hit[j])
                {
                    //Debug.Log("hit with " + hit[j].collider.gameObject);
                    ManagePlayerAttackCollisions(hit[j].collider,j,hit[j]);
                }
            }
        }
        Debug.DrawLine(center, arrowBase, Color.red);
        Debug.DrawLine(center, arrowHead, Color.green);
        //Debug.Log("center=" + center + "; arrowBase= " + arrowBase + "; arrowHead= " + arrowHead);
    }

    void ManagePlayerAttackCollisions(Collider2D col, int ray, RaycastHit2D hit)
    {
        /*if (tag == "PlayerAttack")
        {
            for (int i = 0; i <= enemiesInsidePHB.Count; i++)
            {
                if (enemiesInsidePHB[i].enemy != col.gameObject)
                {
                    EnemyHB aux = new EnemyHB(col.gameObject, true);
                    enemiesInsidePHB.Add(aux);
                }
            }
        }*/
        if (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)//COLLISION DE HITBOX DE ATAQUE DEL JUGADOR Y HACIENDO SLASH
        {
            if (col.tag == "hitBox")
            {
                //Debug.Log("ray "+ray+":PAttack agains " + col.name);
                StartCoroutine(SplashAnim(hit.point));
                PlayerSlash.instance.StopSlash();
                PlayerMovement.instance.BounceBack(col.transform.position, PlayerMovement.instance.bounceForce / 3f);
                //Debug.Log("enemy " + transform.root.name + " recieves damage");
                /*if (col.transform.GetComponentInParent<EnemyHP>().gameObject.name.Contains("Ghost"))//menor bounce con fantasmas
                {
                }
                else
                {
                }*/
                PlayerSlash.instance.ResetSlash();
                if (col.gameObject.layer == 18)//Keeper
                {
                    if (Keeper_Phase1.instance.vulnerable)
                    {
                        Keeper_Phase1.instance.BecomeVulnerable();
                    }
                    else if (col.name.Contains("Espada"))
                    {
                        Keeper_Phase1.instance.TakeHit();
                    }
                    else if (col.name.Contains("Tongue"))
                    {
                        Keeper_Phase2.instance.TakeHit();
                    }
                }
                else
                {
                    (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
                    (col.transform.GetComponentInParent(typeof(EnemyHP)) as EnemyHP).TakeDamage(PlayerSlash.instance.slashDamage);
                }
            }
            else if (col.tag == "enemy")
            {
                //Debug.Log("ray " + ray + ":PAttack agains " + col.name);
                PlayerSlash.instance.StopSlash();
                PlayerMovement.instance.BounceBack(col.transform.position);
                if (col.gameObject.layer != 18 && !col.GetComponentInParent<EnemyHP>().gameObject.name.Contains("Heavy"))// no Keeper, no heavy
                {
                    Debug.Log("HEAVY: dont bounceback");
                    (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
                }
            }
            else if (col.gameObject.tag == "crystal")
            {
                PlayerSlash.instance.EnterCrystal(col.gameObject);
            }
            else if (col.tag == "destructible")
            {
                RespawnControler.instance.AddObject(col.GetComponent<Destructible>().myRespDestructible);
                StartCoroutine(SplashAnim(hit.point));
                PlayerSlash.instance.StopSlash();
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerSlash.instance.ResetSlash();
                col.gameObject.GetComponent<Destructible>().TakeDamage(PlayerSlash.instance.slashDamage);
            }
        }
    }
    public void EnterCrystal(GameObject crystal)
    {
        if (instance.atCrystal.attachReady)
        {
            PlayerMovement.instance.attachToCrystal(crystal);
            atCrystal = new AttachedCrystal(crystal);
        }
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
    public void JumpOutCrystal()
    {
        if (gameObject.isStatic)
        {
            gameObject.isStatic = false;
        }
        slashSt = PlayerSlash.SlashState.ready;
        jumpUpCrystal = true;
    }

    Vector2 slideStartPos;
    public void Slide(Collider2D Obstacle)
    {
        if (slashSt == SlashState.slashing)
        {
            slashSt = SlashState.sliding;
            slideStartPos = transform.position;
            //Vector2 newSlashDir =;
            //myRB.velocity = newSlashDir * InitialSpeed;


        }
    }

    private Vector2 DecomposeSpeed(float speed, Vector3 posB, Vector3 posA)
    {
        float angle = Mathf.Atan2((posB.x - posA.x), (posB.y - posA.y)) * Mathf.Rad2Deg;
        return new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
    }

    IEnumerator SplashAnim(Vector3 _position)
    {
        GameObject splash = Instantiate(slashSplash, _position, Quaternion.identity, slashSplashes);
        yield return new WaitForSeconds(0.2f);
        Destroy(splash);
    }
}
