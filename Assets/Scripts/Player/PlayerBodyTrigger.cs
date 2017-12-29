using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyTrigger : MonoBehaviour {

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "enemy" && !PlayerHP.instance.Inmune)
        {
            if (!PlayerMovement.instance.bouncing && PlayerSlash.instance.slashSt != PlayerSlash.SlashState.slashing)//recibir daño
            {
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerHP.instance.TakeDamage(col.transform.GetComponentInParent<EnemyHP>().damage);            
            }
        }
    }
}
