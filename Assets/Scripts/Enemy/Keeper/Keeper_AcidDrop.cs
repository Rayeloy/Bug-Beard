using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_AcidDrop : MonoBehaviour {
    bool started = false;
    public Sprite[] sprites;
    public SpriteRenderer spritedRend;
    public float damage;
    float dist;
    float maxDist;
    Vector2 myPos;

    private void Update()
    {
        if (started)
        {
            dist= new Vector2(transform.position.x - myPos.x, transform.position.y - myPos.y).magnitude;
            if (dist >= maxDist)
            {
                Destroy(gameObject);
            }
        }
    }

    public void KonoStart()
    {
        myPos = transform.position;
        started = true;
        int randomSprite = Random.Range(0, sprites.Length);//max exclusive!
        spritedRend.sprite = sprites[randomSprite];
        Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = S;
    }
}
