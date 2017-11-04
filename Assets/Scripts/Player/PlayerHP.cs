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
            GameController.instance.updateHUDHP();
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
            if (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
            {
                PlayerSlash.instance.StopSlash();
                PlayerMovement.instance.BounceBack();
            }
            else//recibir daño
            {
                //PlayerMovement.instance.BounceBack();
                GameObject hitBox = GameController.instance.GetChild(col.gameObject, "weakBox");
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
}
