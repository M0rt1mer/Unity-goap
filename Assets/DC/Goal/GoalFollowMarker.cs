using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalFollowMarker : GoapGoal {

    protected override void Awake() {
        base.Awake();
        goal.Set( WorldStates.STATE_POSITION, GameObject.Find( "Cube" ).transform.position );
    }

    public override float GetPriority() {
        return 0.1f;
    }

}
