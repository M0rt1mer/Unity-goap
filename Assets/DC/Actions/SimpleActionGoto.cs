using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "GOAP/SimpleActions/GoTo")]
class SimpleActionGoto : SimpleAction {

    protected override void InitializePreconditionsAndEffects( ref ReGoapState staticEffects, ref IWorldState[] parametrizedEffects, ref ReGoapState staticPreconditions ) {
        staticEffects = new ReGoapState();
        parametrizedEffects = new IWorldState[] { WorldStates.STATE_POSITION };
        staticPreconditions = new ReGoapState();
        Debug.Log( "Initializnf SimpleActionGoto" );
    }

    protected override IEnumerator Execute( SimpleActionSettings settings, Action fail ) {

        var navMeshAgent = settings.agent.GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination( settings.effects.Get( WorldStates.STATE_POSITION) );
        yield return new WaitForFixedUpdate();

        while(navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance || (navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude > 0f) ) {
            yield return new WaitForSeconds( 0.5f );
        }

    }

    protected override ReGoapState GetPreconditionsFromGoal( ReGoapState goal ) {
        return null;
    }
    
}
