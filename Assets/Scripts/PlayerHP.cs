﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{

    public static PlayerHP instance;
    public float HitPoints;
    public float MaxInmunityTime;
    private float InmTime;
    private bool Inmune;

    private void Awake()
    {
        InmTime = 0;
        Inmune = false;
        instance = this;
    }

    private void Update()
    {
        //INMUNE CONTROL
        if (InmTime < MaxInmunityTime && Inmune)
        {
            InmTime += Time.deltaTime;
            if (InmTime >= MaxInmunityTime)
            {
                Inmune = false;
                gameObject.layer = 8;
            }
        }
        //DEATH CONTROL
        if (HitPoints <= 0)
        {
            GameController.instance.GameOver();
        }
    }

    public void TakeDamage(float damage)
    {
        if (!Inmune)
        {
            HitPoints -= damage;
            GameController.instance.updateHUD();
            Debug.Log("HP= " + HitPoints);
            Inmunidad();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "enemy")
        {
            if (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
            {
                PlayerSlash.instance.StopSlash();
                BounceBack();
            }
            else
            {
                BounceBack();
                GameObject hitBox = GameController.instance.GetChild(col.gameObject, "hitBox");
                Debug.Log("-------------------------hitBox= " + hitBox);
                TakeDamage(hitBox.GetComponent<EnemyHP>().damage);
            }
        }
    }
    void Inmunidad()
    {
        Inmune = true;
        //animacion inmune por t
        InmTime = 0;
        gameObject.layer = 10;//Inmune
    }
    private void BounceBack()
    {
        //bounce
        PlayerMovement.instance.phase = PlayerMovement.jumpphase.fall;
    }
}
