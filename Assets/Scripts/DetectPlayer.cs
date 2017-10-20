using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour {

    public EnemyAI enemyAI;
    // Use this for initialization
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            enemyAI.StartPursue(col.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {

        if (col.tag == "Player")
        {
            enemyAI.StopPursue(col.gameObject);
        }
    }
}
