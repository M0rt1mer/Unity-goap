using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Generic action for executing a single transition in LSM.
/// </summary>
/// <typeparam name="Settings">IReGoapActionSettings class used by this transition. Factory uses it to initialize the transition</typeparam>
/// <typeparam name="Factory">Child of LSMTransitionFactory, that can make the desired transition from this action's settings </typeparam>
/// <typeparam name="Transition">The transition type used by this action</typeparam>
public abstract class ActionExecuteTransition<Settings,Factory,Transition> : GoapAction where Factory : LSMTransitionFactory<Settings,Transition> where Settings : IReGoapActionSettings where Transition : LSMTransition {

    public Factory transition;
    ITransitionExecutor executor;

    protected override void Awake(){
        base.Awake();
    }

    public override void Run(IReGoapAction previous, IReGoapAction next, IReGoapActionSettings settings, ReGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail) {
        base.Run(previous, next, settings, goalState, done, fail);
        executor.ExecuteTransition( transition.MakeTransition((Settings)settings), OnTransitionDone, OnTransitionFailed);
    }

    public void OnTransitionDone() { }

    public void OnTransitionFailed() { }

    public override bool IsInterruptable(){
        return transition.IsInterruptable();
    }

}
