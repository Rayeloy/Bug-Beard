using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitBox : MonoBehaviour
{

    protected List<GameObject> tags;
    AttachedCrystal atCrystal;
    struct AttachedCrystal
    {
        public GameObject Crystal;
        public bool attachReady;
        public AttachedCrystal(GameObject _Crystal, bool _attachReady = false)
        {
            Crystal = _Crystal;
            attachReady = _attachReady;
        }
    }

    private void Awake()
    {
        atCrystal = new AttachedCrystal(null,true);
        tags = new List<GameObject>();

    }
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        tags.Add(col.gameObject);
        if(tag=="PlayerAttack" && PlayerSlash.instance.slashSt==PlayerSlash.SlashState.slashing)//COLLISION DE HITBOX DE ATAQUE DEL JUGADOR Y HACIENDO SLASH
        {
            if (col.tag == "hitBox")
            {
                Debug.Log("enemy " + transform.root.name + " recieves damage");
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
                (col.transform.GetComponentInParent(typeof(EnemyHP)) as EnemyHP).TakeDamage(PlayerSlash.instance.slashDamage);
                (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
            }
            else if (col.tag == "enemy")
            {
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
                (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
            }
            else if (col.gameObject.tag == "crystal")
            {
                PlayerMovement.instance.attachToCrystal(col.gameObject);
                atCrystal = new AttachedCrystal(col.gameObject);
            }
            else if (col.tag == "destructible")
            {
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
                col.gameObject.GetComponent<Destructible>().TakeDamage(PlayerSlash.instance.slashDamage);
            }
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D col)
    {
        if (tag == "PlayerAttack" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
        {
            if (col.gameObject.tag == "crystal" && PlayerSlash.instance.slashSt!=PlayerSlash.SlashState.crystal && atCrystal.attachReady)
            {
                PlayerMovement.instance.attachToCrystal(col.gameObject);
                atCrystal = new AttachedCrystal(col.gameObject);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (tags.Contains(col.gameObject))
        {
            tags.Remove(col.gameObject);
            if (col == atCrystal.Crystal)
            {
                atCrystal.Crystal = null;
                atCrystal.attachReady = true;
            }
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
