using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleActionBase : ScriptableObject, IReGoapAction {

    public abstract void AskForInterruption( IReGoapActionSettings settings );
    public abstract bool CheckProceduralCondition( IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction = null );
    public abstract void Exit( IReGoapAction nextAction );
    public abstract float GetCost( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null );
    public abstract BGoapState GetEffects( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null );
    public abstract Dictionary<string, object> GetGenericValues();
    public abstract string GetName();
    public abstract BGoapState GetPreconditions( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null );
    public abstract bool IsActive();
    public abstract bool IsInterruptable();
    public abstract void PostPlanCalculations( IReGoapAgent goapAgent );
    public abstract IReGoapActionSettings Precalculations( IReGoapAgent goapAgent, BGoapState goalState );
    public abstract IEnumerator Run( IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settings, BGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail );
}
/// <summary>
/// Simple action is defined by three sets:
/// - effects - a set of effects. Some of them can be parametrized (if effect is parametrized, action can produce any value of the effect)
/// - preconditions - 
/// - run function
/// </summary>
public abstract class SimpleAction <Settings> : SimpleActionBase where Settings : SimpleActionSettings, new() {

    private BGoapState staticEffects;
    private BGoapState parametrizedEffectsWithDefaults;
    private BGoapState staticPreconditions;

    public void OnEnable(){
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

    protected abstract IEnumerator<SimpleActionExecutionControlElements> Execute( Settings settings, Action fail );

    public override sealed IReGoapActionSettings Precalculations( IReGoapAgent goapAgent, BGoapState goalState ) {
        return new Settings { agent = goapAgent as GoapAgent, effects = ExtractEffectsFromGoal(goalState) };
    }

    public override float GetCost( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next ) {
        return 1;
    }

    public override bool CheckProceduralCondition( IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction ) {
        return true;
    }

    public override bool IsInterruptable() { return true; }
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

    public override void Exit(IReGoapAction nextAction){}

    #region ========================================================================================================================= sealed
    public override sealed BGoapState GetEffects( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next ) {
        return (settings as SimpleActionSettings).effects;
    }

    public override sealed IEnumerator Run( IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settingsParam, BGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail ) {
        Settings settings = settingsParam as Settings;
        IEnumerator<SimpleActionExecutionControlElements> progress = Execute( settings, () => { fail( this ); } );
        while(progress.MoveNext()) {
            if( settings.interruptNextChanceYouHave && progress.Current!=SimpleActionExecutionControlElements.CANNOT_INTERRUPT ) {
                done( this );
                yield break;
            }
            if(progress.Current == SimpleActionExecutionControlElements.WAIT_NEXT_FRAME)
                yield return new WaitForFixedUpdate();
            yield return progress.Current;
        }
        done( this );
    }

    public override sealed BGoapState GetPreconditions( BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null ) {
        BGoapState variablePreconditions = GetPreconditionsFromGoal( goalState, settings as Settings );
        if(variablePreconditions == null || variablePreconditions.IsEmpty() )
            return staticPreconditions;
        else {
            return staticPreconditions + variablePreconditions;
        }
    }

    public override sealed void AskForInterruption( IReGoapActionSettings settings ) {
        (settings as SimpleActionSettings).interruptNextChanceYouHave = true;
    }

    public override string GetName() {
        return name;
    }
    #endregion

    public override string ToString() {
        return name;
    }

    #region =========================================================================================================================== not implemented
    //not used, therefore not implemented
    public override sealed Dictionary<string, object> GetGenericValues() {
        throw new NotImplementedException();
    }

    public override void PostPlanCalculations( IReGoapAgent goapAgent ) { }

    // used in oneActionPerActor
    public override bool IsActive() {
        throw new NotImplementedException();
    }
    #endregion

}

public enum SimpleActionExecutionControlElements {
    NORMAL, CANNOT_INTERRUPT, WAIT_NEXT_FRAME
}

public class SimpleActionSettings : IReGoapActionSettings {

    public GoapAgent agent { get; set; }
    public BGoapState effects { get; set; }

    public bool interruptNextChanceYouHave { set; get; }

}