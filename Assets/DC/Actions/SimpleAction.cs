using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple action is defined by three sets:
/// - effects - a set of effects. Some of them can be parametrized (if effect is parametrized, action can produce any value of the effect)
/// - preconditions - 
/// - run function
/// </summary>
public abstract class SimpleAction <Settings> : ScriptableObject, IReGoapAction where Settings : SimpleActionSettings, new() {

    public string name;

    [System.NonSerialized]
    private ReGoapState staticEffects;
    [System.NonSerialized]
    List<IWorldState> parametrizedEffects;
    [System.NonSerialized]
    private ReGoapState staticPreconditions;

    /// <summary>
    /// Can be overriden (likely will be overriden, but for some actions it may be empty)
    /// </summary>
    /// <param name="staticEffects"></param>
    /// <param name="parametrizedEffects"></param>
    /// <param name="staticPreconditions"></param>
    protected virtual void InitializePreconditionsAndEffects(ref ReGoapState staticEffects, ref List<IWorldState> parametrizedEffects, ref ReGoapState staticPreconditions) { }

    public void OnEnable(){
        staticEffects = new ReGoapState();
        staticPreconditions = new ReGoapState();
        parametrizedEffects = new List<IWorldState>();
        InitializePreconditionsAndEffects( ref staticEffects, ref parametrizedEffects, ref staticPreconditions );
    }

    /// <summary>
    /// Creates a new Effects set based on parametrized world states
    /// </summary>
    /// <param name="goalState"></param>
    /// <returns></returns>
    protected ReGoapState ExtractEffectsFromGoal( ReGoapState goalState ) {
        ReGoapState newState = new ReGoapState( staticEffects );
        foreach(IWorldState state in parametrizedEffects) {
            if(goalState.HasKey( state )) {
                newState.SetFrom( state, goalState );
            }
        }
        return newState;
    }

    protected abstract ReGoapState GetPreconditionsFromGoal( ReGoapState goalState, Settings settings );

    ReGoapState IReGoapAction.GetPreconditions( ReGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null ) {
        ReGoapState variablePreconditions = GetPreconditionsFromGoal( goalState, settings as Settings);
        if( variablePreconditions == null || variablePreconditions.GetValues().Count == 0)
            return staticPreconditions;
        else {
            return staticPreconditions + variablePreconditions;
        }
    }

    protected abstract IEnumerator Execute(Settings settings, Action fail );

    public virtual IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, ReGoapState goalState){
        return new Settings { agent = goapAgent as GoapAgent, effects = ExtractEffectsFromGoal( goalState ) };
    }

    public IEnumerator Run(IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settings, ReGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail){
        IEnumerator progress = Execute( settings as Settings, () => { fail( this ); } );
        while(progress.MoveNext())
            yield return progress.Current;
        done(this);
    }

    public void Exit(IReGoapAction nextAction){}

    //not used, therefore not implemented
    Dictionary<string, object> IReGoapAction.GetGenericValues()
    {
        throw new NotImplementedException();
    }

    string IReGoapAction.GetName()
    {
        return name;
    }

    // used in oneActionPerActor
    bool IReGoapAction.IsActive()
    {
        throw new NotImplementedException();
    }

    void IReGoapAction.PostPlanCalculations(IReGoapAgent goapAgent){}

    bool IReGoapAction.IsInterruptable() { return true; }

    void IReGoapAction.AskForInterruption(){}

    ReGoapState IReGoapAction.GetEffects(ReGoapState goalState, IReGoapActionSettings settings, IReGoapAction next){
        return (settings as SimpleActionSettings).effects;
    }

    bool IReGoapAction.CheckProceduralCondition(IReGoapAgent goapAgent, IReGoapActionSettings settings, ReGoapState goalState, IReGoapAction nextAction){
        return true;
    }

    float IReGoapAction.GetCost(ReGoapState goalState, IReGoapActionSettings settings, IReGoapAction next){
        return 1;
    }

}

public class SimpleActionSettings : IReGoapActionSettings {

    public GoapAgent agent { get; set; }
    public ReGoapState effects { get; set; }

}