using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DCGoal : IReGoapGoal {

    public float ErrorDelay = 0.5f;


    public abstract ReGoapState GetGoalState();

    float IReGoapGoal.GetErrorDelay() {
        return ErrorDelay;
    }

    string IReGoapGoal.GetName() {
        throw new NotImplementedException();
    }

    Queue<ReGoapActionState> IReGoapGoal.GetPlan() {
        throw new NotImplementedException();
    }

    float IReGoapGoal.GetPriority() {
        throw new NotImplementedException();
    }

    bool IReGoapGoal.IsGoalPossible() {
        throw new NotImplementedException();
    }

    void IReGoapGoal.Precalculations( IGoapPlanner goapPlanner ) {
        throw new NotImplementedException();
    }

    void IReGoapGoal.Run( Action<IReGoapGoal> callback ) {
        throw new NotImplementedException();
    }

    void IReGoapGoal.SetPlan( Queue<ReGoapActionState> path ) {
        throw new NotImplementedException();
    }

}
