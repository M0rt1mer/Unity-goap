using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGoTo : ActionExecuteTransition<ActionGoToSettings,TransitionGoToNavigatedFactory,TransitionGoToNavigated,TransitionGoToNavigatedInitializer> {

    public override void Precalculations( IReGoapAgent goapAgent, ReGoapState goalState ) {
        base.Precalculations( goapAgent, goalState );

        if(goalState.GetValues().ContainsKey( WorldStates.STATE_POSITION )) {

            effects.Clear();
            settings = new ActionGoToSettings {
                target = goalState.Get<Vector3>( WorldStates.STATE_POSITION )
            };
            effects.Set( WorldStates.STATE_POSITION, (settings as ActionGoToSettings).target );
        }
    }

    public override float GetCost( ReGoapState goalState, IReGoapAction next = null ) {
        float distance = Vector3.Distance( agent.GetMemory().GetWorldState().Get<Vector3>( WorldStates.STATE_POSITION ), (settings as ActionGoToSettings).target );
        return base.GetCost( goalState, next ) + distance;
    }

}

public class ActionGoToSettings : TransitionGoToNavigatedInitializer, IReGoapActionSettings {}
