using System;
using System.Collections;
using System.Collections.Generic;

public interface IReGoapAction
{
    IEnumerator Run(IReGoapAction previousAction, IReGoapAction nextAction, IReGoapActionSettings settings, ReGoapState goalState, Action<IReGoapAction> done, Action<IReGoapAction> fail);
    void Exit(IReGoapAction nextAction);
    Dictionary<string, object> GetGenericValues();
    string GetName();
    bool IsActive();
    void PostPlanCalculations(IReGoapAgent goapAgent);
    bool IsInterruptable();
    void AskForInterruption();
    // THREAD SAFE
    ReGoapState GetPreconditions(ReGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);
    ReGoapState GetEffects(ReGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);
    bool CheckProceduralCondition(IReGoapAgent goapAgent, IReGoapActionSettings settings, ReGoapState goalState, IReGoapAction nextAction = null);
    float GetCost(ReGoapState goalState, IReGoapActionSettings settings, IReGoapAction next = null);



    IReGoapActionSettings Precalculations(IReGoapAgent goapAgent, ReGoapState goalState);
}

public interface IReGoapActionSettings
{
}