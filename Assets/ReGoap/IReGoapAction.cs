using System;
using System.Collections;
using System.Collections.Generic;

public interface IReGoapAction
{
    IEnumerator Run(IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settings, BGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail);
    void Exit(IReGoapAction nextAction);
    Dictionary<string, object> GetGenericValues();
    string GetName();
    bool IsActive();
    void PostPlanCalculations(IReGoapAgent goapAgent);
    bool IsInterruptable();
    void AskForInterruption( IReGoapActionSettings settings );
    // THREAD SAFE
    BGoapState GetPreconditions(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);
    BGoapState GetEffects(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);
    bool CheckProceduralCondition(IReGoapAgent goapAgent, IReGoapActionSettings settings, BGoapState goalState, IReGoapAction nextAction = null);
    float GetCost(BGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);



    IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, BGoapState goalState);
}

public interface IReGoapActionSettings
{
}