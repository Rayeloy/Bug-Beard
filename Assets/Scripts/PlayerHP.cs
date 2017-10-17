using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHP : MonoBehaviour {

    public float HitPoints;
    public float MaxInmunityTime;
    private float InmTime;
    private bool Inmune;

    private void Awake()
    {
        InmTime = 0;
        Inmune = false;
    }

    private void Update()
    {
        if (InmTime < MaxInmunityTime && Inmune)
        {
            InmTime += Time.deltaTime;
            if (InmTime >= MaxInmunityTime)
            {
                Inmune = false;
                gameObject.layer = 8;
            }
        }
        
        if (HitPoints <= 0)
        {
            GameController.instance.GameOver();
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "enemy" && !Inmune)
        {
            HitPoints -= col.gameObject.GetComponent<EnemyHP>().damage;
            Debug.Log("HP= " + HitPoints);
            Inmunidad();
        }
    }
    void Inmunidad()
    {
        Inmune = true;
        //animacion inmune por t
        InmTime = 0;
        gameObject.layer = 10;//nmune
    }
}
