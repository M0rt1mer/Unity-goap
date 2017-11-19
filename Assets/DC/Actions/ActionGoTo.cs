using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionGoTo : ActionExecuteTransition<ActionGoToSettings,TransitionGoToNavigatedFactory,TransitionGoToNavigated,TransitionGoToNavigatedInitializer> {

    public override IReGoapActionSettings Precalculations( IReGoapAgent goapAgent, ReGoapState goalState ) {
        base.Precalculations( goapAgent, goalState );

        if(goalState.GetValues().ContainsKey( WorldStates.STATE_POSITION )) {

            effects.Clear();
            var settings = new ActionGoToSettings {
                target = goalState.Get<Vector3>( WorldStates.STATE_POSITION )
            };
            effects.Set( WorldStates.STATE_POSITION, (settings as ActionGoToSettings).target );
            return settings;
        }
        return null;
    }

    public override float GetCost( ReGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null ) {
        float distance = Vector3.Distance( agent.GetMemory().GetWorldState().Get<Vector3>( WorldStates.STATE_POSITION ), (settings as ActionGoToSettings).target );
        return base.GetCost( goalState, settings, next ) + distance;
    }

}

public class ActionGoToSettings : TransitionGoToNavigatedInitializer, IReGoapActionSettings {}
