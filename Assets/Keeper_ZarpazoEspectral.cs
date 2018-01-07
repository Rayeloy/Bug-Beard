using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Keeper_ZarpazoEspectral : MonoBehaviour {
    public Rigidbody2D myRB;
    public float damage;
    [HideInInspector]
    public bool started = false;
    Vector2 origPos;
    float minPosY;
    bool returnZarpazo;
    float speed;
    [HideInInspector]
    public bool stopped;
    [HideInInspector]
    public bool returned;
    bool going = false;

    public void KonoStart(float _speed,Vector2 _origPos,float _minPosY)
    {
        started = true;
        returnZarpazo = false;
        stopped = false;
        returned = false;
        origPos = _origPos;
        minPosY = _minPosY+ ZarpEspInfo.zarpEspHeight / 2;
        speed = _speed;
        myRB.velocity = Vector2.down * speed;
        going = true;
    }

    private void Update()
    {
        if (started)
        {
            if (going)
            {
                myRB.velocity = Vector2.down * speed;
                if (transform.position.y <= minPosY)
                {
                    StopZarpazo();
                }
            }


            if (returnZarpazo)
            {
                myRB.velocity = Vector2.up * speed;
                if (transform.position.y >= origPos.y)
                {
                    myRB.velocity = Vector2.zero;
                    returned = true;
                }
            }
        }
    }

    void StopZarpazo()
    {
        Debug.Log("ZARPAZO STOPPED");
        myRB.velocity = Vector2.zero;
        going = false;
        stopped = true;
    }

    public void ReturnZarpazo()
    {
        returnZarpazo = true;
        myRB.velocity = Vector2.up * speed;
    }

}
