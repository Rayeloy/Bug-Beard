using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Boss_Keeper : Mission
{
    public Mission_Boss_Keeper(string _missionName, MissionType _mType) : base(_missionName,_mType)
    {
        missionName = _missionName;
        mType = _mType;
    }
    public override void completeObjective(Objective _objective)
    {
        base.completeObjective(_objective);
        switch (currentObjective.ObjectiveName)
        {
            case "Derrota al golem":
                break;
        }
    }
    public override void konoUpdate()
    {
        base.konoUpdate();
 
    }

}
