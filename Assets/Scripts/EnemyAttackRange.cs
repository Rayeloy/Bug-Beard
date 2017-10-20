using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackRange : MonoBehaviour {
    //public EnemyAI eAI;
    //public EnemyHP eHP;
    [HideInInspector]
    public bool thereIsPlayer;
    //public bool damaged;

    private void Awake()
    {
        thereIsPlayer = false;
    }
    /*private void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag=="Player" && eAI.AState==EnemyAI.AttackState.damaging && !damaged)
        {
            damaged = true;
            PlayerHP.instance.TakeDamage(eHP.damage);
        }
    }*/
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            thereIsPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            thereIsPlayer = false;
        }
    }
}
