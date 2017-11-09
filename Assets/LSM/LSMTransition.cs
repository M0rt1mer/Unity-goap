using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Generic class for transitions. Actual factory methods differ per instance
/// </summary>
public abstract class StateMachineTransitionFactory<Initializer,Transition> : ScriptableObject where Transition : StateMachineTransition{

    [SerializeField]
    private StateMachineLayer layer;
    public StateMachineLayer Layer { get { return layer; } }

    public abstract bool IsInterruptable();

    public abstract Transition MakeTransition( Initializer init, Action<StateMachineTransition> OnDone, Action<StateMachineTransition> OnFailed );

}

public abstract class StateMachineTransition {

    public readonly StateMachineLayer layer;
    protected Action<StateMachineTransition> OnDone;
    protected Action<StateMachineTransition> OnFailed;

    public StateMachineTransition(StateMachineLayer layer, Action<StateMachineTransition> OnDone, Action<StateMachineTransition> OnFailed) {
        this.layer = layer;
        this.OnDone = OnDone;
        this.OnFailed = OnFailed;
    }


    protected GameObject agent;
    public virtual void Initialize( GameObject agent ) {
        this.agent = agent;
    }

    /// <summary>
    /// Updates this transition. Returns true on end (fail, success, or other states should be handled internally)
    /// </summary>
    /// <returns></returns>
    public abstract bool Update();

}
