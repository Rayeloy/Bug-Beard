using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

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
