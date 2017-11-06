using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHitBox : MonoBehaviour
{

    private List<string> tags;

    private void Awake()
    {
        tags = new List<string>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        tags.Add(col.tag);
        if (tag == "PlayerAttack")
        {
            Debug.Log("colisión con " + col.name+"; slashSt= "+PlayerSlash.instance.slashSt.ToString());
        }
        if(tag=="PlayerAttack" && col.tag == "hitBox" && PlayerSlash.instance.slashSt==PlayerSlash.SlashState.slashing)
        {
            Debug.Log("enemy " + transform.root.name + " recieves damage");
            PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
            PlayerMovement.instance.BounceBack();
            col.transform.root.GetComponent<EnemyHP>().TakeDamage(PlayerSlash.instance.slashDamage);
        }

    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (tags.Contains(col.tag))
        {
            tags.Remove(col.tag);
        }
    }
    public bool CheckFor(string _tag)
    {
        for (int i = 0; i < tags.Count; i++)
        {
            if (tags[i] == _tag)
            {
                return true;
            }
        }
        return false;
    }
}
