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
        if(tag=="PlayerAttack" && PlayerSlash.instance.slashSt==PlayerSlash.SlashState.slashing)
        {
            if (col.tag == "hitBox")
            {
                Debug.Log("enemy " + transform.root.name + " recieves damage");
                PlayerSlash.instance.slashSt = PlayerSlash.SlashState.ready;
                PlayerMovement.instance.BounceBack();
                col.transform.root.GetComponent<EnemyHP>().TakeDamage(PlayerSlash.instance.slashDamage);
            }
            else if (col.gameObject.tag == "crystal" && PlayerSlash.instance.slashSt == PlayerSlash.SlashState.slashing)
            {
                PlayerMovement.instance.attachToCrystal(col.gameObject);
            }
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
