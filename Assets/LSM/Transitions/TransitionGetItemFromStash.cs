using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Transitions/Get Item From Stash")]
public class TransitionFactoryGetItemFromStash : StateMachineTransitionFactory<TransitionGetItemFromStashInitializer, TransitionGetItemFromStash>
{
    public override bool IsInterruptable(){
        return true;
    }

}


public class TransitionGetItemFromStash : StateMachineTransition<TransitionGetItemFromStashInitializer> {

    public TransitionGetItemFromStash(TransitionGetItemFromStashInitializer initializer, StateMachineLayer layer, Action<StateMachineTransition<TransitionGetItemFromStashInitializer>> OnDone, Action<StateMachineTransition<TransitionGetItemFromStashInitializer>> OnFailed) : base(initializer, layer, OnDone, OnFailed)
    {
    }

    public override bool Update() {
        return true;
    }
}

public class TransitionGetItemFromStashInitializer {

}