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
class SimpleActionGoto<Settings> : SimpleAction<Settings> where Settings: SimpleActionSettings, new() {

    protected override void InitializePreconditionsAndEffects( ref ReGoapState staticEffects, ref List<IWorldState> parametrizedEffects, ref ReGoapState staticPreconditions ) {
        parametrizedEffects.Add( WorldStates.STATE_POSITION );
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
class SimpleActionGoTo : SimpleActionGoto<SimpleActionSettings> { }