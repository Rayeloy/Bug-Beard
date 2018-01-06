﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_Phase2 : EnemyAI {

    public static Keeper_Phase2 instance;

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
    KeeperP2[] KP2_actPatron;
    KeeperP2[] KP2_patron1 = { KeeperP2.rugido, KeeperP2.zarpazoEspectral, KeeperP2.AcidExalation };
    KeeperP2[] KP2_patron2 = { KeeperP2.rugido_ZarpazoEspectral, KeeperP2.RayoFatuo, KeeperP2.AcidExalation };
    KeeperP2[] KP2_patron3 = { KeeperP2.rugido_ZarpazoEspectral, KeeperP2.rugido_RayoFatuo, KeeperP2.rugido_AcidExalation };
    int patronIndex;
    bool nextSkill;

    [Header("Rugido")]
    public float rugidoMaxTime;
    bool rugidoCharging;
    public Transform rugidoPos;
    public float timeBetweenSpikes;
    float spikesTime;
    public float spikesSpeed;
    public GameObject spike;
    public Collider2D ceiling;
    public Transform spikes;
    float spikeWidth = 9.10791f;//width of the widest possible spike
    float spikeHeight = 12.91689f;

    [Header("Zarpazo Espectral")]


    [Header("Acid Exalation")]


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
        rugido_ZarpazoEspectral=4,
        rugido_RayoFatuo=5,
        rugido_AcidExalation=6
    }

    private void OnEnable()
    {
        KonoStart();
        //TRANSICIÓN
        ManageCurrentSkill();
    }

    public void KonoStart()
    {
        instance = this;
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
        luces[1].enabled = false;

        standBySpritePos = spritesOffsets[0];
        standBySpriteProp = spritesProportions[0];

        KP2_actPatron = KP2_patron1;
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
                    //PositionForSkill();
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
                            break;
                        case KeeperP2.AcidExalation:
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
        if (patronIndex >= KP2_actPatron.Length)
        {
            patronIndex = 0;
        }
        KP2 = KP2_actPatron[patronIndex];
        nextSkill = false;
        patronTimeline = 0;

        poseSet = false;
        moving = true;
        patronIndex++;

        //puedo poner Ifs para solo resetear lo siguiente en caso de que sea la siguiente skill
        spikesTime = 0;
        rugidoCharging = true;
        Debug.Log("Current skill= " + KP2);
    }

    bool poseSet = false;
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
            switch (KP2)
            {
                case KeeperP2.rugido:
                    break;
                case KeeperP2.zarpazoEspectral:
                    break;
                case KeeperP2.AcidExalation:
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
            poseSet = true;
        }
    }

    void DoRugido()
    {
        if (patronTimeline < rugidoMaxTime)
        {
            if (patronTimeline == 0)//primera vez
            {
                spikesTime = 0;
            }
            if (rugidoCharging && spikesTime >= 1.5f)
            {
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
