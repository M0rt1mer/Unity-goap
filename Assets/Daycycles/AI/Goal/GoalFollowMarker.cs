using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalFollowMarker : GoapGoal {

    protected override void Awake() {
        base.Awake();
        goal.Set(WorldStates.STATE_POSITION, GameObject.FindGameObjectWithTag("Marker").transform.position);
    }

    public override float GetPriority() {
        return 0.1f;
    }

    public void Update()
    {
        goal.Set(WorldStates.STATE_POSITION, GameObject.FindGameObjectWithTag("Marker").transform.position);
    }

}
