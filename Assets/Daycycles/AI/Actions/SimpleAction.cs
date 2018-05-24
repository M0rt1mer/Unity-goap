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
public abstract class SimpleAction <Settings> : ExecutableAction<Settings> where Settings : SimpleActionSettings, new() {

    private BGoapState staticEffects;
    private BGoapState parametrizedEffectsWithDefaults;
    private BGoapState staticPreconditions;

    public override void OnEnable(){
        base.OnEnable();
        staticEffects = new BGoapState();
        staticPreconditions = new BGoapState();
        parametrizedEffectsWithDefaults = new BGoapState();
        InitializePreconditionsAndEffects(  staticEffects, ref parametrizedEffectsWithDefaults,  staticPreconditions );
    }

    #region ========================================================================================================  overridable
    /// <summary>
    /// Can be overriden (likely will be overriden, but for some actions it may be empty)
    /// Parametrized effects are set to whatever current goal requires (e.g. if current goal is "stand at a specific position", a parametrized "goto" action can be created, which will move the actor to that exact position
    /// If the current goal doesn't have the WorldState, the value from parametrizedEffects is used instead
    /// </summary>
    /// <param name="staticEffects"></param>
    /// <param name="parametrizedEffects">A ReGoapState containing all WorldStates that will be parametrized</param>
    /// <param name="staticPreconditions"></param>
    protected virtual void InitializePreconditionsAndEffects( BGoapState staticEffects, ref BGoapState parametrizedEffects, BGoapState staticPreconditions ) { }

    protected abstract BGoapState GetPreconditionsFromGoal( BGoapState goalState, Settings settings );

    public override float GetCost( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next ) {
        return 1;
    }

    public override bool CheckProceduralCondition( IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction ) {
        return true;
    }

    #endregion

    /// <summary>
    /// Creates a new Effects set based on parametrized world states
    /// </summary>
    /// <param name="goalState"></param>
    /// <returns></returns>
    protected BGoapState ExtractEffectsFromGoal( BGoapState goalState ) {
            BGoapState newState = new BGoapState( staticEffects );
            foreach(IStateVarKey state in parametrizedEffectsWithDefaults) {
                if(goalState.HasKey( state ))
                    newState.SetFrom( state, goalState );
                else
                    newState.SetFrom( state, parametrizedEffectsWithDefaults );
            }
            return newState;
    }

    #region ========================================================================================================================= sealed
    public override sealed BGoapState GetEffects( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next ) {
        return (settings as SimpleActionSettings).effects;
    }

    public override sealed BGoapState GetPreconditions( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null ) {
        BGoapState variablePreconditions = GetPreconditionsFromGoal( goalState, settings as Settings );
        if(variablePreconditions == null || variablePreconditions.IsEmpty() )
            return staticPreconditions;
        else {
            return staticPreconditions.Union(variablePreconditions);
        }
    }

    public override sealed IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        return new Settings { agent = goapAgent as GoapAgent, effects = ExtractEffectsFromGoal(goalState) };
    }

    public sealed override IEnumerable<IReGoapActionSettings> MultiPrecalculations(IReGoapAgent goapAgent, BGoapState goalState)
    {
        yield return Precalculations(goapAgent, goalState);
    }

    #endregion

}



public class SimpleActionSettings : ExecutableActionSettings {

    public GoapAgent agent { get; set; }
    public BGoapState effects { get; set; }


}