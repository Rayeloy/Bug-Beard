using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RespawnObject {

    public enum Type
    {
        Enemy,
        Destructible,
        Event
    }
    public Type type;
    public Vector2 Position;
    public string Name;

}
