using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {
    [HideInInspector]
    public RespawnDestructible myRespDestructible;

    private void Awake()
    {
        AssingRespObject();
        myRespDestructible.PrintAll();
    }
    public void AssingRespObject()
    {
        myRespDestructible = new RespawnDestructible(transform.position, transform.localRotation.eulerAngles, name, transform.localScale);
    }
    public float Health;
    public void TakeDamage(float damage)
    {
        Health = Health - damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
