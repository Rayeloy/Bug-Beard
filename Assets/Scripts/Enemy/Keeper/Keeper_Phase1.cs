using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_Phase1 : EnemyAI
{
    [Header("Sprites")]
    public SpriteRenderer spriteRend;
    public Sprite[] keeperSprites;
    public Vector2 standBySpritePos;
    public Vector2 standBySpriteProp;
    public Vector2[] spritesOffsets;
    public Vector2[] spritesProportions;

    private float attackTimeline;
    private int hitsTaken;
    private float patronTimeline;
    KeeperP1[] KP1_actPatron;
    KeeperP1[] KP1_patron1 = { KeeperP1.excalibur, KeeperP1.lostSouls, KeeperP1.espazado };
    KeeperP1[] KP1_patron2 = { KeeperP1.excalibur_ls, KeeperP1.nightmare, KeeperP1.espazado };
    KeeperP1[] KP1_patron3 = { KeeperP1.excalibur_ls, KeeperP1.nightmare_ls, KeeperP1.espadazo_ls };
    int patronIndex;
    bool nextSkill;

    [Header("Espadazo")]
    public float espadazoMaxTime;
    public Transform espadazoPos;
    public float timeToAttack;
    public float timeDamaging;
    public float timeRecovering;
    public float maxTimeVulnerable;
    float timeVulnerable;

    [Header("Excalibur")]
    public float excaliburMaxTime;
    public Transform excaliburPos;
    public float timeBetweenSpikes;
    float spikesTime;
    public float spikesSpeed;
    public GameObject spike;
    public Collider2D ceiling;
    public Transform spikes;
    float spikeWidth = 9.10791f;//width of the widest possible spike
    float spikeHeight = 12.91689f;

    [Header("Lost Souls")]
    public int lostSoulsMaxWaves;
    int lostSoulsWaves;
    public float lostSoulsTimeWaves;

    [Header("Nihtmare")]
    public int nightmareMaxAttacks;
    int nightmareAttacks;
    public float nightmareTimeAttacks;

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
        espazado = 0,
        excalibur = 1,
        lostSouls = 2,
        nightmare = 3,
        excalibur_ls = 4,
        nightmare_ls = 5,
        espadazo_ls = 6,
    }

    public override void Awake()
    {
        base.Awake();
        attackTimeline = 0;
        timeVulnerable = 0;
        hitsTaken = 0;
        patronIndex = 0;
        nightmareAttacks = 0;
        lostSoulsWaves = 0;
        nextSkill = false;
        moving = false;
        poseSet = false;
        AState = AttackState.ready;
        damaged = false;

        KP1_actPatron = KP1_patron1;
    }

    private void Start()
    {
        ManageCurrentSkill();
    }

    public override void Update()
    {
        gravityFalls();
        CheckGrounded();
        if (!stopEnemy)
        {
            //Debug.Log("Moving= " + moving);
            ManagePose();
            if (moving)
            {
                PositionForSkill();
            }
            else
            {
                switch (KP1)
                {
                    case KeeperP1.espazado:

                        if (AState != AttackState.vulnerable)
                        {
                            Attack();
                        }
                        else
                        {
                            timeVulnerable += Time.deltaTime;
                            if (timeVulnerable > maxTimeVulnerable)
                            {
                                AState = AttackState.ready;
                            }
                        }
                        if (patronTimeline >= espadazoMaxTime && AState == AttackState.ready)
                        {
                            nextSkill = true;
                        }
                        break;
                    case KeeperP1.excalibur:
                        DoExcalibur();
                        if (patronTimeline >= excaliburMaxTime)
                        {
                            nextSkill = true;
                        }
                        break;
                    case KeeperP1.lostSouls:
                        DoExcalibur();
                        if (lostSoulsWaves >= lostSoulsMaxWaves)
                        {
                            nextSkill = true;
                        }
                        break;
                    case KeeperP1.nightmare:
                        DoExcalibur();
                        if (nightmareAttacks >= nightmareMaxAttacks)
                        {
                            nextSkill = true;
                        }
                        break;
                    case KeeperP1.excalibur_ls:
                        DoExcalibur();
                        if (nightmareAttacks >= nightmareMaxAttacks)
                        {
                            nextSkill = true;
                        }
                        break;
                    case KeeperP1.nightmare_ls:
                        DoExcalibur();
                        if (nightmareAttacks >= nightmareMaxAttacks)
                        {
                            nextSkill = true;
                        }
                        break;
                    case KeeperP1.espadazo_ls:
                        DoExcalibur();
                        if (nightmareAttacks >= nightmareMaxAttacks)
                        {
                            nextSkill = true;
                        }
                        break;
                }
                //NO MOVER; hago comprobaciones de si patronTimeline==0 para saber si es primera entrada en cada skill
                patronTimeline += Time.deltaTime;
            }
            //NO MOVER. Deber ir siempre tras el switch
            //Debug.Log("patronTimeline= "+patronTimeline);
            if (nextSkill)
            {
                ManageCurrentSkill();
            }
        }
    }

    void ManageCurrentSkill()
    {
        if (patronIndex >= KP1_actPatron.Length)
        {
            patronIndex = 0;
        }
        KP1 = KP1_actPatron[patronIndex];
        nextSkill = false;
        patronTimeline = 0;

        poseSet = false;
        moving = true;
        patronIndex++;

        //puedo poner Ifs para solo resetear lo siguiente en caso de que sea la siguiente skill
        nightmareAttacks = 0;
        lostSoulsWaves = 0;
        spikesTime = 0;
        Debug.Log("Current skill= " + KP1);
    }

    bool poseSet = false;
    float posOlgura = 0.5f;
    void PositionForSkill()
    {
        switch (KP1)
        {
            case KeeperP1.espazado:
                MoveTowards(espadazoPos.position);
                break;
            case KeeperP1.espadazo_ls:
                MoveTowards(espadazoPos.position);
                break;
            case KeeperP1.lostSouls:
                MoveTowards(espadazoPos.position);
                break;
            case KeeperP1.excalibur:
                MoveTowards(excaliburPos.position);
                break;
            case KeeperP1.excalibur_ls:
                MoveTowards(excaliburPos.position);
                break;
        } 
    }
    bool moving = false;
    void MoveTowards(Vector3 targetPos)
    {
        if (transform.position.x < targetPos.x - posOlgura)
        {
            eState = enemyState.wRight;
            HorizontalMovement();
        }
        else if (transform.position.x > targetPos.x + posOlgura)
        {
            eState = enemyState.wLeft;
            HorizontalMovement();
        }
        else
        {
            eState = enemyState.stop;
            HorizontalMovement();
            moving = false;
        }
    }

    void SetPose(int poseIndex)
    {
        spriteRend.sprite = keeperSprites[poseIndex];
        spriteRend.transform.localPosition = standBySpritePos + spritesOffsets[poseIndex];
        spriteRend.transform.localScale = new Vector2(standBySpriteProp.x * spritesProportions[poseIndex].x, standBySpriteProp.y * spritesProportions[poseIndex].y);
    }

    void ManagePose()
    {
        if (moving)
        {
            //SetPose(1);
        }
        else if (!poseSet)
        {
            switch (KP1)
            {
                case KeeperP1.excalibur:
                    //SetPose();
                    break;
                case KeeperP1.lostSouls:
                    weakBox.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case KeeperP1.espazado:
                    weakBox.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case KeeperP1.nightmare:
                    break;
                case KeeperP1.excalibur_ls:
                    break;
                case KeeperP1.nightmare_ls:
                    break;
                case KeeperP1.espadazo_ls:
                    weakBox.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }
            poseSet = true;
        }
    }

    void DoExcalibur()
    {
        if (patronTimeline == 0)//primera vez
        {
            CameraMovement.instance.StartShakeCamera(excaliburMaxTime);
        }
        if (spikesTime >= timeBetweenSpikes)
        {
            spikesTime = 0;
            //get random pos x between range (room wide-offset); pos y will be the same always
            //no se si coge world positions o local positions...
            float MaxRange = ceiling.bounds.max.x - spikeWidth/2 - 0.3f;
            float MinRange = ceiling.bounds.min.x + spikeWidth / 2 + 0.3f;
            float posY = ceiling.bounds.min.y- spikeHeight/2 - 0.3f;//algo de olgura para evitar collision con techo
            float posX = Random.Range(MinRange, MaxRange);
            Vector3 spikePos = new Vector3(posX, posY,0);
            //spawn Spike at random pos
            GameObject newSpike=Instantiate(spike,spikePos,Quaternion.identity,spikes);
            //set spike speed downwards
            newSpike.GetComponent<Rigidbody2D>().velocity = Vector2.down * spikesSpeed;
            newSpike.GetComponent<Keeper_Spike>().konoStart();
            //spike ignore obstacles? yes
            //spike damages player on collision
            //spike destroys con collision with stage(care for colliding at spawn)

        }
        spikesTime += Time.deltaTime;
    }

    //uso esta funcion como la skill espadazo
    public override void Attack()
    {
        if (AState == AttackState.ready && EARange.CheckFor("Player"))
        {
            attackTimeline = 0;
            AState = AttackState.preparing;
            //stoppu = true;
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
                //stoppu = false;
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

    private void takeHit()
    {
        patronIndex = 0;
        hitsTaken++;
        switch (hitsTaken)
        {
            case 0:
                KP1_actPatron = KP1_patron1;
                break;
            case 1:
                KP1_actPatron = KP1_patron2;
                break;
            case 2:
                KP1_actPatron = KP1_patron3;
                break;
            case 3:
                //phase2 starts
                break;
        }
        ManageCurrentSkill();
    }
}
