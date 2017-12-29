using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitBox : MonoBehaviour
{

    protected List<GameObject> tags;


    private void Awake()
    {
        tags = new List<GameObject>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        tags.Add(col.gameObject);
        if(tag=="PlayerAttack" && PlayerSlash.instance.slashSt==PlayerSlash.SlashState.slashing)//COLLISION DE HITBOX DE ATAQUE DEL JUGADOR Y HACIENDO SLASH
        {
            if (col.tag == "hitBox")
            {
                PlayerSlash.instance.StopSlash();
                //Debug.Log("enemy " + transform.root.name + " recieves damage");
                if (col.transform.GetComponentInParent<EnemyHP>().gameObject.name.Contains("Ghost"))//menor bounce con fantasmas
                {
                    PlayerMovement.instance.BounceBack(col.transform.position, PlayerMovement.instance.bounceForce / 1.5f);
                }
                else
                {
                    PlayerMovement.instance.BounceBack(col.transform.position);
                }

                (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
                (col.transform.GetComponentInParent(typeof(EnemyHP)) as EnemyHP).TakeDamage(PlayerSlash.instance.slashDamage);
            }
            else if (col.tag == "enemy")
            {
                PlayerSlash.instance.StopSlash();
                PlayerMovement.instance.BounceBack(col.transform.position);
                //PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
                (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
            }
            else if (col.gameObject.tag == "crystal")
            {
                PlayerSlash.instance.EnterCrystal(col.gameObject);
            }
            else if (col.tag == "destructible")
            {
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerSlash.instance.ResetSlash();
                col.gameObject.GetComponent<Destructible>().TakeDamage(PlayerSlash.instance.slashDamage);
            }
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D col)
    {
        if (tag == "PlayerAttack" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
        {
            if (col.gameObject.tag == "crystal" && PlayerSlash.instance.slashSt!=PlayerSlash.SlashState.crystal && PlayerSlash.instance.atCrystal.attachReady)
            {
                PlayerMovement.instance.attachToCrystal(col.gameObject);
                PlayerSlash.instance.atCrystal = new PlayerSlash.AttachedCrystal(col.gameObject);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (tags.Contains(col.gameObject))
        {
            tags.Remove(col.gameObject);
            PlayerSlash.instance.ExitCrystal(col.gameObject);
        }
    }
    public bool CheckFor(string _tag)
    {
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i].tag == _tag)
            {
                return true;
            }
        }
        return false;
    }
}
