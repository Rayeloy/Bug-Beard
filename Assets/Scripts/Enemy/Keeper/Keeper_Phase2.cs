using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_Phase2 : HeavyAI {

    public static Keeper_Phase2 instance;

    [Header("Sprites")]
    public SpriteRenderer spriteRend;
    public Sprite[] keeperSprites;
    Vector2 standBySpritePos;
    Vector2 standBySpriteProp;
    public Vector2[] spritesOffsets;//ORDEN: Standby, Anticipation, Attack, Damaged, Vulnerable,
    public Vector2[] spritesProportions;
    public Collider2D[] colliders;
    public SpriteRenderer[] luces;

    [Header("BOSS SKILLS")]
    float bossWaitTime;
    bool bossWait;
    [Tooltip("Max time the boss will wait on standBy after Excalibur and Espadazo")]
    public float bossMaxWaitTime;
    private float attackTimeline;
    private int hitsTaken;
    private float patronTimeline;
    KeeperP2[] KP2_actPatron;
    KeeperP2[] KP2_patron1 = { KeeperP2.excalibur, KeeperP1.lostSouls, KeeperP1.espazado };
    KeeperP2[] KP2_patron2 = { KeeperP1.excalibur_ls, KeeperP1.nightmare, KeeperP1.espazado };
    KeeperP2[] KP2_patron3 = { KeeperP1.excalibur_ls, KeeperP1.nightmare_ls, KeeperP1.espadazo_ls };
    int patronIndex;
    bool nextSkill;

    [Header("Espadazo")]
    public Collider2D espada;
    public float espadazoMaxTime;
    public Transform espadazoPos;
    public float timeToAttack;
    public float timeDamaging;
    public float timeRecovering;
    public float maxTimeDamaged;
    public float maxTimeVulnerable;
    float timeVulnerable;

    [Header("Excalibur")]
    public float excaliburMaxTime;
    bool excaliburCharging;
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
    float lostSoulsTime;
    public int ghostsPerWave;
    public GameObject ghost;
    public SpriteRenderer portal;
    public Transform ghosts;
    List<GameObject> ghostsList;

    [Header("Nihtmare")]
    public int nightmareMaxAttacks;
    int nightmareAttacks;
    float nightmareTime;
    public float nightmareTimeFollowingPlayer;
    public float nightmareAttackFollowSmooth;
    [Tooltip("Time that the attack is static, recovering for next attack")]
    public float nightmareAttackingTime;
    bool nightmareAttacking;
    [Tooltip("Time that the boss takes to completely dissapear and the black filter appears")]
    public float nightmareMaxTimeDissapear;
    bool dissapeared;
    public SpriteRenderer bossEyes;
    public GameObject nightmareAttack;
    public Sprite nightmareAttackWarn;
    public Sprite nightmareAttackActivated;

    public EnemyHP eHP;
    public CheckHitBox EARange;
    public CheckHitBox EAHitBox;
    private bool damaged;
    [HideInInspector]
    public bool vulnerable = false;//esta variable es para saber si está activa la hitbox de la rodilla

    [HideInInspector]
    public KeeperP2 KP2 = KeeperP2.rugido;
    public enum KeeperP2
    {
        rugido = 0,
        zarpazoEspectral = 1,
        AcidExalation = 2,
        RayoFatuo = 3,
        rugido_ZarpazoEspectral=4,
        rugido_RayoFatuo=5,
        rugido_AcidExalation=6
    }

    public override void Awake()
    {
        base.Awake();
        instance = this;

        attackTimeline = 0;
        timeVulnerable = 0;
        bossWaitTime = 0;
        bossWait = false;
        hitsTaken = 0;
        patronIndex = 0;
        nextSkill = false;
        moving = false;
        poseSet = false;
        AState = AttackState.ready;
        damaged = false;

        luces[0].enabled = false;
        luces[1].enabled = false;

        standBySpritePos = spritesOffsets[0];
        standBySpriteProp = spritesProportions[0];

        nightmareAttacks = 0;
        nightmareTime = 0;
        dissapeared = false;
        nightmareAttacking = false;

        lostSoulsWaves = 0;
        lostSoulsTime = 0;
        ghostsList = new List<GameObject>();

        KP2_actPatron = KP2_patron1;
    }

    private void Start()
    {
        ManageCurrentSkill();
    }

    public override void Update()
    {
        gravityFalls();
        CheckGrounded();
        //UpdateGhostsList();
        if (!stopEnemy)
        {
            //Debug.Log("Moving= " + moving);
            ManagePose();
            if (bossWait)
            {
                bossWaitTime += Time.deltaTime;
                if (bossWaitTime >= bossMaxWaitTime)
                {
                    bossWait = false;
                }
            }
            else
            {
                if (moving)
                {
                    PositionForSkill();
                }
                else
                {
                    switch (KP2)
                    {
                        case KeeperP1.espazado:
                            DoEspadazo();
                            if (patronTimeline >= espadazoMaxTime && AState == AttackState.ready)//cuando termina espadazo sin recibir hit
                            {
                                doesPursue = false;
                                stoppu = false;
                                nextSkill = true;
                            }
                            break;
                        case KeeperP1.excalibur:
                            DoExcalibur();
                            if (patronTimeline >= excaliburMaxTime + bossMaxWaitTime)
                            {

                                nextSkill = true;
                            }
                            break;
                        case KeeperP1.lostSouls:
                            DoLostSouls();
                            if (lostSoulsWaves >= lostSoulsMaxWaves && lostSoulsTime >= lostSoulsTimeWaves * 2)//da tiempo a matar a los últimos ghosts
                            {
                                portal.enabled = false;
                                nextSkill = true;
                            }
                            break;
                        case KeeperP1.nightmare:
                            DoNightmare();
                            if (nightmareAttacks >= nightmareMaxAttacks && !dissapeared)
                            {
                                nextSkill = true;
                            }
                            break;
                        case KeeperP1.excalibur_ls:
                            DoExcalibur();
                            DoLostSouls();
                            if (patronTimeline >= excaliburMaxTime + bossMaxWaitTime && lostSoulsWaves >= lostSoulsMaxWaves && lostSoulsTime >= lostSoulsTimeWaves * 2)//da tiempo a matar a los últimos ghosts
                            {
                                portal.enabled = false;
                                nextSkill = true;
                            }
                            break;
                        case KeeperP1.nightmare_ls:
                            DoNightmare();
                            DoLostSouls();
                            if (nightmareAttacks >= nightmareMaxAttacks && !dissapeared && lostSoulsWaves >= lostSoulsMaxWaves && lostSoulsTime >= lostSoulsTimeWaves * 2)
                            {
                                portal.enabled = false;
                                nextSkill = true;
                            }
                            break;
                        case KeeperP1.espadazo_ls:
                            DoLostSouls();
                            DoEspadazo();
                            if (lostSoulsWaves >= lostSoulsMaxWaves && lostSoulsTime >= lostSoulsTimeWaves * 2 && patronTimeline >= espadazoMaxTime && AState == AttackState.ready)//da tiempo a matar a los últimos ghosts
                            {
                                portal.enabled = false;
                                doesPursue = false;
                                stoppu = false;
                                nextSkill = true;
                            }
                            break;
                    }
                    if (!nextSkill)
                    {
                        //NO MOVER; hago comprobaciones de si patronTimeline==0 para saber si es primera entrada en cada skill
                        patronTimeline += Time.deltaTime;
                    }
                }
                //NO MOVER. Deber ir siempre tras el switch
                //Debug.Log("patronTimeline= "+patronTimeline);
                if (nextSkill)
                {
                    ManageCurrentSkill();
                }
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
        excaliburCharging = true;
        Debug.Log("Current skill= " + KP1);
    }

    bool poseSet = false;
    float posOlgura = 0.5f;
    void PositionForSkill()
    {
        switch (KP1)
        {
            case KeeperP1.espazado:
                moving = false;
                break;
            case KeeperP1.espadazo_ls:
                moving = false;
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
            case KeeperP1.nightmare:
                moving = false;
                break;
            case KeeperP1.nightmare_ls:
                moving = false;
                break;
        }
    }
    bool moving = false;
    void MoveTowards(Vector3 targetPos)
    {
        if (transform.position.x < targetPos.x - posOlgura)
        {
            eState = enemyState.wRight;
        }
        else if (transform.position.x > targetPos.x + posOlgura)
        {
            eState = enemyState.wLeft;
        }
        else
        {
            eState = enemyState.stop;
            moving = false;
        }
        //Debug.Log("eState= " + eState + "; stoppu= " + stoppu);
        HorizontalMovement();
    }

    void SetPose(int poseIndex)
    {
        spriteRend.sprite = keeperSprites[poseIndex];
        spriteRend.transform.localPosition = standBySpritePos + spritesOffsets[poseIndex];
        spriteRend.transform.localScale = new Vector2(standBySpriteProp.x * spritesProportions[poseIndex].x, standBySpriteProp.y * spritesProportions[poseIndex].y);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (i == poseIndex)
            {
                colliders[i].enabled = true;
            }
            else
            {
                colliders[i].enabled = false;
            }
        }
        if (poseIndex == 1)
        {
            luces[0].enabled = true;
            luces[1].enabled = false;
        }
        else if (poseIndex == 4)
        {
            luces[0].enabled = false;
            luces[1].enabled = true;
        }
        else
        {
            luces[0].enabled = false;
            luces[1].enabled = false;
        }
    }

    void ManagePose()
    {
        if (moving)
        {
            SetPose(0);
        }
        else if (!poseSet)
        {
            switch (KP1)
            {
                case KeeperP1.excalibur:
                    if (excaliburCharging)
                    {
                        SetPose(5);
                    }
                    else
                    {
                        SetPose(6);
                    }

                    break;
                case KeeperP1.lostSouls:
                    weakBox.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
                    SetPose(7);
                    break;
                case KeeperP1.espazado:
                    switch (AState)
                    {
                        case AttackState.ready:
                            SetPose(0);
                            break;
                        case AttackState.preparing:
                            SetPose(1);
                            break;
                        case AttackState.damaging:
                            SetPose(2);
                            break;
                        case AttackState.recovering:
                            SetPose(2);
                            break;
                        case AttackState.damaged:
                            SetPose(3);
                            break;
                        case AttackState.vulnerable:
                            SetPose(4);
                            break;
                        case AttackState.damagedAfterVulnerable:
                            SetPose(3);
                            break;
                    }
                    break;
                case KeeperP1.nightmare:
                    if (patronTimeline == 0)
                    {
                        SetPose(0);
                    }
                    break;
                case KeeperP1.excalibur_ls:
                    if (excaliburCharging)
                    {
                        SetPose(5);
                    }
                    else
                    {
                        SetPose(6);
                    }
                    break;
                case KeeperP1.nightmare_ls:
                    if (patronTimeline == 0)
                    {
                        SetPose(0);
                    }
                    break;
                case KeeperP1.espadazo_ls:
                    switch (AState)
                    {
                        case AttackState.ready:
                            SetPose(0);
                            break;
                        case AttackState.preparing:
                            SetPose(1);
                            break;
                        case AttackState.damaging:
                            SetPose(2);
                            break;
                        case AttackState.recovering:
                            SetPose(2);
                            break;
                        case AttackState.damaged:
                            SetPose(3);
                            break;
                        case AttackState.vulnerable:
                            SetPose(4);
                            break;
                    }
                    break;
            }
            poseSet = true;
        }
    }
}
