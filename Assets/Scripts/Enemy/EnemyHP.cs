using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{

    public float HP;
    public float damage;
    public CheckHitBox[] hitBox;
    public GameObject wholeEnemy;


    private void Start()
    {
        GameController.enemyList.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        GameController.enemyList.Remove(this);
        Destroy(wholeEnemy);
    }
    public void TakeDamage(float damage)
    {
        switch (PlayerSlash.instance.slashSt)//SUPONIENDO QUE PONDREMOS MÁS ESTADOS PARA OTROS POWERS
        {
            case PlayerSlash.SlashState.slashing:
                PlayerSlash.instance.StopSlash();
                break;
        }
        HP -= damage;
    }
    /*public void CheckTakeDamage()
    {
        if (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
        {
            foreach (CheckHitBox HB in hitBox)
            {
                if (HB.CheckFor("AttackHitBox"))
                {
                    TakeDamage(PlayerSlash.instance.slashDamage);
                    break;//intento de que solo pueda hacerme daño una vez
                }
            }
        }
    }*/

    /*private void OnTriggerEnter2D(Collider2D col)
    {
#if DEBUG_LOG
        Debug.Log("-----------------------------COLLISION HIT BOX con "+col.gameObject.tag);
        Debug.Log("PUTA " + col.gameObject.tag + ": " + (col.gameObject.tag == "Player") + " && " + (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing));
#endif
        if (col.gameObject.tag == "Player" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
        {
            float dam = 0;
            switch (PlayerSlash.instance.slashSt)//SUPONIENDO QUE PONDREMOS MÁS ESTADOS PARA OTROS POWERS
            {
                case PlayerSlash.SlashState.slashing:
                    dam = PlayerSlash.instance.slashDamage;
                    PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
                    break;
            }
#if DEBUG_LOG
            Debug.Log("--------------------------------------DAMAGE=" + dam);
#endif
            TakeDamage(dam);
            PlayerMovement.instance.BounceBack();
        }
    }*/

}
