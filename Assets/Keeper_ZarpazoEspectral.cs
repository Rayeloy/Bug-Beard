using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Keeper_ZarpazoEspectral : MonoBehaviour {
    public Rigidbody2D myRB;
    public float damage;
    [HideInInspector]
    public bool started;
    Vector2 origPos;
    float minPosY;
    bool returnZarpazo;
    float speed;
    [HideInInspector]
    public bool stopped;
    [HideInInspector]
    public bool returned;

    private void Awake()
    {
        started = false;
        returnZarpazo = false;
        stopped = false;
        returned = false;
    }

    public void KonoStart(float _speed,Vector2 _origPos,float _minPosY)
    {
        started = true;
        origPos = _origPos;
        minPosY = _minPosY;
        speed = _speed;
        myRB.velocity = Vector2.down * speed;
    }

    private void Update()
    {
        if (started)
        {
            if(transform.position.y<= minPosY)
            {
                StopZarpazo();
            }
            if (returnZarpazo)
            {
                if (transform.position.y >= origPos.y)
                {
                    StopZarpazo();
                    returned = true;
                    Destroy(gameObject);
                }
            }
        }
    }

    void StopZarpazo()
    {
        myRB.velocity = Vector2.zero;
        stopped = true;
    }

    public void ReturnZarpazo()
    {
        returnZarpazo = true;
        myRB.velocity = Vector2.up * speed;
    }


}
