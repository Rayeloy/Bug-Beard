using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_Phase2 : EnemyAI
{

    public static Keeper_Phase2 instance;
    SpriteRenderer playerSprite;
    public GameObject hotSpot1;
    public GameObject hotSpot2;

    [Header("Transition")]
    public float espadaRotaMaxTime;
    public float standByMaxTime;
    public float scenaryChangeMaxTime;
    [Tooltip("Time that the boss takes to completely dissapear and the black filter appears")]
    public float maxTimeDissapear;
    int dissapeared;//0 start, 1 wait in dissapeared, 2 Start appear, 3 appear again
    bool inTransition;
    Transition Trans;
    enum Transition
    {
        bossAnimation,
        bossDissapear,
        stageChanging,
        fightStarts
    }
    public Transform safePlayerPos;
    public Transform bossOriginalPos;
    bool chenji;
    public GameObject[] posScenary;//grounds from left to right (only the ones that change), Platforms from left to right, crystals from left to right
    public Transform[] newScenary;
    List<Vector3> OrigPosScenary;
    List<Vector3> newPosScenary;

    [Header("Sprites")]
    public Sprite[] keeperSprites;//ORDEN: StandBy=0, Damaged=1, Rugido=2, ZarpazaoEspectral=3,AcidExalation=4, RayoFatuo=5, Muerto=6, Anticipation=7
    Vector2 standBySpritePos;
    Vector2 standBySpriteProp;
    public Vector2[] spritesOffsets;
    public Vector2[] spritesProportions;
    public Collider2D[] colliders;
    public SpriteRenderer[] luces;

    [Header("BOSS SKILLS")]
    float bossWaitTime;
    bool bossWait;
    [Tooltip("Max time the boss will wait on standBy after Rugido")]
    public float bossMaxWaitTime;
    private float attackTimeline;
    private int hitsTaken;
    private float patronTimeline;
    KeeperP2[] KP2_actPatron;
    KeeperP2[] KP2_patron1 = { KeeperP2.rugido, KeeperP2.zarpazoEspectral, KeeperP2.AcidExalation };
    KeeperP2[] KP2_patron2 = { KeeperP2.rugido_ZarpazoEspectral, KeeperP2.RayoFatuo, KeeperP2.AcidExalation };
    KeeperP2[] KP2_patron3 = { KeeperP2.rugido_ZarpazoEspectral, KeeperP2.rugido_RayoFatuo, KeeperP2.rugido_AcidExalation };
    int patronIndex;
    bool nextSkill;

    [Header("Rugido")]
    public float rugidoMaxTime;
    bool rugidoCharging;
    public float timeBetweenSpikes;
    float spikesTime;
    public float spikesSpeed;
    public GameObject spike;
    public Collider2D ceiling;
    public Transform spikes;
    float spikeWidth = 9.10791f;//width of the widest possible spike
    float spikeHeight = 12.91689f;

    [Header("Zarpazo Espectral")]
    [Tooltip("Zarpazos Espectrales al mismo tiempo en el patrón 1, 2 y 3")]
    public int[] zarpEspMaxAttacks = new int[3];
    int zarpEspAttacks;//ATAQUES AL MISMO TIEMPO
    public float zarpEspWarnMaxTime;
    public float zarpEspAttackSpeed;
    public float zarpEspAttackingMaxTime;
    public GameObject zarpEspPrefab;
    public GameObject warningLight;
    public Transform zarpazosEspectrales;
    public Transform warningLights;
    float zarpEspTime;
    public enum ZarpEspState
    {
        calculating,
        warning,
        start,
        going,
        attacking,
        returning,
        returned
    }
    ZarpEspState ZEState;
    bool allZarpazosStopped;
    bool allZarpazosReturned;


    [Header("Acid Exalation")]
    public Transform acidExalOrigin;
    public float acidExalTimeBetweenAttacks;
    public float acidExalMaxTime;
    public float acidExalAttacksSpeed;
    public float acidExalAngle;//º ,not radians
    public GameObject acidExalPrefab;
    public Collider2D tongue;
    public Transform acidDrops;
    float acidExalTime;

    [Header("Rayo Fatuo")]


    public EnemyHP eHP;
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
        rugido_ZarpazoEspectral = 4,
        rugido_RayoFatuo = 5,
        rugido_AcidExalation = 6
    }

    public void KonoStart()
    {
        instance = this;
        playerSprite = PlayerMovement.instance.spriteTransf.GetComponent<SpriteRenderer>();

        inTransition = true;
        Trans = Transition.bossAnimation;
        dissapeared = 0;
        chenji = false;
        OrigPosScenary = new List<Vector3>();
        for (int i = 0; i < posScenary.Length; i++)
        {
            OrigPosScenary.Add(posScenary[i].transform.position);
        }
        newPosScenary = new List<Vector3>();
        for (int i = 0; i < newScenary.Length; i++)
        {
            newPosScenary.Add(newScenary[i].transform.position);
        }

        attackTimeline = 0;
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
        //luces[1].enabled = false;

        standBySpritePos = spritesOffsets[0];
        standBySpriteProp = spritesProportions[0];

        zarpEspAttacks = 0;
        zarpEspTime = 0;

        tongue.enabled = false;

        KP2_actPatron = KP2_patron1;
    }
    private void OnEnable()
    {
        KonoStart();
        poseSet = false;
        SetPose(1);
        PlayerMovement.instance.stopPlayer = true;
    }


    public override void Update()
    {
        if (inTransition)
        {
            DoTransition();
        }
        else
        {
            CheckGrounded();
            gravityFalls();
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
                    switch (KP2)
                    {
                        case KeeperP2.rugido:
                            DoRugido();
                            if (patronTimeline >= rugidoMaxTime + bossMaxWaitTime)
                            {

                                nextSkill = true;
                            }
                            break;
                        case KeeperP2.zarpazoEspectral:
                            DoZarpazoEspectral();
                            if (ZEState == ZarpEspState.returned)
                            {
                                for(int i=0;i<zarpazosEspectrales.childCount; i++)
                                {
                                    Destroy(zarpazosEspectrales.GetChild(i).gameObject);
                                }
                                nextSkill = true;
                            }
                            break;
                        case KeeperP2.AcidExalation:
                            if (patronTimeline >= acidExalMaxTime)
                            {
                                tongue.enabled = false;
                                nextSkill = true;
                            }
                                break;
                        case KeeperP2.RayoFatuo:
                            break;
                        case KeeperP2.rugido_ZarpazoEspectral:
                            break;
                        case KeeperP2.rugido_AcidExalation:
                            break;
                        case KeeperP2.rugido_RayoFatuo:
                            break;
                    }
                    if (!nextSkill)
                    {
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
        }
    }

    void DoTransition()
    {
        switch (Trans)
        {
            //TRANSICIÓN
            case Transition.bossAnimation:
                if (patronTimeline >= espadaRotaMaxTime)
                {
                    patronTimeline = 0;
                    Trans = Transition.bossDissapear;
                    poseSet = false;
                    SetPose(0);
                }
                break;
            //boss dissapears and appears on left side
            case Transition.bossDissapear:
                switch (dissapeared)
                {
                    case 0:
                        float prog = patronTimeline / maxTimeDissapear;
                        float a = Mathf.Lerp(0, 1, prog);
                        float aBoss = Mathf.Lerp(1, 0, prog);
                        Color colorFilter = new Color(0, 0, 0, a);
                        Color colorBoss = new Color(1, 1, 1, aBoss);
                        CameraMovement.instance.BlackFilter.color = colorFilter;
                        sprite.color = colorBoss;
                        playerSprite.color = colorBoss;
                        if (patronTimeline >= maxTimeDissapear)
                        {
                            patronTimeline = 0;
                            dissapeared++;
                        }
                        break;
                    case 1://move player && boss
                        if (!chenji && patronTimeline >= maxTimeDissapear / 2)
                        {
                            chenji = true;
                            PlayerMovement.instance.transform.position = safePlayerPos.position;
                            PlayerMovement.instance.pmState = PlayerMovement.pmoveState.stopLeft;
                            playerSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
                            weakBox.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
                            weakBox.transform.parent.position = bossOriginalPos.position;
                            hotSpot1.SetActive(false);
                            hotSpot2.SetActive(true);

                        }
                        if (patronTimeline >= maxTimeDissapear)
                        {
                            patronTimeline = 0;
                            dissapeared++;
                        }
                        break;
                    case 2://appear again
                        prog = patronTimeline / maxTimeDissapear;
                        a = Mathf.Lerp(1, 0, prog);
                        aBoss = Mathf.Lerp(0, 1, prog);
                        colorFilter = new Color(0, 0, 0, a);
                        colorBoss = new Color(1, 1, 1, aBoss);
                        CameraMovement.instance.BlackFilter.color = colorFilter;
                        sprite.color = colorBoss;
                        playerSprite.color = colorBoss;
                        if (patronTimeline >= maxTimeDissapear)
                        {
                            patronTimeline = 0;
                            dissapeared++;
                        }
                        break;
                    case 3:
                        patronTimeline = 0;
                        poseSet = false;
                        SetPose(2);
                        CameraMovement.instance.StartShakeCamera(scenaryChangeMaxTime);
                        Trans = Transition.stageChanging;
                        /*for(int i = 0; i <= posScenary.Length; i++)
                        {
                            posScenary[i].GetComponent<Collider2D>().enabled = false;
                        }*/
                        break;
                }
                break;
            //boss roars, camera shakes,  stage changes
            case Transition.stageChanging:
                float prog2 = patronTimeline / scenaryChangeMaxTime;
                for (int i = 0; i < posScenary.Length; i++)
                {
                    //float newX = Mathf.Lerp(OrigPosScenary[i].x, newPosScenary[i].x, prog2);
                    //float newY = Mathf.Lerp(OrigPosScenary[i].y, newPosScenary[i].y, prog2);
                    float newX = EasingCurves.easeInOutQuad(OrigPosScenary[i].x, newPosScenary[i].x, prog2);
                    float newY = EasingCurves.easeInOutQuad(OrigPosScenary[i].y, newPosScenary[i].y, prog2);
                    posScenary[i].transform.position = new Vector3(newX, newY, 0);
                }
                if (patronTimeline >= scenaryChangeMaxTime)
                {
                    Trans = Transition.fightStarts;
                    patronTimeline = 0;
                }
                break;
            //battle starts
            case Transition.fightStarts:
                ManageCurrentSkill();
                inTransition = false;
                PlayerMovement.instance.stopPlayer = false;
                stopEnemy = false;
                patronTimeline = 0;
                break;
        }
        if (inTransition)
        {
            patronTimeline += Time.deltaTime;
        }
    }
    void ManageCurrentSkill()
    {
        if (patronIndex >= KP2_actPatron.Length)
        {
            patronIndex = 0;
        }
        KP2 = KP2_actPatron[patronIndex];
        nextSkill = false;
        patronTimeline = 0;

        poseSet = false;
        patronIndex++;

        //puedo poner Ifs para solo resetear lo siguiente en caso de que sea la siguiente skill
        spikesTime = 0;
        rugidoCharging = true;
        Debug.Log("Current skill= " + KP2);
    }

    float posOlgura = 0.5f;
    /*void PositionForSkill()
    {
        switch (KP2)
        {
            case KeeperP2.espazado:
                moving = false;
                break;
            case KeeperP2.espadazo_ls:
                moving = false;
                break;
            case KeeperP2.lostSouls:
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
    }*/
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

    bool poseSet = false;
    void SetPose(int poseIndex)
    {
        sprite.sprite = keeperSprites[poseIndex];
        sprite.transform.localPosition = spritesOffsets[poseIndex];
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
        if (poseIndex == 4)
        {
            luces[0].enabled = true;
            //luces[1].enabled = false;
        }
        else
        {
            luces[0].enabled = false;
            //luces[1].enabled = false;
        }
    }
    void ManagePose()
    {
        if (!poseSet)
        {
            switch (KP2)
            {
                case KeeperP2.rugido:
                    if (rugidoCharging)
                    {
                        SetPose(0);
                    }
                    else
                    {
                        SetPose(2);
                    }
                    break;
                case KeeperP2.zarpazoEspectral:
                    SetPose(3);
                    break;
                case KeeperP2.AcidExalation:
                    SetPose(4);
                    break;
                case KeeperP2.RayoFatuo:
                    SetPose(5);
                    break;
                case KeeperP2.rugido_ZarpazoEspectral:
                    SetPose(3);
                    break;
                case KeeperP2.rugido_AcidExalation:
                    SetPose(4);
                    break;
                case KeeperP2.rugido_RayoFatuo:
                    SetPose(5);
                    break;
            }
            poseSet = true;
        }
    }

    void DoRugido()
    {
        if (patronTimeline < rugidoMaxTime)
        {
            if (patronTimeline == 0)//primera vez
            {
                Debug.Log("RUGIDO FIRST TIME");
                spikesTime = 0;
                rugidoCharging = true;
                poseSet = false;//por si acaso
            }
            if (rugidoCharging && spikesTime >= 1.5f)
            {
                Debug.Log("RUGIDO CHARGED");
                spikesTime = 0;
                rugidoCharging = false;
                poseSet = false;
                CameraMovement.instance.StartShakeCamera(rugidoMaxTime);
            }
            else if (!rugidoCharging && spikesTime >= timeBetweenSpikes)
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
    List<ZarpEspInfo> zarpazos;
    float zarpEspPosY;
    float zarpEspMinX;
    float zarpEspMaxX;
    const float zarpEspMinY =124.31f;
    void DoZarpazoEspectral()
    {
        if (patronTimeline == 0)
        {
            zarpEspAttacks = 0;//CAMBIAR A 0
            zarpEspTime = 0;
            allZarpazosStopped = false;
            allZarpazosReturned = false;
            ZEState = ZarpEspState.calculating;

            zarpEspPosY = ceiling.bounds.min.y + ZarpEspInfo.zarpEspHeight / 2;
            zarpEspMinX = ceiling.bounds.min.x + ZarpEspInfo.zarpEspWidth / 2;
            zarpEspMaxX = ceiling.bounds.max.x - ZarpEspInfo.zarpEspWidth / 2;

            switch (hitsTaken)
            {
                case 0:
                    zarpEspAttacks = zarpEspMaxAttacks[0];
                    break;
                case 1:
                    zarpEspAttacks = zarpEspMaxAttacks[1];
                    break;
                case 2:
                    zarpEspAttacks = zarpEspMaxAttacks[2];
                    break;
            }
            zarpazos = new List<ZarpEspInfo>();
        }

        if (ZEState == ZarpEspState.calculating)//Warn por cada tentáculo
        {//calculate positions
            float posX;
            for (int i = 0; i < zarpEspAttacks; i++)
            {
                Vector2 randomPos;
                if (i == 0)
                {
                    randomPos = new Vector2(PlayerMovement.instance.transform.position.x, zarpEspPosY);
                }
                else
                {
                    float newMaxX = zarpEspMaxX - (ZarpEspInfo.zarpEspWidth * 2 * zarpazos.Count);
                    posX = Random.Range(zarpEspMinX, newMaxX);
                    for (int j = 0; j < zarpazos.Count; j++)
                    {
                        if (posX > zarpazos[j].minX && posX < zarpazos[j].maxX)
                        {
                            posX += ZarpEspInfo.zarpEspWidth;
                        }
                    }
                    posX = Mathf.Clamp(posX, zarpEspMinX, zarpEspMaxX);
                    randomPos = new Vector2(posX, zarpEspPosY);
                }
                //Debug.Log("Add zarpazo");
                zarpazos.Add(new ZarpEspInfo(randomPos, randomPos));
            }
            for (int i = 0; i < zarpazos.Count; i++)
            {
                GameObject light = Instantiate(warningLight, zarpazos[i].zarpEspWarningPosition, Quaternion.identity, warningLights);
                //Debug.Log("Add zarpazo light object= " + light);
                zarpazos[i].warningLight = light;
                //zarpazos[i] = new ZarpEspInfo(zarpazos[i].zarpEspPosition, zarpazos[i].zarpEspWarningPosition,null,light);
                //Debug.Log("zarpazos[i].warningLight= " + zarpazos[i].warningLight);
            }
            ZEState = ZarpEspState.warning;
            zarpEspTime = 0;
        }
        else if (ZEState == ZarpEspState.warning)//show red light and update pos
        {
            if (zarpEspTime < zarpEspWarnMaxTime)
            {
                float cameraLimitY = CameraMovement.instance.transform.position.y + CameraMovement.instance.GetComponent<Camera>().orthographicSize;
                for (int i = 0; i < zarpazos.Count; i++)
                {
                    zarpazos[i].zarpEspWarningPosition = new Vector2(zarpazos[i].zarpEspWarningPosition.x, cameraLimitY);
                    //Debug.Log("move zarpazo light on index =" + i);
                    //Debug.Log("zarpazos[" + i + "].warningLight.transform.position" + zarpazos[i].warningLight.transform.position);
                    zarpazos[i].warningLight.transform.position = zarpazos[i].zarpEspWarningPosition;
                }
            }
            else
            {
                zarpEspTime = 0;
                ZEState = ZarpEspState.start;
            }
        }
        else if (ZEState == ZarpEspState.start)//attack!
        {
            for (int i = 0; i < zarpazos.Count; i++)
            {
                Destroy(zarpazos[i].warningLight);
                zarpazos[i].warningLight = null;

                GameObject zarpEsp = Instantiate(zarpEspPrefab, zarpazos[i].zarpEspPosition, Quaternion.Euler(0, 0, 90), zarpazosEspectrales);
                //zarpazos[i] = new ZarpEspInfo(zarpazos[i].zarpEspPosition, zarpazos[i].zarpEspWarningPosition, zarpEsp, null);
                zarpazos[i].ownObj = zarpEsp;
                zarpEsp.GetComponent<Keeper_ZarpazoEspectral>().KonoStart(zarpEspAttackSpeed, zarpazos[i].zarpEspPosition, zarpEspMinY);
            }
            ZEState = ZarpEspState.going;
        }
        else if (ZEState == ZarpEspState.going)
        {
            allZarpazosStopped = true;
            for (int i = 0; i < zarpazos.Count; i++)
            {
                if (!zarpazos[i].ownObj.GetComponent<Keeper_ZarpazoEspectral>().stopped)
                {
                    allZarpazosStopped = false;
                    break;
                }
                if (allZarpazosStopped)
                {
                    zarpEspTime = 0;
                    ZEState = ZarpEspState.attacking;
                }
            }
        }
        else if (ZEState == ZarpEspState.attacking && zarpEspTime >= zarpEspAttackingMaxTime)
        {
            for (int i = 0; i < zarpazos.Count; i++)
            {
                zarpazos[i].ownObj.GetComponent<Keeper_ZarpazoEspectral>().ReturnZarpazo();
            }
            ZEState = ZarpEspState.returning;
        }
        else if (ZEState == ZarpEspState.returning)
        {
            allZarpazosReturned = true;
            for (int i = 0; i < zarpazos.Count; i++)
            {
                if (!zarpazos[i].ownObj.GetComponent<Keeper_ZarpazoEspectral>().returned)
                {
                    allZarpazosReturned = false;
                    break;
                }
            }
            if (allZarpazosReturned)
            {
                ZEState = ZarpEspState.returned;
            }
        }
        zarpEspTime += Time.deltaTime;
    }

    void DoAcidExalation()
    {
        if (patronTimeline < acidExalMaxTime)
        {
            if (patronTimeline == 0)
            {
                acidExalTime = 0;
                tongue.enabled = true;
            }
            if (acidExalTime >= acidExalTimeBetweenAttacks)//Attack
            {
                acidExalTime = 0;
                float randomAngle = Random.Range(0, acidExalAngle);
                Vector2 randomDir = GameController.GetVectorGivenAngleAndVector(Vector2.right, randomAngle);
                GameObject acidDrop = Instantiate(acidExalPrefab, acidExalOrigin.position, Quaternion.identity, acidDrops);
                float angle = Vector2.Angle(Vector2.down,randomDir);
                acidDrop.transform.rotation = Quaternion.Euler(0,0,angle);
                acidDrop.GetComponent<Keeper_AcidDrop>().KonoStart();
                acidDrop.GetComponent<Rigidbody2D>().velocity = randomDir * acidExalAttacksSpeed;
            }
            acidExalTime += Time.deltaTime;
        }
    }

    public void TakeHit()
    {
        if (hitsTaken < 3)
        {
            bossWaitTime = 0;
            bossWait = true;
            poseSet = false;
            patronIndex = 0;
            Debug.Log("hitsTaken=" + hitsTaken);
        }
        hitsTaken++;
        switch (hitsTaken)
        {
            case 0:
                KP2_actPatron = KP2_patron1;
                break;
            case 1:
                KP2_actPatron = KP2_patron2;
                break;
            case 2:
                KP2_actPatron = KP2_patron3;
                break;
            case 3:
                //end fight
                break;
        }

    }

}
