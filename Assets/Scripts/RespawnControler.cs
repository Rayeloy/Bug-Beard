using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnControler : MonoBehaviour {
    public static RespawnControler instance;
    //Fathers
    public Transform enemiesFather;
    public Transform destructFather;
    public Transform eventsFather;
    //Enemy prefabs
    public GameObject demoPerroPref;
    public GameObject ghostEnemyPref;
    public GameObject heavyEnemyPref;
    //Destructible prefabs
    public GameObject doorPrefab;


    public List<RespawnObject> respObjects;
    public List<RespawnEnemy> reposEnemies;

    private void Awake()
    {
        instance = this;
    }
    public void konoStart()
    {
        respObjects = new List<RespawnObject>();
        reposEnemies = new List<RespawnEnemy>();
    }

    //---------------------------Reposition Enemies----------------------------
    public void AddReposEnemy(RespawnEnemy enemy)
    {
        reposEnemies.Add(enemy);
        Debug.Log("Enemy "+enemy.Name+" moved");
    }

    public void RepositionEnemies()
    {
        for (int i = 0; i < reposEnemies.Count; i++)
        {
            reposEnemies[i].thisObject.transform.position = reposEnemies[i].Position;
            Debug.Log("Enemy " + reposEnemies[i].thisObject + " is moved to pos " + reposEnemies[i].Position);
            reposEnemies[i].thisObject.transform.localRotation = Quaternion.Euler(reposEnemies[i].Rotation);
            reposEnemies[i].thisObject.GetComponent<EnemyAI>().stopEnemy = reposEnemies[i].StopEnemy;
            reposEnemies[i].thisObject.GetComponent<EnemyAI>().moved = false;
        }
        ClearRepositionEnemies();
    }
    public void ClearRepositionEnemies()
    {
        for (int i = 0; i < reposEnemies.Count; i++)
        {
            reposEnemies[i].thisObject.GetComponent<EnemyAI>().moved = false;
        }
            reposEnemies.Clear();
    }
    //---------------------------Respawn All Objects----------------------------
    public void AddObject(RespawnObject _object)
    {
        respObjects.Add(_object);
    }
    public void AddEnemy(EnemyAI enemy)
    {
        respObjects.Add(enemy.myRespEnemy);
    }
    public void RemoveEnemy(EnemyAI enemy)
    {
        respObjects.Remove(enemy.myRespEnemy);
    }
    public void RespawnAll()
    {
        for(int i = 0; i < respObjects.Count; i++)
        {
            GameObject thePrefab = null;

            switch (respObjects[i].type)
            {
                case RespawnObject.Type.Enemy:
                    switch ((respObjects[i] as RespawnEnemy).EnemType)
                    {
                        case RespawnEnemy.EnemyType.Demoperro:
                            thePrefab = demoPerroPref;
                            break;
                        case RespawnEnemy.EnemyType.GhostEnemy:
                            thePrefab = ghostEnemyPref;
                            break;
                        case RespawnEnemy.EnemyType.HeavyEnemy:
                            thePrefab = heavyEnemyPref;
                            break;
                    }
                    GameObject enemy=Instantiate(thePrefab, respObjects[i].Position,Quaternion.identity,enemiesFather);
                    enemy.transform.localRotation = Quaternion.Euler((respObjects[i] as RespawnEnemy).Rotation);
                    enemy.GetComponent<EnemyAI>().stopEnemy = (respObjects[i] as RespawnEnemy).StopEnemy;
                    enemy.GetComponent<EnemyAI>().AssingRespObject();
                    break;

                case RespawnObject.Type.Destructible:
                    switch ((respObjects[i] as RespawnDestructible).DestType)
                    {
                        case RespawnDestructible.DestructibleType.Door:
                            thePrefab = doorPrefab;
                            break;
                    }
                    GameObject destructible = Instantiate(thePrefab, respObjects[i].Position, Quaternion.identity, destructFather);
                    destructible.transform.localScale = (respObjects[i] as RespawnDestructible).Proportions;
                    destructible.GetComponent<Destructible>().AssingRespObject();
                    break;

                case RespawnObject.Type.Event:
                    break;
            }
            thePrefab.name = respObjects[i].Name;
        }

        ClearRespawnObjects();
    }

    public void ClearRespawnObjects()
    {
        respObjects.Clear();
    }
}
