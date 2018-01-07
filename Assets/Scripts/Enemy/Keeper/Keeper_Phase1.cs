using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_Phase1 : EnemyAI
{
    public static Keeper_Phase1 instance;
    [HideInInspector]
    public bool p2Start;
    public Keeper_Phase2 KeeperP2;
    public GameObject senses;

    [Header("Sprites")]
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
    KeeperP1[] KP1_actPatron;
    KeeperP1[] KP1_patron1 = { KeeperP1.excalibur, KeeperP1.lostSouls, KeeperP1.espazado };
    KeeperP1[] KP1_patron2 = { KeeperP1.excalibur_ls, KeeperP1.nightmare, KeeperP1.espazado };
    KeeperP1[] KP1_patron3 = { KeeperP1.excalibur_ls, KeeperP1.nightmare_ls, KeeperP1.espadazo_ls };
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
        KeeperP2.enabled = false;
        base.Awake();
        instance = this;
        p2Start = false;
        senses.SetActive(true);

        attackTimeline = 0;
        timeVulnerable = 0;
        bossWaitTime = 0;
        bossWait = false;

        hitsTaken = 2;
        patronIndex = 2;
        KP1_actPatron = KP1_patron3;

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


    }

    private void Start()
    {
        ManageCurrentSkill();
    }

    public override void Update()
    {
        gravityFalls();
        CheckGrounded();
        UpdateGhostsList();
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
                    switch (KP1)
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
                if (nextSkill && !p2Start)
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
        sprite.sprite = keeperSprites[poseIndex];
        sprite.transform.localPosition = standBySpritePos + spritesOffsets[poseIndex];
        sprite.transform.localScale = new Vector2(standBySpriteProp.x * spritesProportions[poseIndex].x, standBySpriteProp.y * spritesProportions[poseIndex].y);
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
                        case AttackState.damagedAfterVulnerable:
                            SetPose(3);
                            break;
                    }
                    break;
            }
            poseSet = true;
        }
    }

    void DoExcalibur()
    {
        if (patronTimeline < excaliburMaxTime)
        {
            if (patronTimeline == 0)//primera vez
            {
                spikesTime = 0;
            }
            if (excaliburCharging && spikesTime >= 1.5f)
            {
                spikesTime = 0;
                excaliburCharging = false;
                poseSet = false;
                CameraMovement.instance.StartShakeCamera(excaliburMaxTime);
            }
            else if (!excaliburCharging && spikesTime >= timeBetweenSpikes)
            {
                spikesTime = 0;
                //get random pos x between range (room wide-offset); pos y will be the same always
                //no se si coge world positions o local positions...
                float MaxRange = ceiling.bounds.max.x - spikeWidth / 2 - 0.3f;
                float MinRange = ceiling.bounds.min.x + spikeWidth / 2 + 0.3f;
                float posY = ceiling.bounds.min.y - spikeHeight / 2 - 0.3f;//algo de olgura para evitar collision con techo
                float posX = Random.Range(MinRange, MaxRange);
                Vector3 spikePos = new Vector3(posX, posY, 0);
                //spawn Spike at random pos
                GameObject newSpike = Instantiate(spike, spikePos, Quaternion.identity, spikes);
                //set spike speed downwards
                newSpike.GetComponent<Rigidbody2D>().velocity = Vector2.down * spikesSpeed;
                newSpike.GetComponent<Keeper_Spike>().konoStart();
                //spike ignore obstacles? yes
                //spike damages player on collision
                //spike destroys con collision with stage(care for colliding at spawn)
            }
            spikesTime += Time.deltaTime;
        }
    }

    void DoEspadazo()
    {
        if (patronTimeline == 0)
        {
            doesPursue = true;
            attackTimeline = 0;
        }
        if (AState != AttackState.vulnerable && AState != AttackState.damagedAfterVulnerable)
        {
            HorizontalMovement();
            Attack();
        }
        else if (AState == AttackState.vulnerable)
        {
            DoVulnerable();
        }
        else if (AState == AttackState.damagedAfterVulnerable)
        {
            DoDamagedAfterVulnerable();
        }
    }
    //uso esta funcion como la skill espadazo
    public override void Attack()
    {
        if (AState == AttackState.ready && EARange.CheckFor("Player"))
        {
            attackTimeline = 0;
            AState = AttackState.preparing;
            poseSet = false;
            weakBox.SetActive(true);
            vulnerable = true;
            Debug.Log("stoppu true");
            stoppu = true;
            //garrote.SetActive(true);
            //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y + 3);
        }
        else
        {
            //PREPARING ATTACK
            if (attackTimeline > timeToAttack && attackTimeline < timeToAttack + timeDamaging)//quito la condicion  && AState == AttackState.preparing, la pongo abajo vv
            {

                if (!damaged && EAHitBox.CheckFor("Player"))
                {
                    damaged = true;
                    PlayerHP.instance.TakeDamage(eHP.damage);
                }
                if (AState == AttackState.preparing)//separo la condición aqui abajo para poder comprobar si hace daño constantemente y no una sola vez
                {
                    AState = AttackState.damaging;
                    poseSet = false;
                    //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 3);
                }
            }
            //ATTACKING
            else if (attackTimeline > timeDamaging + timeToAttack && AState == AttackState.damaging)
            {
                vulnerable = false;
                weakBox.SetActive(false);

                damaged = false;
                AState = AttackState.recovering;
                poseSet = false;
                //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y - 1);
            }
            //RECOVERING
            else if (attackTimeline > timeDamaging + timeToAttack + timeRecovering && AState == AttackState.recovering)
            {
                Debug.Log("ATTACK READY");
                AState = AttackState.ready;
                poseSet = false;
                stoppu = false;
                // garrote.SetActive(false);
                //garrote.transform.position = new Vector2(garrote.transform.position.x, garrote.transform.position.y + 1);
            }
        }
        if (AState == AttackState.preparing || AState == AttackState.damaging || AState == AttackState.recovering)
        {
            attackTimeline += Time.deltaTime;
        }
        else if (AState == AttackState.damaged)
        {
            attackTimeline += Time.deltaTime;
            if (attackTimeline >= maxTimeDamaged)
            {
                attackTimeline = 0;
                timeVulnerable = 0;
                espada.enabled = true;
                AState = AttackState.vulnerable;
                poseSet = false;
            }
        }
    }
    void DoVulnerable()
    {
        timeVulnerable += Time.deltaTime;
        if (timeVulnerable > maxTimeVulnerable)
        {
            Debug.Log("ATTACK READY");
            AState = AttackState.ready;
            espada.enabled = false;
            poseSet = false;
        }
    }

    float lostSoulsActTimeWaves;
    void DoLostSouls()
    {
        //SetPose
        if (patronTimeline == 0)
        {
            Debug.Log("portal opened!");
            portal.enabled = true;
            lostSoulsTime = 0;
            lostSoulsWaves = 0;
            lostSoulsActTimeWaves = lostSoulsTimeWaves / 2;
        }
        if (lostSoulsTime >= lostSoulsActTimeWaves && lostSoulsWaves < lostSoulsMaxWaves)
        {
            for (int i = 0; i < ghostsPerWave; i++)
            {
                //pos Y 
                //pos X
                float miny = portal.bounds.min.y;
                float maxy = portal.bounds.max.y;
                float y = Random.Range(miny, maxy);
                float x = portal.bounds.center.x - 0.5f;
                Vector3 spawnPos = new Vector3(x, y, 0);
                GameObject newGhost = Instantiate(ghost, spawnPos, Quaternion.identity, ghosts);
                ghostsList.Add(newGhost);
            }
            lostSoulsWaves++;
            lostSoulsTime = 0;
            lostSoulsActTimeWaves = lostSoulsTimeWaves;
        }
        lostSoulsTime += Time.deltaTime;
    }
    void UpdateGhostsList()
    {
        for (int i = 0; i < ghostsList.Count; i++)
        {
            if (ghostsList[i] == null)
            {
                ghostsList.RemoveAt(i);
            }
        }
    }

    float nightmareAttackFollowSpeedX;
    float nightmareAttackFollowSpeedY;
    float attackPosX;
    float attackPosY;
    void DoNightmare()
    {
        if (patronTimeline == 0)
        {
            nightmareAttacks = 0;
            nightmareTime = 0;
            attackPosX = PlayerMovement.instance.transform.position.x;
            attackPosY = PlayerMovement.instance.transform.position.y;
            //nightmareAttack.GetComponent<Collider2D>().enabled = false;
            nightmareAttack.GetComponent<SpriteRenderer>().enabled = false;
            nightmareAttack.GetComponent<SpriteRenderer>().sortingOrder = -1;
            nightmareAttack.GetComponent<SpriteRenderer>().sprite = nightmareAttackWarn;
            nightmareAttacking = false;
            dissapeared = false;
            colliders[0].enabled = false;
        }
        if (nightmareAttacks < nightmareMaxAttacks)
        {
            if (nightmareAttacks == 0 && !dissapeared)//desaparece al principio
            {
                float prog = nightmareTime / nightmareMaxTimeDissapear;
                float a = Mathf.Lerp(0, 0.7f, prog);
                float aBoss = Mathf.Lerp(1, 0, prog);
                float aEyes = Mathf.Lerp(0, 1, prog);
                Color colorFilter = new Color(0, 0, 0, a);
                Color colorBoss = new Color(1, 1, 1, aBoss);
                Color colorEyes = new Color(1, 1, 1, aEyes);
                CameraMovement.instance.BlackFilter.color = colorFilter;
                sprite.color = colorBoss;
                bossEyes.color = colorEyes;
                if (nightmareTime >= nightmareMaxTimeDissapear)
                {
                    dissapeared = true;
                    nightmareTime = 0;
                    nightmareAttack.transform.position = PlayerMovement.instance.transform.position;
                    nightmareAttack.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            else//ataques
            {
                if (!nightmareAttacking)//follow and attack
                {
                    attackPosX = Mathf.SmoothDamp(attackPosX, PlayerMovement.instance.transform.position.x, ref nightmareAttackFollowSpeedX, nightmareAttackFollowSmooth);
                    attackPosY = Mathf.SmoothDamp(attackPosY, PlayerMovement.instance.transform.position.y, ref nightmareAttackFollowSpeedY, nightmareAttackFollowSmooth);
                    nightmareAttack.transform.position = new Vector3(attackPosX, attackPosY, 0);
                    if (nightmareTime >= nightmareTimeFollowingPlayer)//attack!!
                    {
                        nightmareAttack.GetComponent<SpriteRenderer>().sortingOrder = PlayerAnimations.instance.SpriteRend.sortingOrder + 1;
                        nightmareAttack.GetComponent<SpriteRenderer>().sprite = nightmareAttackActivated;
                        //nightmareAttack.GetComponent<Collider2D>().enabled = true;
                        if (nightmareAttack.GetComponent<CheckHitBox>().CheckFor("Player"))
                        {
                            PlayerMovement.instance.BounceBack(nightmareAttack.transform.position);
                            PlayerHP.instance.TakeDamage(GetComponent<EnemyHP>().damage);
                        }
                        nightmareTime = 0;
                        nightmareAttacking = true;
                    }
                }
                else if (nightmareAttacking)//recovering attack
                {
                    if (nightmareTime >= nightmareAttackingTime)
                    {
                        //reset attack
                        nightmareTime = 0;
                        nightmareAttacking = false;
                        nightmareAttack.transform.position = PlayerMovement.instance.transform.position;
                        nightmareAttack.GetComponent<SpriteRenderer>().sprite = nightmareAttackWarn;
                        nightmareAttack.GetComponent<SpriteRenderer>().sortingOrder = -1;
                        nightmareAttacks++;
                    }
                }
            }
        }
        else//aparece
        {
            nightmareAttack.GetComponent<SpriteRenderer>().enabled = false;
            float prog = nightmareTime / nightmareMaxTimeDissapear;
            float a = Mathf.Lerp(0.7f, 0, prog);
            float aBoss = Mathf.Lerp(0, 1, prog);
            float aEyes = Mathf.Lerp(1, 0, prog);
            Color colorFilter = new Color(0, 0, 0, a);
            Color colorBoss = new Color(1, 1, 1, aBoss);
            Color colorEyes = new Color(1, 1, 1, aEyes);
            CameraMovement.instance.BlackFilter.color = colorFilter;
            sprite.color = colorBoss;
            bossEyes.color = colorEyes;
            if (nightmareTime >= nightmareMaxTimeDissapear)
            {
                dissapeared = false;
                nightmareTime = 0;
                colliders[0].enabled = true;
            }
        }
        nightmareTime += Time.deltaTime;
    }

    public void BecomeVulnerable()
    {
        Debug.Log("VULNERABLE!");
        //doesPursue = false;
        weakBox.SetActive(false);
        vulnerable = false;//esta variable es para saber si está activa la hitbox de la rodilla
        AState = AttackState.damaged;
        poseSet = false;
        //animación a vulnerable
        //espada vulnerable 
        attackTimeline = 0;
    }
    void DoDamagedAfterVulnerable()
    {
        //Debug.Log("damaged after vulnerable");
        attackTimeline += Time.deltaTime;
        if (attackTimeline >= maxTimeDamaged)
        {
            stoppu = false;
            doesPursue = false;
            if (p2Start)
            {
                //parar lost souls
                lostSoulsWaves = lostSoulsMaxWaves + 1;
                lostSoulsTime = lostSoulsTimeWaves * 2 + 1;
                for (int i = 0; i < ghosts.childCount; i++)
                {
                    Destroy(ghosts.GetChild(i).gameObject);
                }
                //quitamos colliders
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                }
                senses.SetActive(false);
                //quitamos luces de keeper 1
                luces[0].enabled = false;
                luces[0].enabled = false;
                KeeperP2.enabled = true;
                Debug.Log("PHASE 2");
                this.enabled = false;
                //START PHASE2
            }
            attackTimeline = 0;
            AState = AttackState.ready;
            poseSet = false;


        }
    }

    public void TakeHit()
    {
        portal.enabled = false;
        if (hitsTaken < 3)
        {
            bossWaitTime = 0;
            bossWait = true;
            if (AState == AttackState.vulnerable)
            {
                espada.enabled = false;
                AState = AttackState.damagedAfterVulnerable;
                patronTimeline = espadazoMaxTime + 1;
                poseSet = false;
            }
            patronIndex = 0;
        }
        hitsTaken++;
        Debug.Log("hitsTaken=" + hitsTaken);
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
                p2Start = true;//en DoDamagedAfterVulnerable() se comienza la phase 2
                break;
        }

    }
}
