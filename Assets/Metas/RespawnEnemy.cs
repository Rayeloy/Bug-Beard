using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEnemy : RespawnObject {
    public Vector3 Rotation;
    public bool StopEnemy;
    public EnemyType EnemType;
    public enum EnemyType
    {
        Demoperro,
        GhostEnemy,
        HeavyEnemy
    }

    public RespawnEnemy(Vector2 _position, Vector3 _rotation, string _name, bool _stopEnemy)
    {
        type = Type.Enemy;
        Position = _position;
        Rotation = _rotation;
        Name = _name;
        if (Name.Contains("demoperro"))
        {
            EnemType = EnemyType.Demoperro;
        }
        else if(Name.Contains("Ghost_enemy") && !Name.Contains("v2"))
        {
            EnemType = EnemyType.GhostEnemy;
        }
        else if (Name.Contains("Heavy_enemy"))
        {
            EnemType = EnemyType.HeavyEnemy;
        }
        StopEnemy = _stopEnemy;
    }

    public void PrintAll()
    {
        Debug.Log("Type: " + type + "; Position: " + Position + "; Rotation: " + Rotation + "; EnemType: " + EnemType + "; StopEnemy: " + StopEnemy);
    }

}
