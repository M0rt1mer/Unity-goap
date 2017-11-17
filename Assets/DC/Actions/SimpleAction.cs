using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Simple action is defined by three sets:
/// - effects - a set of effects. Some of them can be parametrized (if effect is parametrized, action can produce any value of the effect)
/// - preconditions - 
/// - run function
/// </summary>
public abstract class SimpleAction : ScriptableObject, IReGoapAction {

    public string name;

    private ReGoapState staticEffects;
    IWorldState[] parametrizedEffects;
    private ReGoapState staticPreconditions;

    protected abstract void InitializePreconditionsAndEffects( ref ReGoapState staticEffects, ref IWorldState[] parametrizedEffects, ref ReGoapState staticPreconditions );

    public void OnEnable(){
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

    protected abstract ReGoapState GetPreconditionsFromGoal( ReGoapState goal );

    public ReGoapState GetPreconditions( ReGoapState goalState, IReGoapAction next = null ) {
        ReGoapState variablePreconditions = GetPreconditionsFromGoal( goalState );
        if( variablePreconditions == null || variablePreconditions.GetValues().Count == 0)
            return staticPreconditions;
        else {
            return staticPreconditions + variablePreconditions;
        }
    }

    protected abstract IEnumerator Execute( SimpleActionSettings settings, Action fail );

    IReGoapActionSettings IReGoapAction.GetSettings(IReGoapAgent goapAgent, ReGoapState goalState){
        return new SimpleActionSettings { agent = goapAgent as GoapAgent, effects = ExtractEffectsFromGoal( goalState ) };
    }

    IEnumerator IReGoapAction.Run(IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settings, ReGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail){
        IEnumerator progress = Execute( settings as SimpleActionSettings, () => { fail( this ); } );
        while(progress.MoveNext())
            yield return progress.Current;
        done(this);
    }

    void IReGoapAction.Exit(IReGoapAction nextAction){}

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

    ReGoapState IReGoapAction.GetEffects(ReGoapState goalState, IReGoapAction next){
        return ExtractEffectsFromGoal( goalState );
    }

    bool IReGoapAction.CheckProceduralCondition(IReGoapAgent goapAgent, ReGoapState goalState, IReGoapAction nextAction){
        return true;
    }

    float IReGoapAction.GetCost(ReGoapState goalState, IReGoapAction next){
        return 1;
    }

    /// <summary>
    /// Not used by this implementation. All precalculations are done in GetPreconditions
    /// </summary>
    /// <param name="goapAgent"></param>
    /// <param name="goalState"></param>
    void IReGoapAction.Precalculations(IReGoapAgent goapAgent, ReGoapState goalState){}

}

public class SimpleActionSettings : IReGoapActionSettings {

    public GoapAgent agent { get; set; }
    public ReGoapState effects { get; set; }

}