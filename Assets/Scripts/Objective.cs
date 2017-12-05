using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    Mission myMission;

    public string ObjectiveName;
    public string description;

    public ObjectiveType oType;
    public List<EnemyAI> enemiesToKill;
    [HideInInspector]
    public int totalEnemies, enemiesKilled;
    //[HideInInspector]
    public bool interacted, reached, completed, active;

    public void konoStart()
    {
        totalEnemies = enemiesToKill.Count;
        enemiesKilled = 0;
        myMission = transform.parent.gameObject.GetComponent<Mission>();
        active = false;
        interacted = false;
        reached = false;
        completed = false;
    }
    public enum ObjectiveType
    {
        interact = 0,
        reach = 1,
        kill = 2
    }
    public Objective(string _ObjectiveName, string _description, bool completed = false, ObjectiveType oType = 0)
    {
        ObjectiveName = _ObjectiveName;
        description = _description;

    }
    public void konoUpdate()
    {
        switch (oType)
        {
            case ObjectiveType.interact:
                if (interacted)
                {
                    myMission.completeObjective(this);
                }
                break;
            case ObjectiveType.reach:
                if (reached)
                {
                    myMission.completeObjective(this);     
                }
                break;
            case ObjectiveType.kill:
                for (int i = 0; i < enemiesToKill.Count; i++)
                {
                    if (enemiesToKill[i] == null)
                    {
                        enemiesToKill.RemoveAt(i);
                        enemiesKilled++;
                    }
                }
                if (enemiesKilled >= totalEnemies)
                {
                    myMission.completeObjective(this);
                }
                break;

        }
    }
    public void Complete()
    {
        switch (oType)
        {
            case ObjectiveType.interact:
                interacted = true;
                break;
            case ObjectiveType.reach:
                reached = true;
                break;
        }
    }
}