using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyTrigger : MonoBehaviour {

    private void OnTriggerStay2D(Collider2D col)
    {
        if ((col.gameObject.tag == "enemy" ||  col.gameObject.tag=="enemy_projectile") && !PlayerHP.instance.Inmune)
        {
            if (!PlayerMovement.instance.bouncing && PlayerSlash.instance.slashSt != PlayerSlash.SlashState.slashing)//recibir daño
            {
                PlayerMovement.instance.BounceBack(col.transform.position);
                if(col.gameObject.tag == "enemy")
                {
                    PlayerHP.instance.TakeDamage(col.transform.GetComponentInParent<EnemyHP>().damage);
                }
                else if (col.gameObject.tag == "enemy_projectile")
                {
                    if (col.name.Contains("Keeper_Spike"))
                    {
                        PlayerHP.instance.TakeDamage(col.GetComponent<Keeper_Spike>().damage);
                    }
                }
            }
        }
    }
}
