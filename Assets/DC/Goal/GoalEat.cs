using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalEat : GoapGoal {

    GoapMemory memory;

    protected override void Awake() {
        base.Awake();
        goal.Set<float>( WorldStates.STATE_FLOAT_HUNGER, 1f );
        memory = GetComponent<GoapMemory>();
    }

    public override float GetPriority() {
        return 1 - memory.GetWorldState().Get<float>( WorldStates.STATE_FLOAT_HUNGER );
    }

}
