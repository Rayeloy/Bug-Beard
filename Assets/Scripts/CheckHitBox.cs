using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitBox : MonoBehaviour
{

    protected List<GameObject> tags;
    protected List<EnemyHB> enemiesInsidePHB;

    public struct EnemyHB
    {
        public GameObject enemy;
        public bool inside;
        public EnemyHB(GameObject _enemy, bool _inside = false)
        {
            enemy = _enemy;
            inside = _inside;
        }
    }

    private void Awake()
    {
        tags = new List<GameObject>();
        enemiesInsidePHB = new List<EnemyHB>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        tags.Add(col.gameObject);
        if (GetComponentInParent<CheckHitBox>().name == "NightmareAttack")
        {
            Debug.Log("tag added: " + col.tag);
        }

    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (tags.Contains(col.gameObject))
        {
            tags.Remove(col.gameObject);
            //Debug.Log("tag removed: " + col.tag);
            PlayerSlash.instance.ExitCrystal(col.gameObject);
            //Debug.Log("CheckFor('Player')="+CheckFor("Player"));
        }
        /*if (tag == "PlayerAttack")
        {
            for (int i = 0; i <= enemiesInsidePHB.Count; i++)
            {
                if (enemiesInsidePHB[i].enemy == col.gameObject)
                {
                    enemiesInsidePHB.RemoveAt(i);
                }
            }
        }*/
    }

    void ManagePlayerAttackCollisions(Collider2D col)
    {
        /*if (tag == "PlayerAttack")
        {
            for (int i = 0; i <= enemiesInsidePHB.Count; i++)
            {
                if (enemiesInsidePHB[i].enemy != col.gameObject)
                {
                    EnemyHB aux = new EnemyHB(col.gameObject, true);
                    enemiesInsidePHB.Add(aux);
                }
            }
        }*/
        if (tag == "PlayerAttack" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)//COLLISION DE HITBOX DE ATAQUE DEL JUGADOR Y HACIENDO SLASH
        {
            if (col.tag == "hitBox")
            {
                Debug.Log("PAttack agains " + col.name);
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
                PlayerSlash.instance.ResetSlash();
                if (col.gameObject.layer == 18)//Keeper
                {
                    if (Keeper_Phase1.instance.vulnerable)
                    {
                        Keeper_Phase1.instance.BecomeVulnerable();
                    }
                    else if (col.name.Contains("Espada"))
                    {
                        Keeper_Phase1.instance.TakeHit();
                    }
                }
                else
                {
                    (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
                    (col.transform.GetComponentInParent(typeof(EnemyHP)) as EnemyHP).TakeDamage(PlayerSlash.instance.slashDamage);
                }
            }
            else if (col.tag == "enemy")
            {
                Debug.Log("PAttack agains " + col.name);
                PlayerSlash.instance.StopSlash();
                PlayerMovement.instance.BounceBack(col.transform.position);
                (col.transform.GetComponentInParent(typeof(EnemyAI)) as EnemyAI).BounceBack(PlayerMovement.instance.transform.position);
            }
            else if (col.gameObject.tag == "crystal")
            {
                PlayerSlash.instance.EnterCrystal(col.gameObject);
            }
            else if (col.tag == "destructible")
            {
                PlayerSlash.instance.StopSlash();
                PlayerMovement.instance.BounceBack(col.transform.position);
                PlayerSlash.instance.ResetSlash();
                col.gameObject.GetComponent<Destructible>().TakeDamage(PlayerSlash.instance.slashDamage);
            }
        }
    }

    public bool CheckFor(string _tag)
    {
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i] == null)
            {
                tags.RemoveAt(i);
            }
            else if (tags[i].tag == _tag)
            {
                return true;
            }
        }
        return false;
    }
}
