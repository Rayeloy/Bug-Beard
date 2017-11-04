using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {
    public int order;
    public Transform spawnPos;
    [HideInInspector]
    public CheckPoint thisCP;
    private void Awake()
    {
        thisCP = this;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            GameController.instance.AddCheckPoint(this);
        }
    }
}
