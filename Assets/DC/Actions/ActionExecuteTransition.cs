using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Generic action for executing a single transition in LSM.
/// </summary>
/// <typeparam name="Settings">IReGoapActionSettings class used by this transition. Factory uses it to initialize the transition</typeparam>
/// <typeparam name="Factory">Child of LSMTransitionFactory, that can make the desired transition from this action's settings </typeparam>
/// <typeparam name="Transition">The transition type used by this action</typeparam>
public abstract class ActionExecuteTransition<Settings,Factory,Transition,Initializer> : GoapAction 
    where Factory : StateMachineTransitionFactory<Initializer,Transition> where Settings : IReGoapActionSettings, Initializer where Transition : StateMachineTransition<Initializer> {

    public Factory transition;
    ITransitionExecutor executor;

    protected override void Awake(){
        base.Awake();
        executor = GetComponent<ITransitionExecutor>();
    }

    public override IEnumerator Run(IReGoapAction previous, IReGoapAction next, IReGoapActionSettings settings, ReGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail) {
        base.Run(previous, next, settings, goalState, done, fail);
        executor.ExecuteTransition( transition.MakeTransition( (Initializer)settings, 
            (StateMachineTransition<Initializer> transition) => done(this), (StateMachineTransition<Initializer> transition) => fail(this) ) );
        yield return null;
    }

    public override bool IsInterruptable(){
        return transition.IsInterruptable();
    }

}