using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour {

    public float HP;
    public float damage;
    public PolygonCollider2D hitBox;
    public static List<EnemyHP> enemyList;

    private void Awake()
    {
        if (enemyList == null)
        {
            enemyList = new List<EnemyHP>();
        }
        enemyList.Add(this);
    }

    // Update is called once per frame
    void Update () {
        if (HP <= 0)
        {
            Die();
        }
	}

    public void Die()
    {
        Destroy(transform.parent.gameObject);
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
    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("-----------------------------COLLISION HIT BOX con "+col.gameObject.tag);
        Debug.Log("PUTA " + col.gameObject.tag + ": " + (col.gameObject.tag == "Player") + " && " + (PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing));
        if (col.gameObject.tag=="Player" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
        {
            float dam = 0;
            switch (PlayerSlash.instance.slashSt)//SUPONIENDO QUE PONDREMOS MÁS ESTADOS PARA OTROS POWERS
            {
                case PlayerSlash.SlashState.slashing:
                    dam = PlayerSlash.instance.slashDamage;
                    break;
            }
            Debug.Log("--------------------------------------DAMAGE=" + dam);
           TakeDamage(dam);
        }
    }

}
