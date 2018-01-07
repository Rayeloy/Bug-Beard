﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController instance;
    public bool CheckPointOrder;
    public bool CheatsOn;

    public static List<EnemyHP> enemyList;
    public CheckPoint lastCheckPoint;
    private List<CheckPoint> checkPoints;

    public List<Mission> missionList;
    //[HideInInspector]
    public Mission currentMission;

    private void Awake()
    {
        Application.targetFrameRate = 300;
        DontDestroyOnLoad(this);
        instance = this;
        if (enemyList == null)
        {
            enemyList = new List<EnemyHP>();
        }
        checkPoints = new List<CheckPoint>();
        checkPoints.Add(lastCheckPoint);//añadimos el primer checkPoint (la salida)
        if (missionList.Count > 0)
            currentMission = missionList[0];
        else currentMission = null;
    }
    // Update is called once per frame


    private void Start()
    {
        if(currentMission!=null)
        currentMission.konoStart();
        setupHUD();
    }

    void Update()
    {
        Cheats();
        if(currentMission!=null)
        currentMission.konoUpdate();

    }

    public void completeMission(Mission _mission=null)
    {
        if (_mission == null)
        {
            for (int i = 0; i < missionList.Count; i++)
            {
                if (missionList[i].missionCompleted)
                {
                    continue;
                }
                else
                {
                    currentMission = missionList[i];
                    break;
                }
            }
        }
        else
        {
            missionList.Remove(_mission);
        }
        
        HUDManager.instance.cleanMission();
        HUDManager.instance.setupMissionHUD(currentMission);
    }

    void setupHUD()
    {
        if (currentMission != null)
        {
            HUDManager.instance.setupMissionHUD(currentMission);
        }
        HUDManager.instance.updateHUDHP();

        //SlashCDBar.maxValue = PlayerSlash.instance.cdTime;
        //SlashCDBar.interactable = false;
    }

    /*public float getSlashCD()
    {
        return PlayerSlash.instance.cd;
    }*/
    public void Respawn(GameObject player)
    {
        if (!CheckPointOrder)
            player.transform.position = lastCheckPoint.spawnPos.position;
        else
        {
            //Debug.Log("checkPoints.Count== " + checkPoints.Count);
            if (checkPoints.Count == 1)
            {
                player.transform.position = checkPoints[0].spawnPos.position;
            }
            else if (checkPoints.Count > 1)
            {
                player.transform.position = lastCheckPoint.spawnPos.position;
            }
        }
        PlayerHP.instance.HitPoints = PlayerHP.instance.MaxHitPoints;
        HUDManager.instance.updateHUDHP();
        PlayerSlash.instance.cd = 2;
    }
    public void GameOver(GameObject player)
    {
        Respawn(player);
    }

    public void AddCheckPoint(CheckPoint newCP)
    {
        bool valido = false;
        for (int i = 0; i < checkPoints.Count; i++)
        {
            if (newCP == checkPoints[i])
                return;
            if (newCP.order >= checkPoints[i].order)
            {
                valido = true;
                if (newCP.order > checkPoints[i].order)
                {
                    checkPoints.Remove(checkPoints[i]);
                }
            }
            
        }
        if (valido && CheckPointOrder)
        {
            checkPoints.Add(newCP);
            lastCheckPoint = newCP;
        }
        else if(!CheckPointOrder)
        {
            lastCheckPoint = newCP;
        }
        for (int i = 0; i < checkPoints.Count; i++)
        {
            //Debug.Log("checkPoint " + i + "= " + checkPoints[i].ToString());
        }
    }

    void Cheats()
    {
        if (CheatsOn)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                if (Time.timeScale == 0.25f)
                    Time.timeScale = 1;
                else
                    Time.timeScale = 0.25f;
            }
        }
    }
    //FUNCIONES AUXILIARES
    public static GameObject GetChild(GameObject padre, string childName)//Busca recursivamente un hijo con nombre childName
    {
        GameObject findResult;
        for (int i = 0; i < padre.transform.childCount; i++)
        {
            if (padre.transform.GetChild(i).name == childName)
            {
                return padre.transform.GetChild(i).gameObject;
            }
            findResult = GetChild(padre.transform.GetChild(i).gameObject, childName);
            if (findResult != null)//Recursivo
                return findResult;
        }

        return null;
    }
    public static Vector2 GetVectorGivenAngleAndVector(Vector2 dirOriginal, float angulo)
    {
        Vector2 result = new Vector2();
        dirOriginal = dirOriginal.normalized;
        float cos = Mathf.Cos(angulo * Mathf.Deg2Rad);
        Debug.Log("cos=" + cos);
        /*result.x = dirOriginal.x / cos;
        result.y = dirOriginal.y / cos;*/
        result.x = dirOriginal.x * Mathf.Cos(angulo) - dirOriginal.y * Mathf.Sin(angulo);
        result.y = dirOriginal.x * Mathf.Sin(angulo) + dirOriginal.y * Mathf.Cos(angulo);
        return result;
    }
    public static Vector2 GetVectorGivenAngleAndPoint(Vector2 pointOriginal, float angulo)
    {
        Vector2 result = new Vector2();
        Vector2 newPoint = new Vector2();
        angulo = angulo * Mathf.Deg2Rad;
        newPoint.x = pointOriginal.x+ Mathf.Cos(angulo);
        newPoint.y = pointOriginal.y + Mathf.Sin(angulo);
        result.x = newPoint.x - pointOriginal.x;
        result.y = newPoint.y - pointOriginal.y;
        return result;
    }
    /*public static GameObject GetFatherWithComponent(GameObject child, Component fatherComponent)//Busca recursivamente un hijo con nombre childName
    {
        GameObject findResult;
        Component comp = child.GetComponent<fatherComponent>();
        for (int i = 0; i < padre.transform.childCount; i++)
        {
            if (padre.transform.GetChild(i).name == childName)
            {
                return padre.transform.GetChild(i).gameObject;
            }
            findResult = GetChild(padre.transform.GetChild(i).gameObject, childName);
            if (findResult != null)//Recursivo
                return findResult;
        }

        return null;
    }*/
}
