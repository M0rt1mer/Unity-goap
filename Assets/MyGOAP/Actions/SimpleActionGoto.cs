using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Simple action that moves agent to desired position. Can fulfill any position requirement
/// Templating is done to allow further overriding
/// </summary>
/// <typeparam name="Settings"></typeparam>
public class SimpleActionGoto<Settings> : SimpleAction<Settings> where Settings: SimpleActionSettings, new() {

    protected override void InitializePreconditionsAndEffects( ReGoapState staticEffects, ref ReGoapState parametrizedEffects, ReGoapState staticPreconditions ) {
        parametrizedEffects.Set( WorldStates.STATE_POSITION, default( Vector3 ) ); //since this is the only effect, default doesn't matter
    }

    protected override IEnumerator<SimpleActionExecutionControlElements> Execute(Settings settings, Action fail ) {

        var navMeshAgent = settings.agent.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination( settings.effects.Get( WorldStates.STATE_POSITION) );
        yield return SimpleActionExecutionControlElements.WAIT_NEXT_FRAME;

        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance || (navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude > 0f) ) {
            yield return SimpleActionExecutionControlElements.WAIT_NEXT_FRAME;
        }

    }

    protected override ReGoapState GetPreconditionsFromGoal( ReGoapState goal, Settings settings ) {
        return null;
    }
    
}

[CreateAssetMenu(menuName = "GOAP/SimpleActions/GoTo")]
public class SimpleActionGoTo : SimpleActionGoto<SimpleActionSettings> { }