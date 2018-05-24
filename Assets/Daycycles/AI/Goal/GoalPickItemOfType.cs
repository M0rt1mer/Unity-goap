using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPickItemOfType : GoapGoal
{

    public DBItem requiredItemType;

    protected override void Awake()
    {
        base.Awake();
        goal.Set(WorldStates.STATE_HAND_RIGHT, requiredItemType);
    }

    public override float GetPriority()
    {
        return 2;
    }

}
