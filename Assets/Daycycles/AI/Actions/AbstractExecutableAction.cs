using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractExecutableAction : ScriptableObject, IReGoapAction
{

    public abstract void AskForInterruption(IReGoapActionSettings settings);
    public abstract bool CheckProceduralCondition(IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction = null);
    public abstract void Exit(IReGoapAction nextAction);
    public abstract float GetCost(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);
    public abstract BGoapState GetEffects(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);
    public abstract Dictionary<string, object> GetGenericValues();
    public abstract string GetName();
    public abstract BGoapState GetPreconditions(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);
    public abstract bool IsActive();
    public abstract bool IsInterruptable();
    public abstract void PostPlanCalculations(IReGoapAgent goapAgent);
    public abstract IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, BGoapState goalState);
    public abstract IEnumerable<IReGoapActionSettings> MultiPrecalculations(IReGoapAgent goapAgent, BGoapState goalState);
    public abstract IEnumerator Run(IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settings, BGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail);

}