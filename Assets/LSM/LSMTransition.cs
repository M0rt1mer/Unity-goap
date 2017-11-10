using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Generic class for transitions. Actual factory methods differ per instance
/// </summary>
public abstract class StateMachineTransitionFactory<Initializer,Transition> : ScriptableObject where Transition : StateMachineTransition<Initializer> {

    [SerializeField]
    private StateMachineLayer layer;
    public StateMachineLayer Layer { get { return layer; } }

    public abstract bool IsInterruptable();

    /// <summary>
    /// If Transition doesn't have constructor with same signature as StateMachineTransition, you need to override this method
    /// </summary>
    /// <param name="init"></param>
    /// <param name="OnDone"></param>
    /// <param name="OnFailed"></param>
    /// <returns></returns>
    public virtual Transition MakeTransition( Initializer init, Action<StateMachineTransition<Initializer>> OnDone, Action<StateMachineTransition<Initializer>> OnFailed){
        //return new Transition( init, Layer, OnDone, OnFailed );
        return Activator.CreateInstance( typeof(Transition), init, Layer, OnDone, OnFailed) as Transition;
    }

}

public interface IStateMachineTransition {
    bool Update();
    StateMachineLayer Layer { get; }
    void Initialize(GameObject agent);
}

/// <summary>
/// A generic parent for all transitions. Contains a constructor and a very basic structure. Only update needs to be overloaded
/// </summary>
/// <typeparam name="Initializer"></typeparam>
public abstract class StateMachineTransition<Initializer> : IStateMachineTransition{

    private readonly StateMachineLayer layer;
    protected Action<StateMachineTransition<Initializer>> OnDone;
    protected Action<StateMachineTransition<Initializer>> OnFailed;
    protected Initializer initializer;

    public StateMachineTransition(Initializer initializer, StateMachineLayer layer, Action<StateMachineTransition<Initializer>> OnDone, Action<StateMachineTransition<Initializer>> OnFailed) {
        this.layer = layer;
        this.OnDone = OnDone;
        this.OnFailed = OnFailed;
        this.initializer = initializer;
    }


    protected GameObject agent;

    StateMachineLayer IStateMachineTransition.Layer{
        get{
            return layer;
        }
    }

    public virtual void Initialize( GameObject agent ) {
        this.agent = agent;
    }

    /// <summary>
    /// Updates this transition. Returns true on end (fail, success, or other states should be handled internally)
    /// </summary>
    /// <returns></returns>
    public abstract bool Update();

}
