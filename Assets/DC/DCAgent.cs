using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DCAgent : GoapAgent, IReGoapMemory {

    public DCGoal[] goalSet;
    public DCAction[] actionSet;
    public IDCSensor[] sensors;

    public override List<IReGoapAction> GetActionsSet() {
        return new List<IReGoapAction>( actionSet );
    }

    public override List<IReGoapGoal> GetGoalsSet() {
        return new List<IReGoapGoal>( goalSet );
    }

    //doesn't use memory, manages world state by itself
    public override void RefreshMemory() {}

    protected ReGoapState currentState;

    ReGoapState IReGoapMemory.GetWorldState() {
        currentState = new ReGoapState();
        foreach (var sensor in sensors)
            sensor.UpdateWorld( currentState, this );
        return currentState;
    }

}

