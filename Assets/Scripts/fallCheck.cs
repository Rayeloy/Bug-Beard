using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallCheck : MonoBehaviour
{
    
    public bool soyDcha;
    public EnemyAI eAI;
    private int stillInside;

    private void Awake()
    {
        stillInside = 0;
    }


    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "ground" || col.gameObject.tag == "platform")
        {
            stillInside -= 1;
            if (stillInside == 0)
            {
                if (soyDcha)
                {
                    eAI.eState = EnemyAI.enemyState.wLeft;
                    eAI.currentDirection = EnemyAI.enemyState.wLeft;
                }
                else
                {
                    eAI.eState = EnemyAI.enemyState.wRight;
                    eAI.currentDirection = EnemyAI.enemyState.wRight;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ground" || col.gameObject.tag == "platform")
        {
            stillInside += 1;
        }
    }
}
