using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnDestructible : RespawnObject
{
    public Vector3 Proportions;
    public Vector3 Rotation;
    public DestructibleType DestType;
    public enum DestructibleType
    {
        Door
    }

    public RespawnDestructible(Vector2 _position, Vector3 _rotation, string _name, Vector3 _proportions,GameObject _thisObject=null)
    {
        type = Type.Destructible;
        Position = _position;
        Rotation = _rotation;
        Name = _name;
        Proportions = _proportions;
        thisObject = _thisObject;
        if (Name.Contains("Door"))
        {
            DestType = DestructibleType.Door;
        }

    }

    public void PrintAll()
    {
        Debug.Log("Type: " + type + "; Position: " + Position+"; Name: "+Name+"; Proportions: "+Proportions);
    }

}
