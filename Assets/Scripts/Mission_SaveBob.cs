﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_SaveBob : Mission {
    public Mission_SaveBob(string _missionName,MissionType _mType) : base(_missionName,_mType)
    {
        missionName = _missionName;
        mType = _mType;
    }

    public GameObject breakableWall;
    public EnemyAI golem;

    public override void completeObjective(Objective _objective)
    {
        base.completeObjective(_objective);
        switch (currentObjective.ObjectiveName)
        {
            case "Derrota al golem":
                Debug.Log("START GOLEM CINEMATIC");
                //ToDo
                //camera hotSpot
                List<Transform> tList = new List<Transform>();
                tList.Add(PlayerMovement.instance.gameObject.transform);
                tList.Add(golem.gameObject.transform);
                hotSpotData myHS=new hotSpotData(hotSpotData.HotSpotMode.listCentre,tList,true,0.5f);
                CameraMovement.instance.setHotSpot(myHS);
                //animacion break
                //shakeCamera
                CameraMovement.instance.StartShakeCamera(3f, 0.5f, 0.08f, true);
                timerGolemCinematic = 0;
                maxTimerGolemCinematic = 3f;
                golemCinemStarted = true;
                break;
        }
    }
    float maxTimerGolemCinematic=3f;
    float timerGolemCinematic = 0;
    bool golemCinemStarted = false;
    bool wallDestroyed = false;
    public override void konoUpdate()
    {
        base.konoUpdate();
        //Debug.Log("mission update: timerGolemCinematic="+ timerGolemCinematic+"; golemCinemStarted= "+ golemCinemStarted);
        if (golemCinemStarted)
        {
            timerGolemCinematic += Time.deltaTime;
            if ((timerGolemCinematic >= maxTimerGolemCinematic / 4) && !wallDestroyed)
            {
                //destroy wall
                Destroy(breakableWall);
                wallDestroyed = true;
            }
            //Debug.Log("GOLEM CINEMATIC PROGRESS: timerGolemCinematic=" + timerGolemCinematic);
            if (timerGolemCinematic >= maxTimerGolemCinematic / 2)
            {
                //activateHeavyEnemy
                golem.stopEnemy = false;
            }
           // Debug.Log("maxTimerGolemCinematic= " + maxTimerGolemCinematic);
            if (timerGolemCinematic >= maxTimerGolemCinematic)
            {
              //  Debug.Log("STOP GOLEM CINEMATIC");
                CameraMovement.instance.stopHotSpot();
                golemCinemStarted = false;
            }
        }
    }

}
