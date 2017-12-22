using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "PlayerBodyTrigger")
        {
            GameController.instance.GameOver(col.transform.root.gameObject);
        }
    }
}
