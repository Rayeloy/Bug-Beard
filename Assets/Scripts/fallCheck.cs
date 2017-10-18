using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallCheck : MonoBehaviour
{

    public bool soyDcha;
    public MushroomAI mAI;
    private int stillInside;

    private void Awake()
    {
        stillInside = 0;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "ground")
        {
            stillInside -= 1;
            if (stillInside == 0)
            {
                if (soyDcha)
                {
                    mAI.mState = MushroomAI.mushroomState.wLeft;
                }
                else
                {
                    mAI.mState = MushroomAI.mushroomState.wRight;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "ground")
        {
            stillInside += 1;
        }
    }
}
