using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission : MonoBehaviour {
    public string missionName;
    public MissionType mType;
    //[HideInInspector]
    public Objective currentObjective;
    public enum MissionType
    {
        main,
        secondary
    }
    public Mission(string _missionName, MissionType _mType)
    {
        missionName = _missionName;
        mType = _mType;
    }
    public List<Objective> objectives;
    //[HideInInspector]
    //public int ObjectiveAct;
    //[HideInInspector]
    public bool missionCompleted;
    public void konoStart()
    {
        //ObjectiveAct = 0;
        missionCompleted = false;
        currentObjective = objectives[0];
        foreach(Objective obj in objectives)
        {
            obj.konoStart();
        }
        currentObjective.active = true;
    }
    public virtual void completeObjective(Objective _objective)
    {
        currentObjective.completed = true;
        konoUpdate();
        if (!missionCompleted)
        {
            currentObjective.active = true;
        }
        HUDManager.instance.updateObjectiveCompletion();
    }
    public virtual void konoUpdate()
    {
        if (!missionCompleted)
        {
            bool unCompletedFound = false;
            for (int i = 0; i < objectives.Count; i++)
            {
                if (objectives[i].completed)
                {
                    continue;
                }
                else
                {
                    currentObjective = objectives[i];
                    //ObjectiveAct = i;
                    unCompletedFound = true;
                    break;
                }
            }
            if (!unCompletedFound)
            {
                missionCompleted = true;
                GameController.instance.completeMission();
            }
            else
            {
                currentObjective.konoUpdate();
            }
        }
    }

}

