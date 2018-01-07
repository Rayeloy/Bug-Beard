﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyTrigger : MonoBehaviour {

    private void OnTriggerStay2D(Collider2D col)
    {
        if ((col.gameObject.tag == "enemy" ||  col.gameObject.tag=="enemy_projectile") && !PlayerHP.instance.Inmune)
        {
            if (!PlayerMovement.instance.bouncing && PlayerSlash.instance.slashSt != PlayerSlash.SlashState.slashing)//recibir daño
            {
                if(col.gameObject.tag == "enemy")
                {
                    PlayerMovement.instance.BounceBack(col.transform.position);
                    PlayerHP.instance.TakeDamage(col.transform.GetComponentInParent<EnemyHP>().damage);
                }
                else if (col.gameObject.tag == "enemy_projectile")
                {
                    if (col.name.Contains("Keeper_Spike"))
                    {
                        PlayerMovement.instance.BounceBack(col.transform.position);
                        PlayerHP.instance.TakeDamage(col.GetComponent<Keeper_Spike>().damage);
                    }//nightmareAttack solved inside keeper script
                    else if (col.name.Contains("Zarpazo"))
                    {
                        PlayerMovement.instance.BounceBack(col.transform.position);
                        PlayerHP.instance.TakeDamage(col.GetComponent<Keeper_ZarpazoEspectral>().damage);
                    }
                    else if (col.name.Contains("AcidDrop"))
                    {
                        PlayerMovement.instance.BounceBack(col.transform.position);
                        PlayerHP.instance.TakeDamage(col.GetComponent<Keeper_AcidDrop>().damage);
                    }
                    else if (col.name.Contains("rayo"))
                    {
                        PlayerMovement.instance.BounceBack(col.transform.position);
                        PlayerHP.instance.TakeDamage(Keeper_Phase1.instance.gameObject.GetComponent<EnemyHP>().damage*2);//50
                    }
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "crystal")
        {
            PlayerSlash.instance.ExitCrystal(col.gameObject);
        }
    }
}
