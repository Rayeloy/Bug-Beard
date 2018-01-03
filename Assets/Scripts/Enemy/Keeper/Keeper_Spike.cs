using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keeper_Spike : MonoBehaviour {
    bool started;
    bool destroying;
    float timeToDestroy;
    float maxTimeToDestroy;
    public Sprite[] sprites;
    public SpriteRenderer spritedRend;
    public float damage;

    public void konoStart()
    {
        int randomSprite = Random.Range(0, sprites.Length);//max exclusive!
        spritedRend.sprite = sprites[randomSprite];
        Vector2 S = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
        gameObject.GetComponent<BoxCollider2D>().size = S;
        //gameObject.GetComponent<BoxCollider2D>().offset = new Vector2((S.x / 2), (S.y/2));

        destroying = false;
        maxTimeToDestroy = 0.5f;
        timeToDestroy = 0;

        started = true;

    }
    private void Update()
    {
        if (started)
        {
            if (destroying)
            {
                GetComponent<Collider2D>().enabled = false;
                gameObject.isStatic = true;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                timeToDestroy += Time.deltaTime;
                if (timeToDestroy >= maxTimeToDestroy)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "ground"){
            //animacion destroy spike
            destroying = true;
        }
    }

}
