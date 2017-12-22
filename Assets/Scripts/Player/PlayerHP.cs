using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{

    public static PlayerHP instance;
    public float MaxHitPoints;
    [HideInInspector]
    public float HitPoints;
    public float MaxInmunityTime;
    private float InmTime;
    private bool Inmune;


    private void Awake()
    {
        InmTime = 0;
        Inmune = false;
        instance = this;
        HitPoints = MaxHitPoints;
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
            GameController.instance.GameOver(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!Inmune)
        {
            HitPoints -= damage;
            HUDManager.instance.updateHUDHP();
#if DEBUG_LOG
            Debug.Log("HP= " + HitPoints);
#endif 
            Inmunidad();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "enemy")
        {
            if(!PlayerMovement.instance.bouncing && PlayerSlash.instance.slashSt != PlayerSlash.SlashState.slashing)//recibir daño
            {
                TakeDamage(col.gameObject.GetComponent<EnemyHP>().damage);
                PlayerMovement.instance.BounceBack(col.transform.position);
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
}
