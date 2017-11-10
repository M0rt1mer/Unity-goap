using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "GOAP/Transitions/GoTo")]
public class TransitionGoToNavigatedFactory : StateMachineTransitionFactory<TransitionGoToNavigatedInitializer, TransitionGoToNavigated> {

    public override bool IsInterruptable() {
        return true;
    }

}

public class TransitionGoToNavigated : StateMachineTransition<TransitionGoToNavigatedInitializer> {

    public TransitionGoToNavigated( TransitionGoToNavigatedInitializer initializer, StateMachineLayer layer, Action<StateMachineTransition<TransitionGoToNavigatedInitializer>> OnDone, Action<StateMachineTransition<TransitionGoToNavigatedInitializer>> OnFailed ) : base( initializer, layer, OnDone, OnFailed ) {
    }

    NavMeshAgent navAgent;

    public override void Initialize( GameObject agent ) {
        base.Initialize( agent );
        navAgent = agent.GetComponent<NavMeshAgent>();
        navAgent.SetDestination( initializer.target );
    }

    public override bool Update() {
        if(navAgent.remainingDistance < 0.01f) {
            OnDone(this);
            return true;
        }
        return false;
    }
}

public class TransitionGoToNavigatedInitializer {

    public Vector3 target { get; set; }

}
