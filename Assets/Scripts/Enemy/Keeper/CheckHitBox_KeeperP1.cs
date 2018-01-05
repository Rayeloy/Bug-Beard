using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitBox_KeeperP1 : CheckHitBox {
    public Keeper_Phase1 KeeperAI;
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        tags.Add(col.gameObject);
        if (tag == "PlayerAttack" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
        {
            if (col.tag == "hitBox")
            {
                Debug.Log("enemy " + transform.root.name + " recieves damage");
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
                if (KeeperAI.vulnerable)
                {
                    //hit en rodilla
                    KeeperAI.BecomeVulnerable();
                }
                else
                {
                    col.transform.root.GetComponent<EnemyHP>().TakeDamage(PlayerSlash.instance.slashDamage);
                    col.transform.root.GetComponent<EnemyAI>().BounceBack(PlayerMovement.instance.transform.position);
                }

            }
            else if (col.gameObject.tag == "crystal" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
            {
                PlayerMovement.instance.attachToCrystal(col.gameObject);
            }
        }
    }
}
