using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC2GoalFollowMarker : GoapGoal {

    protected override void Awake() {
        base.Awake();
        goal.Set( WorldStates.STATE_POSITION, GameObject.Find( "Cube" ).transform.position );
    }

}
