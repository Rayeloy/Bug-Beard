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
    [HideInInspector]
    public bool Inmune;
    public SpriteRenderer spriteRend;


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
        if (Inmune && InmTime < MaxInmunityTime )
        {
            InmTime += Time.deltaTime;
            InmuneAnim();
            if (InmTime >= MaxInmunityTime)
            {
                Inmune = false;
                Color aux = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 1);
                spriteRend.color = aux;
                //gameObject.layer = 8;
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
            PlayerAnimations.instance.StartDamaged();
        }
    }

    void Inmunidad()
    {
        Inmune = true;
        //animacion inmune por t
        InmTime = 0;
        inmAnimTime = 0;
        opaque = true;
        //gameObject.layer = 10;//Inmune
    }
    float InmuneAnimProg = 0;

    float inmAnimMaxTranspTime=0.1f;
    float inmAnimMaxOpaqueTime = 0.2f;
    float inmAnimTime;
    bool opaque = true;
    void InmuneAnim()
    {
        if(opaque && inmAnimTime >= inmAnimMaxOpaqueTime)
        {
            Color aux = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 0);
            spriteRend.color = aux;
            inmAnimTime = 0;
            opaque = false;
        }
        else if(!opaque && inmAnimTime >= inmAnimMaxTranspTime)
        {
            Color aux = new Color(spriteRend.color.r, spriteRend.color.g, spriteRend.color.b, 1);
            spriteRend.color = aux;
            inmAnimTime = 0;
            opaque = true;
        }

        inmAnimTime += Time.deltaTime;

    }
}
