using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPub : GoapGoal
{

    protected override void Awake()
    {
        base.Awake();
        foreach (RPGSmartObjectSimpleState so in GameObject.FindObjectsOfType<RPGSmartObjectSimpleState>())
            goal.Set(so.GetIndicatingWorldState(), true);

        //goal.Set(WorldStates.GROUND_SWEEPED,true);
    }

    public override float GetPriority()
    {
        return 2;
    }

}
