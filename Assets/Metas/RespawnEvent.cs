using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEvent : RespawnObject
{
    public Event myEvent;
    public RespawnEvent(Vector2 _position, string _name, Event _myEvent, GameObject _thisObject = null)
    {
        type = Type.Event;
        Position = _position;
        Name = _name;
        myEvent = _myEvent;
        thisObject = _thisObject;

    }

    public void PrintAll()
    {
        Debug.Log("Type: " + type + "; Position: " + Position + "; Name: " + Name + "; thisObject: " + thisObject);
    }

}